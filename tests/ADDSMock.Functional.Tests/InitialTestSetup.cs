using System.Diagnostics;
using ADDSMock.Functional.Tests.Configurations;
using NUnit.Framework;

namespace ADDSMock.Functional.Tests
{
    [SetUpFixture]
    internal class InitialTestSetup
    {
        private Process? _process;

        [OneTimeSetUp]
        public void FunctionalTestsSetup()
        {
            var configuration = new TestConfiguration();
            _process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = configuration.AddsMockExePath,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            _process.Start();
        }

        [OneTimeTearDown]
        public void FunctionalTestsTearDown()
        {
            _process?.Kill();
            _process?.Dispose();
        }
    }
}
