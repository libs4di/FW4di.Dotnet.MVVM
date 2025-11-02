/*
4di Framework .NET MVVM Library
Copyright (c) 2025 by 4D Illusions. All rights reserved.
Released under the terms of the GNU General Public License version 3 or later.
*/

namespace FW4di.Dotnet.MVVM.Tests;

[TestClass]
public class MessengerTests
{
    [TestInitialize]
    public void Setup()
    {
        Messenger.Reset();
    }

    [TestMethod]
    public void RegisterAndSendSynchronousMessage()
    {
        // Arrange
        string receivedMessage = null;
        Messenger.Register<string>(msg => receivedMessage = msg);

        // Act
        Messenger.Send("Hello, World!");

        // Assert
        Assert.AreEqual("Hello, World!", receivedMessage);
    }

    [TestMethod]
    public async Task RegisterAsyncAndSendAsyncMessage()
    {
        // Arrange
        string receivedMessage = null;

        Messenger.RegisterAsync<string>(async msg =>
        {
            await Task.Delay(50);
            receivedMessage = msg;
        });

        // Act
        await Messenger.SendAsync("Hello, Async World!");

        // Assert
        Assert.AreEqual("Hello, Async World!", receivedMessage);
    }

    [TestMethod]
    public void UnregisterHandlerShouldNotReceiveMessage()
    {
        // Arrange
        string receivedMessage = null;
        void Handler(string msg) => receivedMessage = msg;

        Messenger.Register<string>(Handler);
        Messenger.Unregister<string>(Handler);

        // Act
        Messenger.Send("Should not be received");

        // Assert
        Assert.IsNull(receivedMessage, "Deleted handler still get message!");
    }

    [TestMethod]
    public void MultipleHandlersShouldAllReceiveMessage()
    {
        // Arrange
        int counter = 0;
        Messenger.Register<string>(_ => counter++);
        Messenger.Register<string>(_ => counter++);
        Messenger.Register<string>(_ => counter++);

        // Act
        Messenger.Send("Message");

        // Assert
        Assert.AreEqual(3, counter);
    }

    [TestMethod]
    public async Task ExceptionInHandlerShouldNotStopOthers()
    {
        // Arrange
        int successCounter = 0;

        Messenger.Register<string>(_ => throw new Exception("Test Exception"));
        Messenger.Register<string>(_ => successCounter++);

        // Act
        Messenger.Send("Message");
        await Task.Delay(50);

        // Assert
        Assert.AreEqual(1, successCounter);
    }

    [TestMethod]
    public void HandlersAreCalledInParallel()
    {
        // Arrange
        bool handler1Executed = false;
        bool handler2Executed = false;

        Messenger.Register<string>(_ =>
        {
            Thread.Sleep(100);
            handler1Executed = true;
        });

        Messenger.Register<string>(_ =>
        {
            Thread.Sleep(100);
            handler2Executed = true;
        });

        // Act
        Messenger.Send("Parallel Execution Test");
        Thread.Sleep(200);

        // Assert
        Assert.IsTrue(handler1Executed);
        Assert.IsTrue(handler2Executed);
    }

    [TestMethod]
    public async Task SendAsyncHandlerShouldRunIndependently()
    {
        // Arrange
        // Use a TaskCompletionSource to deterministically wait for the async handler
        var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        bool syncExecuted = false;

        // Register a synchronous handler that should execute immediately
        Messenger.Register<string>(_ => syncExecuted = true);

        // Register an asynchronous handler that signals completion via the TCS
        Messenger.RegisterAsync<string>(async _ =>
        {
            // Yield to ensure asynchronous continuation (non-blocking start)
            await Task.Yield();
            tcs.TrySetResult(true);
        });

        // Act
        // Sending should trigger both handlers:
        //  - the sync handler runs inline
        //  - the async handler runs independently
        Messenger.Send("Async Test");

        // Assert
        // Verify that the synchronous handler ran immediately
        Assert.IsTrue(syncExecuted, "Synchronous handler did not run inline.");

        // Wait for the asynchronous handler to complete, with a safe timeout
        await tcs.Task.WaitAsync(TimeSpan.FromSeconds(1));
    }

    [TestMethod]
    public void UnregisterNonExistentHandlerDoesNotThrowException()
    {
        // Arrange
        void Handler(string msg) { }

        try
        {
            Messenger.Unregister<string>(Handler);
        }
        catch (Exception ex)
        {
            Assert.Fail($"Unregister throwed exception: {ex.Message}");
        }
    }

    [TestMethod]
    public void SendMessageWithNoHandlersDoesNotThrowException()
    {
        Messenger.Send("No Handlers");
    }

    [TestMethod]
    public void HandlersAreCalledOnlyOnce()
    {
        // Arrange
        int callCount = 0;
        Messenger.Register<string>(_ => callCount++);

        // Act
        Messenger.Send("Message");
        Messenger.Send("Message");

        // Assert
        Assert.AreEqual(2, callCount);
    }

    [TestMethod]
    public void DifferentMessageTypesAreHandledSeparately()
    {
        // Arrange
        int intHandlerCalled = 0;
        int stringHandlerCalled = 0;

        Messenger.Register<int>(_ => intHandlerCalled++);
        Messenger.Register<string>(_ => stringHandlerCalled++);

        // Act
        Messenger.Send(42);
        Messenger.Send("Test");

        // Assert
        Assert.AreEqual(1, intHandlerCalled);
        Assert.AreEqual(1, stringHandlerCalled);
    }
}
