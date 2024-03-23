using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using NotesApp.Configuration;
using NotesApp.Utils;

namespace NotesApp.Managers
{
    public static class SettingsManager
    {
        private static readonly object _settingsLock = new();

        private static Settings? _currentSettings;
        /// <summary>
        /// The current settings for the application.
        /// </summary>
        public static Settings CurrentSettings
        {
            get
            {
                if (_currentSettings == null)
                {
                    lock (_settingsLock)
                    {
                        if (_currentSettings == null)
                        {
                            _currentSettings = GetSettings();
                            SaveSettings(_currentSettings);
                            _currentSettings.ApplySettings();
                            _currentSettings.SettingsChanged += OnSettingsChanged;
                        }
                    }
                }

                return _currentSettings;
            }
        }

        /// <summary>
        /// Force-loads the settings for the application.
        /// </summary>
        /// <exception cref="Exception">Thrown when the settings could not be loaded and ended up as null.</exception>
        public static void LoadApplicationSettings()
        {
            // todo: a bit of a fake way to force the settings to apply to the app, but it will do for now
            if (CurrentSettings == null)
                throw new Exception($"{nameof(CurrentSettings)} could not be loaded properly.");
        }

        private static Settings GetSettings()
        {
            Settings settings;

            if (!File.Exists(PathHelper.SettingsFilePath))
            {
                settings = new Settings();
            }
            else
            {
                string settingsJson = File.ReadAllText(PathHelper.SettingsFilePath);
                // DeserializeObject could return null when the file is empty
                settings = JsonConvert.DeserializeObject<Settings>(settingsJson) ?? new Settings();
            }

            return settings;
        }

        private static void SaveSettings(Settings settings)
        {
            string settingsJson = JsonConvert.SerializeObject(
                settings,
                Formatting.Indented);

            File.WriteAllText(PathHelper.SettingsFilePath, settingsJson);
        }

        private static void OnSettingsChanged()
        {
            SaveSettings(CurrentSettings);
        }
    }
}
