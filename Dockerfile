#Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /App

COPY . ./

RUN dotnet restore
RUN dotnet publish -c Release -o out

#Serve Stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0

# Installer ffmpeg Linux
RUN apt-get update && \
    apt-get install -y ffmpeg && \
    rm -rf /var/lib/apt/lists/*
WORKDIR /App
COPY --from=build /App/out .
ENTRYPOINT ["dotnet", "Web.dll"]