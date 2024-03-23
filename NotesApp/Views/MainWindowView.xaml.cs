using System.Windows;
using System.Windows.Controls;
using NotesApp.Managers;
using NotesApp.ViewModels;

namespace NotesApp.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindowView : Window
    {
        public MainWindowView()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel(this);
            RestoreMainGridColumns();
            RestoreMainWindowSize();
        }

        private void RestoreMainWindowSize()
        {
            Width = SettingsManager.CurrentSettings.MainWindowWidth;
            Height = SettingsManager.CurrentSettings.MainWindowHeight;
            WindowState = SettingsManager.CurrentSettings.MainWindowIsMaximized
                ? WindowState.Maximized
                : WindowState.Normal;
        }

        private void RestoreMainGridColumns()
        {
            ColumnDefinition leftColumn = MainGrid.ColumnDefinitions[0];
            leftColumn.Width = new GridLength(
                SettingsManager.CurrentSettings.MainWindowLeftColumnWidth,
                GridUnitType.Star);
            leftColumn.MinWidth = 100;

            ColumnDefinition splitterColumn = MainGrid.ColumnDefinitions[1];
            splitterColumn.Width = new GridLength(1, GridUnitType.Auto);

            ColumnDefinition rightColumn = MainGrid.ColumnDefinitions[2];
            rightColumn.Width = new GridLength(
                SettingsManager.CurrentSettings.MainWindowRightColumnWidth,
                GridUnitType.Star);
            rightColumn.MinWidth = 100;
        }

        private void GridSplitter_DragCompleted(
            object sender,
            System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            SettingsManager.CurrentSettings.MainWindowLeftColumnWidth =
                MainGrid.ColumnDefinitions[0].Width.Value;

            SettingsManager.CurrentSettings.MainWindowRightColumnWidth =
                MainGrid.ColumnDefinitions[2].Width.Value;
        }

        private void OnMainWindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            SettingsManager.CurrentSettings.MainWindowWidth = Width;
            SettingsManager.CurrentSettings.MainWindowHeight = Height;
        }

        private void OnMainWindowStateChanged(object sender, EventArgs e)
        {
            SettingsManager.CurrentSettings.MainWindowIsMaximized =
                WindowState == WindowState.Maximized;
        }
    }
}