using System.Configuration;

namespace Poced.Configuration
{
    /// <summary>
    /// Wrapper around configurations so we can test the code more easily
    /// </summary>
    public class AppConfiguration : IConfiguration
    {
        public string GetConfigurationValue(string name)
        {
            //todo: replace with BP code
            return "";
            //return ConfigurationManager.AppSettings[name];
        }

    }
}