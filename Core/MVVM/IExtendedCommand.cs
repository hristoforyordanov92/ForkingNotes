using System.Windows.Input;

namespace Core.MVVM
{
    public interface IExtendedCommand : ICommand
    {
        void RaiseCanExecuteChanged();
    }
}
