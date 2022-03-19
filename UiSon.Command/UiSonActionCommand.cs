// UiSon, by Cameron Gale 2021

using System;
using System.Windows.Input;

namespace UiSon.Command
{
    /// <summary>
    /// ommand that preforms an action
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

        /// <summary>
        /// Executes the command's action
        /// </summary>
        /// <param name="parameter">parameter for action</param>
        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        /// <summary>
        /// Checks if parameter can be used for Execute
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            return (_canExecute == null) || _canExecute(parameter);
        }
    }
}
