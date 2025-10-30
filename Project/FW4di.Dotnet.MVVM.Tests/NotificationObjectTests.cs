/*
4di Framework .NET MVVM Library
Copyright (c) 2025 by 4D Illusions. All rights reserved.
Released under the terms of the GNU General Public License version 3 or later.
*/

using FW4di.Dotnet.MVVM.Tests.Helpers;

namespace FW4di.Dotnet.MVVM.Tests;

[TestClass]
public class NotificationObjectTests
{
    [TestMethod]
    public void RaisePropertyChangedShouldTriggerEventWithSingleProperty()
    {
        var viewModel = new TestViewModel();
        bool eventRaised = false;
        viewModel.PropertyChanged += (sender, e) =>
        {
            if (e.PropertyName == "Name")
                eventRaised = true;
        };

        viewModel.RaisePropertyChanged("Name");

        Assert.IsTrue(eventRaised, "PropertyChanged event was not raised for 'Name'.");
    }

    [TestMethod]
    public void RaisePropertyChangedShouldTriggerEventWithMultipleProperties()
    {
        var viewModel = new TestViewModel();
        var changedProperties = new HashSet<string>();
        viewModel.PropertyChanged += (sender, e) => changedProperties.Add(e.PropertyName);

        viewModel.RaisePropertyChanged("Name", "OtherProperty");

        Assert.IsTrue(changedProperties.Contains("Name"), "PropertyChanged event was not raised for 'Name'.");
        Assert.IsTrue(changedProperties.Contains("OtherProperty"), "PropertyChanged event was not raised for 'OtherProperty'.");
    }

    [TestMethod]
    public void SetPropertyShouldUpdateFieldAndRaiseEvent()
    {
        var viewModel = new TestViewModel();
        bool eventRaised = false;

        viewModel.PropertyChanged += (sender, e) =>
        {
            if (e.PropertyName == nameof(TestViewModel.Name))
            {
                eventRaised = true;
            }
        };

        viewModel.Name = "New Value";

        Assert.AreEqual("New Value", viewModel.Name, "Property was not updated correctly.");
        Assert.IsTrue(eventRaised, "PropertyChanged event was not raised.");
    }

    [TestMethod]
    public void SetPropertyShouldNotRaiseEventIfValueDoesNotChange()
    {
        var viewModel = new TestViewModel { Name = "Initial" };
        bool eventRaised = false;

        viewModel.PropertyChanged += (sender, e) => eventRaised = true;

        viewModel.Name = "Initial";

        Assert.IsFalse(eventRaised, "PropertyChanged event should not be raised if the value does not change.");
    }

    // -------------------- ✅ VALIDATION Tests --------------------
    [TestMethod]
    public void NameValidationShouldShowErrorWhenEmpty()
    {
        var viewModel = new TestViewModel();

        viewModel.Name = "";

        Assert.IsTrue(viewModel.HasErrors, "HasErrors should be true for invalid Name.");
        Assert.IsTrue(viewModel.GetErrors(nameof(TestViewModel.Name)).Cast<string>().Contains("Name cannot be empty."),
            "Expected validation error for empty Name.");
    }

    [TestMethod]
    public void NameValidationShouldClearErrorWhenValid()
    {
        var viewModel = new TestViewModel();

        viewModel.Name = "";
        Assert.IsTrue(viewModel.HasErrors, "Should have errors for empty Name.");

        viewModel.Name = "Valid Name";

        Assert.IsFalse(viewModel.HasErrors, "Errors should be cleared when Name is valid.");
    }

    [TestMethod]
    public void AgeValidationShouldShowErrorWhenNegative()
    {
        var viewModel = new TestViewModel();

        viewModel.Age = -1;

        Assert.IsTrue(viewModel.HasErrors, "HasErrors should be true for invalid Age.");
        Assert.IsTrue(viewModel.GetErrors(nameof(TestViewModel.Age)).Cast<string>().Contains("Age cannot be negative or zero."),
            "Expected validation error for negative Age.");
    }

    [TestMethod]
    public void AgeValidationShouldClearErrorWhenValid()
    {
        var viewModel = new TestViewModel();

        viewModel.Age = -1;
        Assert.IsTrue(viewModel.HasErrors, "Should have errors for negative Age.");

        viewModel.Age = 25;

        Assert.IsFalse(viewModel.HasErrors, "Errors should be cleared when Age is valid.");
    }
}
