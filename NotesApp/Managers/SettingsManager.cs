using System.IO;
using Newtonsoft.Json;
using NotesApp.Configuration;
using NotesApp.Utils;

namespace NotesApp.Managers
{
    public static class SettingsManager
    {
        private static bool _isInitialized = false;

        public static Settings CurrentSettings { get; private set; } = new Settings();

        static SettingsManager()
        {
            LoadApplicationSettings();
        }

        public static void LoadApplicationSettings()
        {
            if (_isInitialized)
                return;

            LoadSettingsFile();

            _isInitialized = true;
        }

        private static void LoadSettingsFile()
        {
            CurrentSettings.SettingsChanged -= OnSettingsChanged;

            if (!File.Exists(PathHelper.SettingsFilePath))
            {
                CurrentSettings = new Settings();
            }
            else
            {
                string settingsJson = File.ReadAllText(PathHelper.SettingsFilePath);
                // todo: is it possible to be null?
                CurrentSettings = JsonConvert.DeserializeObject<Settings>(settingsJson) ?? new Settings();
            }

            SaveSettings();
            CurrentSettings.ApplySettings();
            CurrentSettings.SettingsChanged += OnSettingsChanged;
        }

        private static void SaveSettings()
        {
            string settingsJson = JsonConvert.SerializeObject(
                CurrentSettings,
                Formatting.Indented);

            File.WriteAllText(PathHelper.SettingsFilePath, settingsJson);
        }

        private static void OnSettingsChanged()
        {
            SaveSettings();
        }
    }
}
