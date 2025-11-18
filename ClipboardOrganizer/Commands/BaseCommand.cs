using System;
using System.Windows.Input;

namespace ClipboardOrganizer
{
    public class BaseCommand : ICommand
    {
        private Action _action;

        private Func<bool> _canExecute;
        private ICommand syncCommand;
        private object canSyncCommand;
        private ICommand setAsNotAMatch;
        private Func<bool> canSetAsNotAMatch;
        private ICommand setSearchTypeToViewList;
        private object canSetSearchTypeToViewList;

        /// <summary>
        /// Creates instance of the command handler
        /// </summary>
        /// <param name="action">Action to be executed by the command</param>
        /// <param name="canExecute">A bolean property to containing current permissions to execute the command</param>
        public BaseCommand(Action action, Func<bool> canExecute)
        {
            _action = action;
            _canExecute = canExecute;
        }

        public BaseCommand(ICommand setAsNotAMatch, Func<bool> canSetAsNotAMatch)
        {
            this.setAsNotAMatch = setAsNotAMatch;
            this.canSetAsNotAMatch = canSetAsNotAMatch;
        }

        public BaseCommand(ICommand setSearchTypeToViewList, object canSetSearchTypeToViewList)
        {
            this.setSearchTypeToViewList = setSearchTypeToViewList;
            this.canSetSearchTypeToViewList = canSetSearchTypeToViewList;
        }

        /// <summary>
        /// Wires CanExecuteChanged event
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Forcess checking if execute is allowed
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            return _canExecute.Invoke();
        }

        public void Execute(object parameter)
        {
            _action();
        }
    }
}