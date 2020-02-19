using System;
using System.Windows.Input;

namespace PowerCenter.UWP
{
    public class Command : ICommand
    {
        private Action<object> _action;
        private Func<object, bool> _canExecute;

        public Command(Action action)
        {
            _action = (o) => action();
            _canExecute = (o) => true;
        }

        public Command(Action<object> action)
        {
            _action = action;
            _canExecute = (o) => true;
        }

        public Command(Action action, Func<bool> canExecute)
        {
            _action = (o) => action();
            _canExecute = (o) => canExecute();
        }

        public Command(Action<object> action, Func<bool> canExecute)
        {
            _action = action;
            _canExecute = (o) => canExecute();
        }

        public Command(Action action, Func<object, bool> canExecute)
        {
            _action = (o) => action();
            _canExecute = canExecute;
        }

        public Command(Action<object> action, Func<object, bool> canExecute)
        {
            _action = action;
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _action(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
