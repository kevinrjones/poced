using System.Configuration;

namespace Configuration
{
    /// <summary>
    /// Wrapper around configurations so we can test the code more easily
    /// </summary>
    public class AppConfiguration : IConfiguration
    {
        public string GetConfigurationValue(string name)
        {
            return ConfigurationManager.AppSettings[name];
        }

    }
}