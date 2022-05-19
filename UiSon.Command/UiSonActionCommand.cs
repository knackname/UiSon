// UiSon, by Cameron Gale 2021

using System;
using System.Windows.Input;

namespace UiSon.Command
{
    /// <summary>
    /// Command that preforms an action
    /// </summary>
    public class UiSonActionCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="execute">Action to execute</param>
        /// <param name="canExecute">Function to validate execution</param>
        public UiSonActionCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        /// <inheritdoc cref="ICommand.Execute(object?)" />
        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        /// <inheritdoc cref="ICommand.CanExecute(object?)" />
        public bool CanExecute(object parameter)
        {
            return (_canExecute == null) || _canExecute(parameter);
        }
    }
}
