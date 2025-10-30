/*
4di Framework .NET MVVM Library
Copyright (c) 2025 by 4D Illusions. All rights reserved.
Released under the terms of the GNU General Public License version 3 or later.
*/

namespace FW4di.Dotnet.MVVM;

/// <summary>
/// A simple type-safe publish–subscribe messenger for loose coupling between components.
/// </summary>
/// <remarks>
/// This class allows both synchronous and asynchronous handlers for message types.
/// It is cross-platform and does not depend on any UI framework.
/// 
/// <para>
/// <b>Limitations:</b>
/// - The internal dictionaries are not thread-safe; multi-threaded use may require locking or a concurrent collection.
/// - Async handlers are executed fire-and-forget using <see cref="Task.Run"/>; exceptions are logged but not propagated.
/// - Does not use weak references; subscribers must unregister manually to avoid memory leaks.
/// </para>
/// </remarks>
public static class Messenger
{
    private static readonly Dictionary<Type, List<Delegate>> handlerList = new();
    private static readonly Dictionary<Type, List<Func<object, Task>>> asyncHandlerList = new();

    public static void Register<TMessage>(Action<TMessage> handler)
    {
        var messageType = typeof(TMessage);
        if (!handlerList.ContainsKey(messageType))
        {
            handlerList[messageType] = new List<Delegate>();
        }
        handlerList[messageType].Add(handler);
    }

    public static void RegisterAsync<TMessage>(Func<TMessage, Task> asyncHandler)
    {
        var messageType = typeof(TMessage);
        if (!asyncHandlerList.ContainsKey(messageType))
        {
            asyncHandlerList[messageType] = new List<Func<object, Task>>();
        }
        asyncHandlerList[messageType].Add(async msg => await asyncHandler((TMessage)msg));
    }

    public static void Send<TMessage>(TMessage message)
    {
        var messageType = typeof(TMessage);

        if (handlerList.ContainsKey(messageType))
        {
            foreach (var handler in handlerList[messageType])
            {
                try
                {
                    if (handler is Action<TMessage> typedHandler)
                    {
                        typedHandler(message);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Handler threw an exception: {ex.Message}");
                }
            }
        }

        if (asyncHandlerList.ContainsKey(messageType))
        {
            foreach (var asyncHandler in asyncHandlerList[messageType])
            {
                Task.Run(async () =>
                {
                    try
                    {
                        await asyncHandler(message);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Async handler threw an exception: {ex.Message}");
                    }
                });
            }
        }
    }

    public static async Task SendAsync<TMessage>(TMessage message)
    {
        var messageType = typeof(TMessage);

        // Sync handlers
        if (handlerList.ContainsKey(messageType))
        {
            foreach (var handler in handlerList[messageType])
            {
                try
                {
                    if (handler is Action<TMessage> typedHandler)
                    {
                        typedHandler(message);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Handler threw an exception: {ex.Message}");
                }
            }
        }

        // Async handlers - AWAIT them!
        if (asyncHandlerList.ContainsKey(messageType))
        {
            var tasks = new List<Task>();
            foreach (var asyncHandler in asyncHandlerList[messageType])
            {
                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        await asyncHandler(message);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Async handler threw an exception: {ex.Message}");
                    }
                }));
            }

            await Task.WhenAll(tasks);
        }
    }

    public static void Unregister<TMessage>(Action<TMessage> handler)
    {
        var messageType = typeof(TMessage);
        if (handlerList.ContainsKey(messageType))
        {
            handlerList[messageType].Remove(handler);
            if (handlerList[messageType].Count == 0)
            {
                handlerList.Remove(messageType);
            }
        }
    }

    public static void UnregisterAsync<TMessage>(Func<TMessage, Task> asyncHandler)
    {
        var messageType = typeof(TMessage);
        if (asyncHandlerList.ContainsKey(messageType))
        {
            asyncHandlerList[messageType].Remove(async msg => asyncHandler((TMessage)msg));
            if (asyncHandlerList[messageType].Count == 0)
            {
                asyncHandlerList.Remove(messageType);
            }
        }
    }
}
