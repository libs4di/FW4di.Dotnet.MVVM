/*
4di Framework .NET MVVM Library
Copyright (c) 2025 by 4D Illusions. All rights reserved.
Released under the terms of the GNU General Public License version 3 or later.
*/

namespace FW4di.Dotnet.MVVM;

/// <summary>
/// Represents an asynchronous implementation of the <see cref="ICommand"/> interface.
/// Allows the execution of <see cref="Task"/>-based actions while automatically 
/// managing the command’s execution state to prevent re-entry.
/// </summary>
public class AsyncRelayCommand : ICommand
{
    private readonly Func<Task> execute;
    private readonly Func<bool> canExecute;
    private bool isExecuting;

    public AsyncRelayCommand(Func<Task> execute, Func<bool> canExecute = null)
    {
        this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
        this.canExecute = canExecute;
    }

    public bool CanExecute(object parameter) => !isExecuting && (canExecute?.Invoke() ?? true);

    public async Task ExecuteAsync()
    {
        if (!CanExecute(null)) return;

        isExecuting = true;
        RaiseCanExecuteChanged();

        try
        {
            await execute();
        }
        finally
        {
            isExecuting = false;
            RaiseCanExecuteChanged();
        }
    }

    public void Execute(object parameter)
    {
        _ = ExecuteAsync();
    }

    public event EventHandler CanExecuteChanged;

    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}

public class AsyncRelayCommand<T> : ICommand
{
    private readonly Func<T, Task> execute;
    private readonly Func<T, bool> canExecute;
    private bool isExecuting;

    public AsyncRelayCommand(Func<T, Task> execute, Func<T, bool> canExecute = null)
    {
        this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
        this.canExecute = canExecute;
    }

    public bool CanExecute(object parameter)
    {
        return !isExecuting && (canExecute?.Invoke((T)parameter) ?? true);
    }

    public async void Execute(object parameter)
    {
        await ExecuteAsync((T)parameter);
    }

    public async Task ExecuteAsync(T parameter)
    {
        if (!CanExecute(parameter)) return;

        isExecuting = true;
        RaiseCanExecuteChanged();

        try
        {
            await execute(parameter);
        }
        finally
        {
            isExecuting = false;
            RaiseCanExecuteChanged();
        }
    }

    public event EventHandler CanExecuteChanged;

    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
