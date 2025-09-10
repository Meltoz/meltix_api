
using Application.DTOs;
using Application.Interfaces;
using System.Globalization;
using System.Text.Json;
using Xabe.FFmpeg;

namespace Application.Services
{
    public class MediaInfoService : IMediaInfoService
    {
        private readonly IFfMpegService _ffmpegService;
        public MediaInfoService(IFfMpegService fs) 
        {
            _ffmpegService = fs;
        }

        public async Task<MediaInfoDTO> GetMediaInfoAsync(string videoPath, CancellationToken cancellationToken = default)
        {
            string input = Path.GetFullPath(videoPath).Replace("\\", "/");
            var args = new List<string>
            {
                "-v", "quiet",
                "-print_format", "json",
                "-show_format",
                "-show_streams",
                input
            };
            
            var argument = string.Join(" ", args);
            string json = await _ffmpegService.RunProbeAsync(argument, cancellationToken);

            using var doc = JsonDocument.Parse(json);

            var format = doc.RootElement.GetProperty("format");
            var streams = doc.RootElement.GetProperty("streams");

            var videoStream = streams.EnumerateArray().FirstOrDefault(s => s.GetProperty("codec_type").GetString() == "video");

            return new MediaInfoDTO
            {
                FormatName = format.GetProperty("format_name").GetString() ?? string.Empty,
                Codec = format.GetProperty("format_name").GetString() ?? string.Empty,
                Duration = double.TryParse(format.GetProperty("duration").GetString(), NumberStyles.Float, CultureInfo.InvariantCulture, out var d) ? (int)Math.Round(d) : 0,
                Width = videoStream.ValueKind != JsonValueKind.Undefined ? videoStream.GetProperty("width").GetInt32() : 0,
                Height = videoStream.ValueKind != JsonValueKind.Undefined ? videoStream.GetProperty("height").GetInt32() : 0
            };
        }
    }
}
