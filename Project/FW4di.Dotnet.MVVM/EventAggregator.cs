/*
4di Framework .NET MVVM Library
Copyright (c) 2025 by 4D Illusions. All rights reserved.
Released under the terms of the GNU General Public License version 3 or later.
*/

using System.Collections.Concurrent;

namespace FW4di.Dotnet.MVVM;

/// <summary>
/// Provides a simple, type-safe publish–subscribe (pub/sub) mechanism for loosely coupled communication 
/// between components such as ViewModels or services.
/// </summary>
/// <remarks>
/// This class supports asynchronous handlers and stores them using <see cref="WeakReference{T}"/>,
/// preventing memory leaks if subscribers are garbage collected.
/// 
/// Thread-safety is ensured via <see cref="ConcurrentDictionary{TKey,TValue}"/> and locks on the internal handler lists.
/// This implementation is cross-platform and does not depend on any UI framework.
/// </remarks>
public class EventAggregator
{
    private readonly ConcurrentDictionary<Type, List<WeakReference<Delegate>>> subscriberList = new();

    public void Subscribe<T>(Func<T, Task> asyncHandler)
    {
        var key = typeof(T);
        var weakHandler = new WeakReference<Delegate>(asyncHandler);

        subscriberList.AddOrUpdate(key,
            _ => new List<WeakReference<Delegate>> { weakHandler },
            (_, list) =>
            {
                lock (list)
                {
                    list.Add(weakHandler);
                }
                return list;
            });
    }

    public void Unsubscribe<T>(Func<T, Task> asyncHandler)
    {
        var key = typeof(T);
        if (subscriberList.TryGetValue(key, out var handlers))
        {
            lock (handlers)
            {
                handlers.RemoveAll(wr => wr.TryGetTarget(out var target) && target == (Delegate)asyncHandler);
            }

            CleanupSubscribers(key, handlers);
        }
    }

    private void CleanupSubscribers(Type key, List<WeakReference<Delegate>> handlers)
    {
        lock (handlers)
        {
            handlers.RemoveAll(wr => !wr.TryGetTarget(out _));
            if (handlers.Count == 0)
            {
                subscriberList.TryRemove(key, out _);
            }
        }
    }

    public async Task Publish<T>(T message)
    {
        var key = typeof(T);
        if (subscriberList.TryGetValue(key, out var handlers))
        {
            List<WeakReference<Delegate>> toRemove = new();

            foreach (var weakRef in handlers.ToList())
            {
                if (weakRef.TryGetTarget(out var target))
                {
                    var handler = (Func<T, Task>)target;
                    await handler(message).ConfigureAwait(false);
                }
                else
                {
                    toRemove.Add(weakRef);
                }
            }

            if (toRemove.Count > 0)
            {
                lock (handlers)
                {
                    handlers.RemoveAll(wr => toRemove.Contains(wr));
                    if (handlers.Count == 0)
                    {
                        subscriberList.TryRemove(key, out _);
                    }
                }
            }
        }
    }
}
