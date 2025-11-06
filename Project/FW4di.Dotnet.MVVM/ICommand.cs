#define ANDROID

/*
4di Framework .NET MVVM Library
Copyright (c) 2025 by 4D Illusions. All rights reserved.
Released under the terms of the GNU General Public License version 3 or later.
 */

namespace FW4di.Dotnet.MVVM;

/// <summary>
/// Cross-platform ICommand interface that adapts to the target platform's native ICommand.
/// On WinUI: implements Microsoft.UI.Xaml.Input.ICommand
/// On MAUI/Xamarin: implements System.Windows.Input.ICommand
/// On other platforms: uses own definition compatible with System.Windows.Input.ICommand
/// </summary>
/// 
#if WINUI
public interface ICommand : Microsoft.UI.Xaml.Input.ICommand { }
#elif ANDROID || IOS || MACCATALYST || WINDOWS_WPF
public interface ICommand : System.Windows.Input.ICommand { }
#else
public interface ICommand
{
    void Execute(object? parameter);
    bool CanExecute(object? parameter);
    event EventHandler? CanExecuteChanged;
}
#endif