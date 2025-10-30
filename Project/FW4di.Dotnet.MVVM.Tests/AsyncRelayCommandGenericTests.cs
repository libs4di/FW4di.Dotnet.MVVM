/*
4di Framework .NET MVVM Library
Copyright (c) 2025 by 4D Illusions. All rights reserved.
Released under the terms of the GNU General Public License version 3 or later.
*/

namespace FW4di.Dotnet.MVVM.Tests;

[TestClass]
public class AsyncRelayCommandGenericTests
{
    [TestMethod]
    public async Task ExecuteAsyncShouldInvokeExecuteDelegateWithParameter()
    {
        int receivedValue = 0;
        var command = new AsyncRelayCommand<int>(async (value) =>
        {
            receivedValue = value;
            await Task.CompletedTask;
        });

        await command.ExecuteAsync(42);

        Assert.AreEqual(42, receivedValue);
    }

    [TestMethod]
    public void CanExecuteShouldRespectCanExecuteDelegateWithParameter()
    {
        bool canExecuteState = false;
        var command = new AsyncRelayCommand<int>(
            async (param) => await Task.CompletedTask,
            (param) => canExecuteState && param > 0
        );

        Assert.IsFalse(command.CanExecute(1)); // canExecuteState is false
        canExecuteState = true;
        Assert.IsTrue(command.CanExecute(1)); // Now should be true
        Assert.IsFalse(command.CanExecute(-1)); // Should be false due to param condition
    }

    [TestMethod]
    public async Task ExecuteAsyncShouldHandleExceptionsWithParameter()
    {
        var command = new AsyncRelayCommand<int>(async (param) =>
        {
            await Task.CompletedTask;
            throw new InvalidOperationException("Test Exception");
        });

        var exception = await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => command.ExecuteAsync(5));

        Assert.AreEqual("Test Exception", exception.Message);
    }

    [TestMethod]
    public async Task ExecuteAsyncShouldRaiseCanExecuteChangedWithParameter()
    {
        int eventCount = 0;
        var command = new AsyncRelayCommand<int>(async (param) => await Task.Delay(100));
        command.CanExecuteChanged += (s, e) => eventCount++;

        await command.ExecuteAsync(5);

        Assert.IsTrue(eventCount >= 2); // Raised at start and end
    }
}
