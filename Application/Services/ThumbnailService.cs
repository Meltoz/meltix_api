using Application.Interfaces.Services;

namespace Application.Services
{
    public class ThumbnailService(IFfMpegService fs) : IThumbnailService
    {
        private readonly IFfMpegService _ffMpegService = fs;

        public async Task<string> GenerateThumbnailAsync(string videoPath, string outputPath, CancellationToken cancellationToken = default)
        {
            string input = $"\"{Path.GetFullPath(videoPath).Replace("\\", "/")}\"";
            string output = $"\"{Path.GetFullPath(outputPath).Replace("\\", "/")}\"";

            var args = new List<string>
            {
            "-ss", "00:00:01",
            "-i", input,
            "-frames:v", "1",
            "-q:v", "2",
            "-y",
            "-update", "1",
            output
            };

            var t = string.Join(" ", args);
            int exitCode = await _ffMpegService.RunCommandAsync(t, cancellationToken);

            if (exitCode != 0)
            {
                throw new Exception($"FFmep failed with exit code {exitCode}, arguments: {t}");
            }

            return outputPath;

        }
    }
}
