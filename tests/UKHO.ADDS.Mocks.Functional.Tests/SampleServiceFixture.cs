using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace UKHO.ADDS.Mocks.Functional.Tests
{
    internal sealed class SampleServiceFixture
    {
        public Uri BaseAddress { get; private set; } = null!;

        private Process? _process;

        public async Task StartAsync()
        {
            var port = GetAvailablePort();
            BaseAddress = new Uri($"http://127.0.0.1:{port}");

            var serviceDllPath = Path.Combine(AppContext.BaseDirectory, "UKHO.ADDS.Mocks.SampleService.dll");
            if (!File.Exists(serviceDllPath))
            {
                throw new FileNotFoundException("Could not locate sample service assembly.", serviceDllPath);
            }

            var startInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"\"{serviceDllPath}\" --urls {BaseAddress}",
                WorkingDirectory = Path.GetDirectoryName(serviceDllPath)!,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            startInfo.Environment["ASPNETCORE_URLS"] = BaseAddress.ToString();
            startInfo.Environment["OTEL_EXPORTER_OTLP_ENDPOINT"] = "http://127.0.0.1:4318";

            _process = Process.Start(startInfo) ?? throw new InvalidOperationException("Failed to start sample service process.");

            await WaitUntilReadyAsync(TimeSpan.FromSeconds(60));
        }

        public async Task StopAsync()
        {
            if (_process == null)
            {
                return;
            }

            try
            {
                if (!_process.HasExited)
                {
                    _process.Kill(entireProcessTree: true);
                }
            }
            finally
            {
                _process.Dispose();
                _process = null;
            }

            await Task.CompletedTask;
        }

        private async Task WaitUntilReadyAsync(TimeSpan timeout)
        {
            using var cts = new CancellationTokenSource(timeout);
            using var client = new HttpClient { BaseAddress = BaseAddress };

            while (!cts.IsCancellationRequested)
            {
                try
                {
                    using var response = await client.GetAsync("/sample/files", cts.Token);
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        return;
                    }
                }
                catch (HttpRequestException)
                {
                }
                catch (TaskCanceledException) when (!cts.IsCancellationRequested)
                {
                }

                await Task.Delay(TimeSpan.FromMilliseconds(250), cts.Token);
            }

            var output = _process is null ? string.Empty : await _process.StandardOutput.ReadToEndAsync();
            var error = _process is null ? string.Empty : await _process.StandardError.ReadToEndAsync();
            throw new TimeoutException($"Sample service did not become ready within {timeout}.\nSTDOUT:\n{output}\nSTDERR:\n{error}");
        }

        private static int GetAvailablePort()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }
    }
}
