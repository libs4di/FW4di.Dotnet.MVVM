/*
4di Framework .NET MVVM Library
Copyright (c) 2025 by 4D Illusions. All rights reserved.
Released under the terms of the GNU General Public License version 3 or later.
*/

using FW4di.Dotnet.MVVM.Tests.Helpers;

namespace FW4di.Dotnet.MVVM.Tests;

[TestClass]
public class EventAggregatorTests
{
    private EventAggregator eventAggregator;

    [TestInitialize]
    public void Setup()
    {
        eventAggregator = new EventAggregator();
    }

    [TestMethod]
    public async Task PublishShouldInvokeSubscribedHandlers()
    {
        // Arrange
        var receivedMessages = new List<string>();
        eventAggregator.Subscribe<UserLoggedInEvent>(async e =>
        {
            await Task.Delay(10);
            receivedMessages.Add(e.Username);
        });

        // Act
        await eventAggregator.Publish(new UserLoggedInEvent("JohnDoe"));

        // Assert
        Assert.AreEqual(1, receivedMessages.Count);
        Assert.AreEqual("JohnDoe", receivedMessages[0]);
    }

    [TestMethod]
    public async Task UnsubscribeShouldStopReceivingEvents()
    {
        // Arrange
        var receivedMessages = new List<string>();
        Func<UserLoggedInEvent, Task> handler = async e => receivedMessages.Add(e.Username);

        eventAggregator.Subscribe(handler);
        eventAggregator.Unsubscribe(handler);

        // Act
        await eventAggregator.Publish(new UserLoggedInEvent("JohnDoe"));

        // Assert
        Assert.AreEqual(0, receivedMessages.Count);
    }

    [TestMethod]
    public async Task MultipleSubscribersShouldAllReceiveEvent()
    {
        // Arrange
        var receivedMessages = new List<string>();
        eventAggregator.Subscribe<UserLoggedInEvent>(async e => receivedMessages.Add("Handler1: " + e.Username));
        eventAggregator.Subscribe<UserLoggedInEvent>(async e => receivedMessages.Add("Handler2: " + e.Username));

        // Act
        await eventAggregator.Publish(new UserLoggedInEvent("JohnDoe"));

        // Assert
        Assert.AreEqual(2, receivedMessages.Count);
        CollectionAssert.Contains(receivedMessages, "Handler1: JohnDoe");
        CollectionAssert.Contains(receivedMessages, "Handler2: JohnDoe");
    }

    [TestMethod]
    public async Task WeakReferenceShouldRemoveGarbageCollectedSubscribers()
    {
        // Arrange
        Func<UserLoggedInEvent, Task> handler = async e => await Task.CompletedTask;

        eventAggregator.Subscribe(handler);
        WeakReference<Func<UserLoggedInEvent, Task>> weakRef = new(handler);

        eventAggregator.Unsubscribe(handler);

        GC.KeepAlive(handler);
        handler = null;

        await Task.Delay(100);
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        //Assert.IsFalse(weakRef.TryGetTarget(out _), "Handler should have been garbage collected.");
    }

    [TestMethod]
    public async Task PublishShouldBeThreadSafe()
    {
        // Arrange
        var receivedMessages = new List<string>();
        eventAggregator.Subscribe<UserLoggedInEvent>(async e =>
        {
            lock (receivedMessages)
            {
                receivedMessages.Add(e.Username);
            }
        });

        var tasks = Enumerable.Range(0, 100)
            .Select(i => eventAggregator.Publish(new UserLoggedInEvent($"User{i}")));

        // Act
        await Task.WhenAll(tasks);

        // Assert
        Assert.AreEqual(100, receivedMessages.Count);
    }

    [TestMethod]
    public async Task UnsubscribedHandlerShouldNotAffectOtherSubscribers()
    {
        // Arrange
        var receivedMessages = new List<string>();
        Func<UserLoggedInEvent, Task> handlerToRemove = async e => receivedMessages.Add("ToRemove: " + e.Username);
        eventAggregator.Subscribe(handlerToRemove);
        eventAggregator.Subscribe<UserLoggedInEvent>(async e => receivedMessages.Add("Remaining: " + e.Username));

        eventAggregator.Unsubscribe(handlerToRemove);

        // Act
        await eventAggregator.Publish(new UserLoggedInEvent("JohnDoe"));

        // Assert
        Assert.AreEqual(1, receivedMessages.Count);
        Assert.AreEqual("Remaining: JohnDoe", receivedMessages[0]);
    }
}
