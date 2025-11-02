/*
4di Framework .NET MVVM Library
Copyright (c) 2025 by 4D Illusions. All rights reserved.
Released under the terms of the GNU General Public License version 3 or later.
*/

namespace FW4di.Dotnet.MVVM;

/// <summary>
/// Type-safe publish–subscribe messenger for loose coupling between components.
/// </summary>
/// <remarks>
/// Supports both synchronous (<see cref="Action{T}"/>) and asynchronous (<see cref="Func{T, Task}"/>) handlers.
/// Cross-platform, no UI dependencies.
///
/// <para><b>Behavior</b></para>
/// <list type="bullet">
///   <item><description><see cref="Send{TMessage}(TMessage)"/> invokes synchronous handlers inline and dispatches asynchronous handlers in a fire-and-forget manner.</description></item>
///   <item><description><see cref="SendAsync{TMessage}(TMessage)"/> invokes synchronous handlers inline and <em>awaits</em> completion of all asynchronous handlers.</description></item>
///   <item><description>Handler exceptions are swallowed to avoid preventing other handlers from running.</description></item>
/// </list>
///
/// <para><b>Thread-safety</b></para>
/// Registration/unregistration and snapshot creation are guarded by a lock; handler invocation runs outside the lock.
/// No weak references are used, so subscribers must explicitly unregister to avoid memory leaks.
/// </remarks>
public static class Messenger
{
    private static readonly object _lock = new();

    // Stores Action<TMessage> handlers
    private static Dictionary<Type, List<Delegate>> handlerList = [];

    // Stores Func<TMessage, Task> handlers (original delegates, not wrappers)
    private static Dictionary<Type, List<Delegate>> asyncHandlerList = [];

    /// <summary> Clears all registered handlers. Intended for tests. </summary>
    public static void Reset()
    {
        lock (_lock)
        {
            handlerList = [];
            asyncHandlerList = [];
        }
    }

    /// <summary> Registers a synchronous handler for the specified message type. </summary>
    public static void Register<TMessage>(Action<TMessage> handler)
    {
        lock (_lock)
        {
            if (!handlerList.TryGetValue(typeof(TMessage), out var list))
                handlerList[typeof(TMessage)] = list = [];
            list.Add(handler);
        }
    }

    /// <summary> Registers an asynchronous handler for the specified message type. </summary>
    public static void RegisterAsync<TMessage>(Func<TMessage, Task> asyncHandler)
    {
        lock (_lock)
        {
            if (!asyncHandlerList.TryGetValue(typeof(TMessage), out var list))
                asyncHandlerList[typeof(TMessage)] = list = [];
            list.Add(asyncHandler); // store original delegate (important for UnregisterAsync)
        }
    }

    /// <summary> Unregisters a previously registered synchronous handler. </summary>
    public static void Unregister<TMessage>(Action<TMessage> handler)
    {
        lock (_lock)
        {
            if (handlerList.TryGetValue(typeof(TMessage), out var list))
            {
                list.Remove(handler);
                if (list.Count == 0) handlerList.Remove(typeof(TMessage));
            }
        }
    }

    /// <summary> Unregisters a previously registered asynchronous handler. </summary>
    public static void UnregisterAsync<TMessage>(Func<TMessage, Task> asyncHandler)
    {
        lock (_lock)
        {
            if (asyncHandlerList.TryGetValue(typeof(TMessage), out var list))
            {
                list.Remove(asyncHandler);
                if (list.Count == 0) asyncHandlerList.Remove(typeof(TMessage));
            }
        }
    }

    /// <summary> Sends a message. Synchronous handlers run inline; asynchronous handlers are started fire-and-forget. </summary>
    public static void Send<TMessage>(TMessage message)
    {
        // Snapshot to avoid holding the lock during invocation
        List<Delegate> syncHandlers, asyncHandlers;
        lock (_lock)
        {
            handlerList.TryGetValue(typeof(TMessage), out syncHandlers);
            asyncHandlerList.TryGetValue(typeof(TMessage), out asyncHandlers);
            syncHandlers = syncHandlers != null ? [.. syncHandlers] : [.. Array.Empty<Delegate>()];
            asyncHandlers = asyncHandlers != null ? [.. asyncHandlers] : [.. Array.Empty<Delegate>()];
        }

        // Synchronous: run inline
        foreach (var d in syncHandlers)
        {
            if (d is Action<TMessage> a)
            {
                try { a(message); } catch { /* don't stop others */ }
            }
        }

        // Asynchronous: fire-and-forget
        foreach (var d in asyncHandlers)
        {
            if (d is Func<TMessage, Task> fa)
            {
                _ = Task.Run(async () =>
                {
                    try { await fa(message).ConfigureAwait(false); } catch { /* swallow */ }
                });
            }
        }
    }

    /// <summary> Sends a message and awaits completion of all asynchronous handlers. </summary>
    public static async Task SendAsync<TMessage>(TMessage message)
    {
        // Snapshot
        List<Delegate> syncHandlers, asyncHandlers;
        lock (_lock)
        {
            handlerList.TryGetValue(typeof(TMessage), out syncHandlers);
            asyncHandlerList.TryGetValue(typeof(TMessage), out asyncHandlers);
            syncHandlers = syncHandlers != null ? [.. syncHandlers] : [.. Array.Empty<Delegate>()];
            asyncHandlers = asyncHandlers != null ? [.. asyncHandlers] : [.. Array.Empty<Delegate>()];
        }

        // Synchronous: inline
        foreach (var d in syncHandlers)
        {
            if (d is Action<TMessage> a)
            {
                try { a(message); } catch { /* swallow */ }
            }
        }

        // Asynchronous: start and await all
        var tasks = new List<Task>(asyncHandlers.Count);
        foreach (var d in asyncHandlers)
        {
            if (d is Func<TMessage, Task> fa)
            {
                tasks.Add(Task.Run(async () =>
                {
                    try { await fa(message).ConfigureAwait(false); } catch { /* swallow */ }
                }));
            }
        }

        if (tasks.Count > 0)
            await Task.WhenAll(tasks).ConfigureAwait(false);
    }
}
