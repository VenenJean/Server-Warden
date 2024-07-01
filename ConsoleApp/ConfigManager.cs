using System.Collections.Specialized;
using System.Configuration;

namespace ConsoleApp;

public class ConfigManager {
  private readonly NameValueCollection _appSettings = ConfigurationManager.AppSettings;

  // Isn't required for use
  private void ReadAllSettings() {  
    try {
      if (_appSettings.Count == 0) {
        Console.WriteLine("AppSettings is empty.");
      } else {
        foreach (var key in _appSettings.AllKeys) {
          Console.WriteLine("Key: {0} Value: {1}", key, _appSettings[key]);  
        }
      }
    } catch (ConfigurationErrorsException) {  
      Console.WriteLine("Error reading app settings");  
    }  
  }
  
  public string ReadConnectionString() {
    try {
      var connectionString =
        $"server={ReadSetting("DbHost")};" +
        $"port={ulong.Parse(ReadSetting("DbPort"))};" +
        $"user={ReadSetting("DbUser")};" +
        $"password={ReadSetting("DbUserPass")};" +
        $"database={ReadSetting("DbName")};";

      Console.WriteLine(connectionString);

      return connectionString;
    } catch (ConfigurationErrorsException ex) {
      Console.WriteLine($"{ex.GetType().Name}: {ex.Message}\nInner exception: {(ex.InnerException == null ? "hey" : $"{ex.InnerException.GetType().Name}: {ex.InnerException.Message}")}");
    }

    return string.Empty;
  }

  public string ReadSetting(string key) {
    string result;
      
    try {
      result = _appSettings[key] ?? string.Empty;
    } catch (ConfigurationErrorsException) {
      Console.WriteLine("Error reading app settings");
      result = string.Empty;
    }
      
    return result;
  }

  public void AddUpdateAppSetting(string key, string value) {
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