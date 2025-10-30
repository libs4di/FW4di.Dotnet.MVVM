/*
4di Framework .NET MVVM Library
Copyright (c) 2025 by 4D Illusions. All rights reserved.
Released under the terms of the GNU General Public License version 3 or later.
*/

namespace FW4di.Dotnet.MVVM.Tests.Helpers;

public class TestNotificationObject : NotificationObject
{
    public string testProperty;

    public string TestProperty
    {
        get => testProperty;
        set => SetProperty(ref testProperty, value);
    }
}
