using Application.Interfaces.Services;

namespace Application.Services
{
    public class ThumbnailService(IFfMpegService fs) : IThumbnailService
    {
        private readonly IFfMpegService _ffMpegService = fs;

        public async Task<string> GenerateThumbnailAsync(string videoPath, string outputPath, TimeSpan timecode, CancellationToken cancellationToken = default)
        {
            var args = new List<string>
            {
            "-ss", timecode.ToString(),
             "-i", videoPath,                           
            "-frames:v", "1",
            "-q:v", "2",
            "-y",
            "-update", "1",
            outputPath
            };

            var t = string.Join(" ", args);
            int exitCode = await _ffMpegService.RunCommandAsync(t, cancellationToken);

            if (exitCode != 0)
            {
                throw new Exception($"FFmep failed with exit code {exitCode}, arguments: {t}");
            }
            
            return Path.GetFileName(outputPath);
        }
    }
}
