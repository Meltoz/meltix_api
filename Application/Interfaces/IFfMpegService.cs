namespace Application.Interfaces
{
    public interface IFfMpegService
    {
        Task<int> RunCommandAsync(string arguments, CancellationToken cancellationToken = default);

        Task<string> RunProbeAsync(string arguments, CancellationToken cancellationToken = default);
    }
}
