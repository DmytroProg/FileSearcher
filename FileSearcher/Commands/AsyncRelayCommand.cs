using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSearcher.Commands
{
    public class AsyncRelayCommand : AsyncCommandBase
    {
        private readonly Func<Task> _executeAsync;
        private readonly Func<bool> _canExecute;

        public AsyncRelayCommand(Func<Task> execute) : this(execute, () => true)
        {
        }

        public AsyncRelayCommand(Func<Task> execute, Func<bool> canExecute)
        {
            _executeAsync = execute;
            _canExecute = canExecute;
        }

        public override async Task ExecuteAsync(object? parameter)
        {
            await _executeAsync.Invoke();
        }

        public override bool CanExecute(object? parameter)
        {
            return _canExecute.Invoke();
        }
    }
}
