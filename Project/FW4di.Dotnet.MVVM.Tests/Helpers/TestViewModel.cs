/*
4di Framework .NET MVVM Library
Copyright (c) 2025 by 4D Illusions. All rights reserved.
Released under the terms of the GNU General Public License version 3 or later.
*/

namespace FW4di.Dotnet.MVVM.Tests.Helpers;

public class TestViewModel : TestNotificationObject
{
    private int age;
    public int Age
    {
        get => age;
        set
        {
            if (SetProperty(ref age, value))
            {
                ValidateProperty(nameof(Age));
            }
        }
    }

    private string name;
    public string Name
    {
        get => name;
        set
        {
            if (SetProperty(ref name, value))
            {
                ValidateProperty(nameof(Name));
            }
        }
    }

    protected override IEnumerable<string> Validate(string propertyName)
    {
        var errors = new List<string>();

        if (propertyName == nameof(Age))
        {
            if (Age <= 0)
            {
                errors.Add("Age cannot be negative or zero.");
            }
        }

        if (propertyName == nameof(Name))
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                errors.Add("Name cannot be empty.");
            }
        }

        return errors;
    }
}
