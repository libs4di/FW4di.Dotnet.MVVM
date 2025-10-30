/*
4di Framework .NET MVVM Library
Copyright (c) 2025 by 4D Illusions. All rights reserved.
Released under the terms of the GNU General Public License version 3 or later.
*/

namespace FW4di.Dotnet.MVVM.Tests;

[TestClass]
public class RelayCommandGenericTests
{
    private int executedValue;
    private bool canExecute;

    [TestInitialize]
    public void Setup()
    {
        executedValue = 0;
        canExecute = true;
    }

    [TestMethod]
    public void ExecuteShouldInvokeActionWithParameter()
    {
        var command = new RelayCommand<int>(value => executedValue = value);

        command.Execute(10);

        Assert.AreEqual(10, executedValue, "Execute did not pass the correct parameter to the action.");
    }

    [TestMethod]
    public void CanExecuteShouldReturnTrueWhenNoCanExecuteProvided()
    {
        var command = new RelayCommand<int>(_ => { });

        Assert.IsTrue(command.CanExecute(5), "CanExecute should return true when no canExecute function is provided.");
    }

    [TestMethod]
    public void CanExecuteShouldReturnFalseWhenCanExecuteIsFalse()
    {
        var command = new RelayCommand<int>(_ => { }, _ => canExecute);

        canExecute = false;
        Assert.IsFalse(command.CanExecute(5), "CanExecute should return false when the provided function returns false.");
    }

    [TestMethod]
    public void RaiseCanExecuteChangedShouldTriggerCanExecuteChangedEvent()
    {
        var command = new RelayCommand<int>(_ => { });
        bool eventTriggered = false;

        command.CanExecuteChanged += (s, e) => eventTriggered = true;
        command.RaiseCanExecuteChanged();

        Assert.IsTrue(eventTriggered, "RaiseCanExecuteChanged did not trigger the CanExecuteChanged event.");
    }
}
