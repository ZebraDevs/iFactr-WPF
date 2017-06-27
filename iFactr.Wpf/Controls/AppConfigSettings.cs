using System.Configuration;
using System.Linq;
using iFactr.Core.Targets.Settings;

namespace iFactr.Wpf
{
    class AppConfigSettings : SettingsDictionary
    {
        public AppConfigSettings()
        {
            Load();
        }

        public override sealed void Load()
        {
            foreach (var key in ConfigurationManager.AppSettings.AllKeys)
            {
                Add(key, ConfigurationManager.AppSettings[key]);
            }
        }

        public override void Store()
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            var allKeys = ConfigurationManager.AppSettings.AllKeys;
            foreach (var key in Keys)
            {
                if (allKeys.Contains(key))
                {
                    config.AppSettings.Settings[key].Value = this[key];
                }
                else
                {
                    config.AppSettings.Settings.Add(key, this[key]);
                }
            }
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }
    }
}