using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SLPReportAdminApp
{
    public class RealayCommand : ICommand
    {
        Action<object?> execute;
        Func<object?, bool> canExecute;

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }

        }
        public RealayCommand(Action<object?> execute, Func<object?, bool> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }   

        public bool CanExecute(object? parameter)
        {
            return canExecute == null || CanExecute(parameter);
        }

        public void Execute(object? parameter)
        {
            execute(parameter);
        }
    }
}
