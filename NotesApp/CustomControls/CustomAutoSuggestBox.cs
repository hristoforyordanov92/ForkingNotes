using System.Windows;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;

namespace NotesApp.CustomControls
{
    public class CustomAutoSuggestBox : AutoSuggestBox
    {
        public static readonly DependencyProperty ReturnCommandProperty =
            DependencyProperty.Register(
                "ReturnCommand",
                typeof(ICommand),
                typeof(CustomAutoSuggestBox),
                new PropertyMetadata(default(ICommand)));

        public ICommand ReturnCommand
        {
            get => (ICommand)GetValue(ReturnCommandProperty);
            set => SetValue(ReturnCommandProperty, value);
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            // todo: this kinda works, but not really... holding left mouse button will open the suggestion box,
            // but releasing it closes it. maybe it's another thing to hack, but gotta check the AutoSuggestBox code first
            IsSuggestionOpen = true;
            e.Handled = true;
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            // HACK! Trying to sneakily override the default behaviour of the AutoSuggestBox by
            // having a custom command which executes when we've already selected a value

            // todo: maybe use the event for the value selection instead of this crazy 'if' statement??
            if (!IsSuggestionOpen && !string.IsNullOrWhiteSpace(Text) && e.Key == Key.Enter)
            {
                ReturnCommand?.Execute(null);
                e.Handled = true;
            }
            else
            {
                base.OnPreviewKeyDown(e);
            }
        }
    }
}
