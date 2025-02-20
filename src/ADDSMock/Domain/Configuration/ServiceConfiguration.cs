namespace ADDSMock.Domain.Configuration
{
    public class ServiceConfiguration
    {
        public ServiceConfiguration(string prefix, string name)
        {
            Prefix = prefix;
            Name = name;
        }

        public string Name { get; }

        public string Prefix { get; }
    }
}
