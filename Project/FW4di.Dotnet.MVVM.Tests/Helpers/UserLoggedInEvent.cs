/*
4di Framework .NET MVVM Library
Copyright (c) 2025 by 4D Illusions. All rights reserved.
Released under the terms of the GNU General Public License version 3 or later.
*/

namespace FW4di.Dotnet.MVVM.Tests.Helpers;

public class UserLoggedInEvent
{
    public string Username { get; }

    public UserLoggedInEvent(string username)
    {
        Username = username ?? throw new ArgumentNullException(nameof(username));
    }
}
