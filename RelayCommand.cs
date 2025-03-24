using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace xamlPianoRoll;

public class RelayCommand : ICommand
{
	private readonly Func<object, Task> execute;
	private readonly Predicate<object> canExecute;

	public RelayCommand(Func<object, Task> execute, Predicate<object> canExecute = null)
	{
		this.execute = execute;
		this.canExecute = canExecute;
	}

	public bool CanExecute(object parameter) => canExecute == null || canExecute(parameter);

	public event EventHandler CanExecuteChanged
	{
		add { CommandManager.RequerySuggested += value; }
		remove { CommandManager.RequerySuggested -= value; }
	}

	public async void Execute(object parameter)
	{
		await execute(parameter);
	}
}

