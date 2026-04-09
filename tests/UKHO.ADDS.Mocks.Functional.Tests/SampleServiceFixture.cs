using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace UKHO.ADDS.Mocks.Functional.Tests
{
    internal sealed class SampleServiceFixture
    {
        private readonly List<string> _processOutput = [];
        private Process? _process;

        public int Port { get; } = GetFreeTcpPort();

        public Uri BaseAddress => new($"http://{IPAddress.Loopback}:{Port}");

        public async Task StartAsync()
        {
            var repoRoot = FindRepoRoot();
            var sampleServiceProject = Path.Combine(
                repoRoot,
                "src",
                "UKHO.ADDS.Mocks.SampleService",
                "UKHO.ADDS.Mocks.SampleService.csproj");

            var startInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                WorkingDirectory = repoRoot,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            startInfo.ArgumentList.Add("run");
            startInfo.ArgumentList.Add("--no-launch-profile");
            startInfo.ArgumentList.Add("--project");
            startInfo.ArgumentList.Add(sampleServiceProject);

            startInfo.Environment["ASPNETCORE_URLS"] = BaseAddress.ToString();
            startInfo.Environment["ASPNETCORE_ENVIRONMENT"] = "Development";
            startInfo.Environment["DOTNET_ENVIRONMENT"] = "Development";
            startInfo.Environment["OTEL_EXPORTER_OTLP_ENDPOINT"] = $"http://{IPAddress.Loopback}:4317";

            _process = Process.Start(startInfo)
                ?? throw new InvalidOperationException("Failed to start UKHO.ADDS.Mocks.SampleService.");

            _ = Task.Run(() => DrainAsync(_process.StandardOutput));
            _ = Task.Run(() => DrainAsync(_process.StandardError));

            await WaitUntilReadyAsync();
        }

        public async Task StopAsync()
        {
            if (_process is null)
            {
                return;
            }

            try
            {
                if (!_process.HasExited)
                {
                    _process.Kill(entireProcessTree: true);
                    await _process.WaitForExitAsync();
                }
            }
            finally
            {
                _process.Dispose();
            }
        }

        private async Task WaitUntilReadyAsync()
        {
            var timeoutAt = DateTime.UtcNow.AddSeconds(30);
            Exception? lastException = null;
            using var client = new HttpClient { BaseAddress = BaseAddress, Timeout = TimeSpan.FromSeconds(5) };

            while (DateTime.UtcNow < timeoutAt)
            {
                if (_process is { HasExited: true })
                {
                    lock (_processOutput)
                    {
                        throw new InvalidOperationException(
                            "Sample service exited before becoming ready."
                            + Environment.NewLine
                            + string.Join(Environment.NewLine, _processOutput));
                    }
                }

                try
                {
                    using var response = await client.GetAsync("/sample/files");

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        return;
                    }
                }
                catch (Exception ex)
                {
                    lastException = ex;
                }

                await Task.Delay(500);
            }

            lock (_processOutput)
            {
                throw new TimeoutException(
                    "Timed out waiting for sample service to start."
                    + Environment.NewLine
                    + (lastException?.Message ?? "No additional exception.")
                    + Environment.NewLine
                    + string.Join(Environment.NewLine, _processOutput));
            }
        }

        private async Task DrainAsync(StreamReader reader)
        {
            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();

                if (!string.IsNullOrWhiteSpace(line))
                {
                    lock (_processOutput)
                    {
                        _processOutput.Add(line);
                    }
                }
            }
        }

        private static int GetFreeTcpPort()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();

            try
            {
                return ((IPEndPoint)listener.LocalEndpoint).Port;
            }
            finally
            {
                listener.Stop();
            }
        }

        private static string FindRepoRoot()
        {
            var directory = new DirectoryInfo(AppContext.BaseDirectory);

            while (directory is not null)
            {
                if (File.Exists(Path.Combine(directory.FullName, "azure-pipelines.yml")))
                {
                    return directory.FullName;
                }

                directory = directory.Parent;
            }

            throw new DirectoryNotFoundException("Could not find repository root.");
        }
    }
}
