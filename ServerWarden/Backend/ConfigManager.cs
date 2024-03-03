using System.Collections.Specialized;
using System.Configuration;

namespace Backend;

public static class ConfigManager {
  private static readonly NameValueCollection AppSettings = ConfigurationManager.AppSettings;

  public static string ReadSetting(string key) {
    string result;
      
    try {
      result = AppSettings[key] ?? string.Empty;
    } catch (ConfigurationErrorsException) {
      Console.WriteLine("Error reading app settings");
      result = string.Empty;
    }
      
    return result;
  }

  public static void AddUpdateAppSetting(string key, string value) {
    try {
      var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
      var settings = configFile.AppSettings.Settings;

      if (settings[key] == null) {
        settings.Add(key, value);
      } else {
        settings[key].Value = value;
      }
        
      configFile.Save(ConfigurationSaveMode.Modified);
      ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
    } catch (ConfigurationErrorsException) {
      Console.WriteLine("Error writing app settings");
    }
  }
}