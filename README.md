# FW4di.Dotnet.MVVM

**FW4di.Dotnet.MVVM** is a lightweight .NET MVVM framework designed to provide cross-platform, UI-independent building blocks for creating maintainable applications following the MVVM pattern.

---

## Features

- **ICommand implementation** (`RelayCommand`, `AsyncRelayCommand`)  
  Supports both synchronous and asynchronous commands with `CanExecute` support.

- **Property Change Notification** (`NotificationObject`)  
  Base class for ViewModels providing `INotifyPropertyChanged` functionality.

- **Event Aggregation** (`EventAggregator`)  
  A thread-safe, loosely coupled publish-subscribe mechanism for communication between components.

- **Messenger** (`Messenger`)  
  A simple type-safe message bus for synchronous and asynchronous messages.

- **Cross-Platform & UI Independent**  
  All components are framework-agnostic, so they can be used in WPF, MAUI, Avalonia, Blazor, Unity, or any other .NET platform.
