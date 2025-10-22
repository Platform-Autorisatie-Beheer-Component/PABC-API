namespace PABC.Server.Helper
{
    public class ConfigHelper
    {
        public static string GetRequiredConfigValue(IConfiguration configuration, string key)
        {
            var value = configuration[key];
            if (string.IsNullOrEmpty(value))
            {
                throw new InvalidOperationException($"Missing required configuration value for '{key}'");
            }
            return value;
        }
    }
}
