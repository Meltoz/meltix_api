namespace Application.Interfaces.Services
{
    public interface IThumbnailService
    {
        public Task<string> GenerateThumbnailAsync(string videoPath, string outputPath, CancellationToken cancellationToken =default);
    }
}
