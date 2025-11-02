# FW4di.Dotnet.MVVM <img src="https://img.shields.io/badge/Windows-0078D6?style=for-the-badge&logo=windows&logoColor=white"> <img src="https://img.shields.io/badge/Linux-FCC624?style=for-the-badge&logo=linux&logoColor=black"> <img src="https://img.shields.io/badge/mac%20os-000000?style=for-the-badge&logo=macos&logoColor=F0F0F0"> <img src="https://img.shields.io/badge/-.NET%209.0-blueviolet">
[![Azure Static Web Apps CI/CD](https://github.com/libs4di/FW4di.Dotnet.MVVM/actions/workflows/dotnet.yml/badge.svg)](https://github.com/libs4di/FW4di.Dotnet.MVVM/actions/workflows/dotnet-desktop.yml)

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
