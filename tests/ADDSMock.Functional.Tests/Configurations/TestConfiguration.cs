using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace ADDSMock.Functional.Tests.Configurations
{
    internal class TestConfiguration
    {
        public string AddsMockExePath { get; }

        public TestConfiguration()
        {
            var configurationRoot = new ConfigurationBuilder().AddJsonFile("appsettings.json", false).Build() ?? throw new NullReferenceException("Unable to build configuration from appsettings.json");
            AddsMockExePath = Path.GetFullPath(GetString("AddsMockExePath"));

            if (!Path.Exists(AddsMockExePath))
            {
                throw new FileNotFoundException($"Can't find {AddsMockExePath}", AddsMockExePath);
            }

            string GetString(string lookup) => configurationRoot.GetValue<string>(lookup) ?? throw new NullReferenceException($"Unable to find {lookup} in appsettings.json");
            //int GetInt(string lookup) => configurationRoot.GetValue<int>(lookup);
        }
    }
}
