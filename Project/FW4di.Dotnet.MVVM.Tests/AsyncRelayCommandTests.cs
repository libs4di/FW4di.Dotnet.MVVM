/*
4di Framework .NET MVVM Library
Copyright (c) 2025 by 4D Illusions. All rights reserved.
Released under the terms of the GNU General Public License version 3 or later.
*/

using FW4di.Dotnet.MVVM;

[TestClass]
public class AsyncRelayCommandTests
{
    private bool canExecuteValue;
    private bool commandExecuted;
    private AsyncRelayCommand command;

    [TestInitialize]
    public void Setup()
    {
        canExecuteValue = true;
        commandExecuted = false;
        command = new AsyncRelayCommand(
            async () => {
                await Task.Delay(100);
                commandExecuted = true;
            },
            () => canExecuteValue
        );
    }

    [TestMethod]
    public void ConstructorShouldThrowExceptionWhenExecuteIsNull()
    {
        Assert.ThrowsException<ArgumentNullException>(() => new AsyncRelayCommand(null));
    }

    [TestMethod]
    public void CanExecuteShouldReturnTrueWhenNotExecutingAndCanExecuteIsNull()
    {
        var cmd = new AsyncRelayCommand(async () => await Task.CompletedTask);
        Assert.IsTrue(cmd.CanExecute(null));
    }

    [TestMethod]
    public void CanExecuteShouldReturnFalseWhenExecuting()
    {
        canExecuteValue = true;
        Assert.IsTrue(command.CanExecute(null));

        typeof(AsyncRelayCommand)
            .GetField("isExecuting", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(command, true);

        Assert.IsFalse(command.CanExecute(null));
    }

    [TestMethod]
    public void CanExecuteShouldRespectCanExecuteDelegate()
    {
        canExecuteValue = false;
        Assert.IsFalse(command.CanExecute(null));

        canExecuteValue = true;
        Assert.IsTrue(command.CanExecute(null));
    }

    [TestMethod]
    public async Task ExecuteShouldRunExecuteDelegate()
    {
        Assert.IsFalse(commandExecuted);
        await Task.Run(() => command.Execute(null));
        await Task.Delay(200);
        Assert.IsTrue(commandExecuted);
    }

    [TestMethod]
    public async Task ExecuteShouldDisableCanExecuteWhileRunning()
    {
        bool canExecuteChangedRaised = false;
        command.CanExecuteChanged += (s, e) => canExecuteChangedRaised = true;

        Assert.IsTrue(command.CanExecute(null));

        var executeTask = Task.Run(() => command.Execute(null));
        await Task.Delay(50);

        Assert.IsFalse(command.CanExecute(null), "CanExecute should be false while command is executing.");
        Assert.IsTrue(canExecuteChangedRaised, "CanExecuteChanged event should have been raised.");

        await executeTask;
        await Task.Delay(50);

        Assert.IsTrue(command.CanExecute(null), "CanExecute should be true again after execution.");
    }

    [TestMethod]
    public void RaiseCanExecuteChangedShouldInvokeEvent()
    {
        bool eventRaised = false;
        command.CanExecuteChanged += (s, e) => eventRaised = true;

        command.RaiseCanExecuteChanged();
        Assert.IsTrue(eventRaised);
    }
}
