/*
4di Framework .NET MVVM Library
Copyright (c) 2025 by 4D Illusions. All rights reserved.
Released under the terms of the GNU General Public License version 3 or later.
*/

namespace FW4di.Dotnet.MVVM;

// <summary>
/// A basic implementation of the <see cref="ICommand"/> interface for non-generic actions.
/// Provides a simple way to bind UI interactions (such as buttons) to logic in ViewModels.
/// </summary>
public class RelayCommand : ICommand
{
    private readonly Action<object> execute;
    private readonly Func<object, bool> canExecute;
    private event EventHandler canExecuteChanged;

    public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
    {
        this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
        this.canExecute = canExecute;
    }

    public bool CanExecute(object parameter) => canExecute?.Invoke(parameter) ?? true;

    public void Execute(object parameter) => execute(parameter);

    public event EventHandler CanExecuteChanged
    {
        add { canExecuteChanged += value; }
        remove { canExecuteChanged -= value; }
    }

    public void RaiseCanExecuteChanged()
    {
        canExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}

public class RelayCommand<T> : ICommand
{
    private readonly Action<T> execute;
    private readonly Func<T, bool> canExecute;
    private event EventHandler canExecuteChanged;

    public RelayCommand(Action<T> execute, Func<T, bool> canExecute = null)
    {
        this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
        this.canExecute = canExecute;
    }

    public bool CanExecute(object parameter)
    {
        return canExecute == null || (parameter is T t && canExecute(t));
    }

    public void Execute(object parameter)
    {
        if (parameter is T t)
            execute(t);
    }

    public event EventHandler CanExecuteChanged
    {
        add { canExecuteChanged += value; }
        remove { canExecuteChanged -= value; }
    }

    public void RaiseCanExecuteChanged()
    {
        canExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
