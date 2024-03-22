using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WPFBasics
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        // This method is called by the Set accessor of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        protected void SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
        {
            field = value;
            RaisePropertyChanged(propertyName);
        }

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
