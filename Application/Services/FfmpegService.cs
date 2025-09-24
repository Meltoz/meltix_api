using Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;

namespace Application.Services
{
    public class FfmpegService : IFfMpegService
    {
        private readonly string _ffmpegPath;
        private readonly string _ffprobePath;


        public FfmpegService(IConfiguration configuration)
        {
            _ffmpegPath = configuration["FFmpeg:Path"] ?? "ffmpeg";
            _ffprobePath = configuration["FFmpeg:ProbePath"] ?? "ffprobe";

        }

        public async Task<int> RunCommandAsync(string arguments, CancellationToken cancellationToken = default)
        {
            return await RunProcessAsync(_ffmpegPath, arguments, cancellationToken);
        }

        public async Task<string> RunProbeAsync(string arguments, CancellationToken cancellationToken = default)
        {
            return await RunProcessAndCaptureOutputAsync(_ffprobePath, arguments, cancellationToken);
        }

        private async Task<int> RunProcessAsync(string exePath, string arguments, CancellationToken cancellationToken)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = exePath,
                    Arguments = arguments,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            Debug.WriteLine($"Exécution: {exePath} {arguments}");

            process.Start();

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            process.OutputDataReceived += (s, e) =>
            {
                if (!string.IsNullOrWhiteSpace(e.Data))
                    Console.WriteLine($"[ffmpeg-out] {e.Data}");
            };

            process.ErrorDataReceived += (s, e) =>
            {
                if (!string.IsNullOrWhiteSpace(e.Data))
                    Console.WriteLine($"[ffmpeg-err] {e.Data}");
            };

            await process.WaitForExitAsync(cancellationToken);

            return process.ExitCode;
        }

        private async Task<string> RunProcessAndCaptureOutputAsync(string exePath, string arguments, CancellationToken cancellationToken)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = exePath,
                    Arguments = arguments,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();

            string output = await process.StandardOutput.ReadToEndAsync();
            string error = await process.StandardError.ReadToEndAsync();

            await process.WaitForExitAsync(cancellationToken);

            if (process.ExitCode != 0)
            {
                throw new Exception($"ffprobe failed: {error}");
            }

            return output;
        }
    }
}
