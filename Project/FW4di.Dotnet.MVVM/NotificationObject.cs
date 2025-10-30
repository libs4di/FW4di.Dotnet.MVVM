/*
4di Framework .NET MVVM Library
Copyright (c) 2025 by 4D Illusions. All rights reserved.
Released under the terms of the GNU General Public License version 3 or later.
*/

using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FW4di.Dotnet.MVVM;

/// <summary>
/// Base class for MVVM view models providing property change notifications
/// and validation error tracking through INotifyPropertyChanged and INotifyDataErrorInfo.
/// </summary>
public class NotificationObject : INotifyPropertyChanged, INotifyDataErrorInfo
{
    private readonly Dictionary<string, List<string>> errors = new();

    public event PropertyChangedEventHandler PropertyChanged;
    public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

    public bool HasErrors => errors.Any();

    public IEnumerable GetErrors(string propertyName)
    {
        return propertyName != null && errors.ContainsKey(propertyName) ? errors[propertyName] : null;
    }

    protected virtual IEnumerable<string> Validate(string propertyName)
    {
        return Enumerable.Empty<string>();
    }

    protected void ValidateProperty(string propertyName)
    {
        this.errors.Remove(propertyName);
        var errors = Validate(propertyName);
        if (errors.Any())
        {
            this.errors[propertyName] = errors.ToList();
        }
        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
    }

    public virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        ValidateProperty(propertyName);
    }

    public void RaisePropertyChanged(params string[] propertyNames)
    {
        foreach (var propertyName in propertyNames)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            ValidateProperty(propertyName);
        }
    }

    public bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        field = value;
        RaisePropertyChanged(propertyName);
        return true;
    }

    public bool SetProperty<T>(ref T field, T value, params string[] propertyNames)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        field = value;
        RaisePropertyChanged(propertyNames);
        return true;
    }
}
