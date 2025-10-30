/*
4di Framework .NET MVVM Library
Copyright (c) 2025 by 4D Illusions. All rights reserved.
Released under the terms of the GNU General Public License version 3 or later.
*/

namespace FW4di.Dotnet.MVVM.Tests;

[TestClass]
public class RelayCommandTests
{
    private bool wasExecuted;
    private bool canExecute;

    [TestInitialize]
    public void Setup()
    {
        wasExecuted = false;
        canExecute = true;
    }

    [TestMethod]
    public void ExecuteShouldInvokeAction()
    {
        var command = new RelayCommand(_ => wasExecuted = true);

        command.Execute(null);

        Assert.IsTrue(wasExecuted, "Execute did not invoke the provided action.");
    }

    [TestMethod]
    public void CanExecuteShouldReturnTrueWhenNoCanExecuteProvided()
    {
        var command = new RelayCommand(_ => { });

        Assert.IsTrue(command.CanExecute(null), "CanExecute should return true when no canExecute function is provided.");
    }

    [TestMethod]
    public void CanExecuteShouldReturnFalseWhenCanExecuteIsFalse()
    {
        var command = new RelayCommand(_ => { }, _ => canExecute);

        canExecute = false;
        Assert.IsFalse(command.CanExecute(null), "CanExecute should return false when the provided function returns false.");
    }

    [TestMethod]
    public void RaiseCanExecuteChangedShouldTriggerCanExecuteChangedEvent()
    {
        var command = new RelayCommand(_ => { });
        bool eventTriggered = false;

        command.CanExecuteChanged += (s, e) => eventTriggered = true;
        command.RaiseCanExecuteChanged();

        Assert.IsTrue(eventTriggered, "RaiseCanExecuteChanged did not trigger the CanExecuteChanged event.");
    }
}
