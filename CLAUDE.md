# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Meltix is a video management and streaming API built with ASP.NET Core. The application automatically scans video folders, extracts metadata using FFmpeg, generates thumbnails, and provides a RESTful API for video organization and streaming with JWT-based authentication.

## Architecture

This codebase follows **Clean Architecture** principles with clear layer separation:

- **Domain**: Core business entities (Video, User, Category, Tag) with rich domain models and value objects (Pseudo, Password). No external dependencies except Shared.
- **Application**: Business logic and service interfaces. Contains 12 services including VideoService (core CRUD and folder sync), FfmpegService, ThumbnailService, MediaInfoService, and authentication services (JwtService, TokenService, AesEncryptionService).
- **Infrastructure**: EF Core data access with SQLite, repositories following the Repository pattern with a generic base repository.
- **Web**: ASP.NET Core Web API entry point with controllers, view models, middleware (ExceptionHandlingMiddleware).
- **Shared**: Cross-cutting utilities (SlugGenerator, PagedResult, custom exceptions, configuration classes).
- **MigrationRunner**: Standalone console app for manual database migrations.

All projects target **.NET 10.0**.

### Key Patterns

- **Domain-Driven Design**: Rich domain entities with encapsulated business logic (e.g., `Video.ChangeTitle()`, `User.ChangePassword()`), value objects with validation
- **Repository Pattern**: Generic base repository with specialized implementations for each entity
- **Dependency Injection**: Services registered in Application/DependencyInjection.cs and Infrastructure layer
- **AutoMapper**: Three separate mapping profiles (DtoToEntityProfile, EntityToDtoProfile, DtoToViewModelProfile)
- **Options Pattern**: Strongly-typed configuration (AuthConfiguration, FfmpegConfiguration, EncryptionConfiguration, ConnectionConfiguration)

### Core Domain Entities

- **Video**: Title, Path, Thumbnail, Slug, Duration, CategoryId. Relationships: Many-to-Many with Tags, Many-to-One with Category (nullable).
- **User**: Uses Pseudo and Password value objects. Relationships: One-to-Many with TokenInfo.
- **Category**: Simple name-based categorization.
- **Tag**: Automatically lowercased tag values for consistency.
- **TokenInfo**: Encrypted JWT access and refresh tokens.

All entities inherit from BaseEntity (Id, Created, Updated fields).

### Database

- **Technology**: SQLite via Entity Framework Core 10.0.1
- **DbContext**: `Infrastructure/Data/MeltixContext.cs`
- **Migrations**: Applied automatically on startup in `Web/Program.cs` line 100
- **Repositories**: Located in `Infrastructure/Data/Repositories/`

## Common Commands

### Build and Run

```bash
# Restore dependencies
dotnet restore

# Build solution
dotnet build

# Run the API (from Web directory)
cd Web
dotnet run

# Build for release
dotnet build -c Release

# Publish for deployment
dotnet publish -c Release -o out
```

### Testing

```bash
# Run all tests
dotnet test

# Run unit tests only
dotnet test Meltix.UnitTests/Meltix.UnitTests.csproj

# Run integration tests only
dotnet test Meltix.IntegrationTests/Meltix.IntegrationTests.csproj

# Run tests with coverage
dotnet test /p:CollectCoverage=true

# Run specific test
dotnet test --filter "FullyQualifiedName~TestNamespace.TestClass.TestMethod"
```

**Test Stack**: xUnit with Moq (unit tests) and Microsoft.AspNetCore.Mvc.Testing (integration tests).

### Database Migrations

```bash
# Create new migration (from Infrastructure directory)
cd Infrastructure
dotnet ef migrations add MigrationName --startup-project ../Web

# Apply migrations manually (alternative to automatic startup migration)
cd MigrationRunner
dotnet run

# Remove last migration (before applying)
cd Infrastructure
dotnet ef migrations remove --startup-project ../Web

# View migration SQL
dotnet ef migrations script --startup-project ../Web
```

### Docker

```bash
# Build Docker image
docker build -t meltix_api .

# Run container
docker run -p 8080:8080 meltix_api
```

The Dockerfile uses multi-stage builds (.NET 9.0 SDK and runtime) and includes FFmpeg installation for video processing.

## Configuration

Configuration is managed through `Web/appsettings.json` and `Web/appsettings.development.json`. Required settings:

- **FFmpeg.Path**: Path to ffmpeg executable
- **FFmpeg.ProbePath**: Path to ffprobe executable
- **Connection.ConnectionString**: SQLite database file path
- **Connection.Thumbnails**: Directory for thumbnail storage
- **Connection.VideosFolder**: Directory to scan for videos
- **Auth.Key**: JWT signing key
- **Encryption.Key**: AES encryption key (32 bytes base64)
- **Encryption.IV**: AES initialization vector (16 bytes base64)

The `FolderScanService` background service automatically scans the VideosFolder every hour to sync videos with the database.

## Video Processing

Video operations use FFmpeg through several coordinated services:

- **VideoService**: Orchestrates video operations, batch processing uses Channel-based producer-consumer pattern (max 10 concurrent, batches of 20)
- **MediaInfoService**: Extracts metadata (duration, resolution) using ffprobe
- **ThumbnailService**: Generates thumbnails at configurable timecode
- **FfmpegService**: Low-level FFmpeg process wrapper

When a video title changes via `Video.ChangeTitle()`, the physical file is automatically renamed to match the new slug.

## Release Process

Releases are automated via GitHub Actions (`.github/workflows/release.yml`):

1. Push to `release/*` branch (e.g., `release/1.2.3`)
2. Workflow extracts version from branch name
3. Builds and pushes Docker image to `meltoz/meltix_api:VERSION` and `:latest`
4. Merges release branch into master with force push
5. Creates git tag `vVERSION` on master
6. Deletes release branch

## API Features

- **Authentication**: JWT with access/refresh tokens stored as encrypted cookies
- **Authorization**: Role-based (Administrator, User)
- **Video Management**: CRUD, search (scopes: All, TitleDescription, Category, Tags, Uncategorised), pagination, recommendations based on tags and category
- **Video Streaming**: Range request support for video playback
- **CORS**: Configured for localhost:3000 and localhost:30000
- **Swagger**: Available at `/swagger` in development mode
- **Exception Handling**: Global middleware with custom exceptions (EntityNotFoundException, TagAlreadyExistException)

Response pagination uses custom header `X-Total-Count` (defined in `Web/Constantes/ApiConstantes.cs`).

## Important File Locations

- Entry point: `Web/Program.cs`
- Database configuration: `Infrastructure/Data/MeltixContext.cs`
- Core business logic: `Application/Services/VideoService.cs`
- Domain models: `Domain/Entities/`
- Value objects: `Domain/ValueObjects/`
- Repository implementations: `Infrastructure/Data/Repositories/`
- Controllers: `Web/Controllers/`
- AutoMapper profiles: `Application/Mappings/` and `Web/Mappings/`
