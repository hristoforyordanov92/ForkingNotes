using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;

namespace NotesApp.Configuration
{
    [JsonObject(memberSerialization: MemberSerialization.OptIn)]
    public class Settings
    {
        private readonly PaletteHelper _paletteHelper = new();

        [JsonProperty("ApplicationTheme")]
        private BaseTheme _theme;

        public event Action? SettingsChanged;

        public BaseTheme Theme
        {
            get => _theme;
            set
            {
                _theme = value;
                ApplyBaseTheme(_theme);
                SettingsChanged?.Invoke();
            }
        }

        public void ApplySettings()
        {
            ApplyBaseTheme(Theme);
        }

        private void ApplyBaseTheme(BaseTheme baseTheme)
        {
            Theme currentTheme = _paletteHelper.GetTheme();
            currentTheme.SetBaseTheme(baseTheme);
            _paletteHelper.SetTheme(currentTheme);
        }
    }
}
