using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MaterialDesignThemes.Wpf;

namespace NotesApp.Managers
{
    public static class ThemeManager
    {
        private static readonly PaletteHelper _paletteHelper = new();

        public static void ApplyBaseTheme(BaseTheme baseTheme)
        {
            Theme currentTheme = _paletteHelper.GetTheme();
            currentTheme.SetBaseTheme(baseTheme);
            _paletteHelper.SetTheme(currentTheme);
        }

        // todo: maybe allow users to change their colors? should be ezpz anyway
        //public static void SetPrimaryColor(string color)
        //{
        //    Theme currentTheme = _paletteHelper.GetTheme();
        //    currentTheme.SetPrimaryColor(System.Windows.Media.Color.FromRgb());
        //    _paletteHelper.SetTheme(currentTheme);
        //}
    }
}
