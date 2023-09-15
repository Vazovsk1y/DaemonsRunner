using DaemonsRunner.Domain.Exceptions.Base;
using DaemonsRunner.Domain.Models;
using DaemonsRunner.Domain.Tests.Infrastructure.EventSpies;

namespace DaemonsRunner.Domain.Tests;

public class ScritpExecutorTests
{
    [Fact]
    public void IS_Object_Disposed_Exception_Thrown_on_disposed_object_propertiesUsing()
    {
        using var testObject = CreateTestExecutor();

        testObject.Dispose();

        Assert.Throws<ObjectDisposedException>(() => testObject.IsRunning);
        Assert.Throws<ObjectDisposedException>(() => testObject.IsMessagesReceiving);
        Assert.Throws<ObjectDisposedException>(() => testObject.ExecutableScript);
    }

    [Fact]
    public void IS_Object_Disposed_Exception_Thrown_on_disposed_object_methodsUsing()
    {
        using var testObject = CreateTestExecutor();

        testObject.Dispose();

        Assert.Throws<ObjectDisposedException>(testObject.Start);
        Assert.Throws<ObjectDisposedException>(testObject.StartMessagesReceiving);
        Assert.Throws<ObjectDisposedException>(testObject.ExecuteCommand);
        Assert.Throws<ObjectDisposedException>(testObject.StopMessagesReceiving);
        Assert.Throws<ObjectDisposedException>(testObject.Stop);
    }

    [Fact]
    public void IS_Starting_Correct()
    {
        bool expected = true;
        using var testObject = CreateTestExecutor();

        testObject.Start();

        Assert.Equal(expected, testObject.IsRunning);
    }

    [Fact]
    public void Is_Multiply_Start_Method_Calling_doesnt_raise_exceptions()
    {
        using var testExecutor = CreateTestExecutor();

        testExecutor.Start();
        testExecutor.Start();
        testExecutor.Start();
        testExecutor.Start();
    }

    [Fact]
    public void Is_Multiply_Stop_Method_Calling_doesnt_raise_exceptions()
    {
        using var testObject = CreateTestExecutor();

        testObject.Start();

        testObject.Stop();
        testObject.Stop();
        testObject.Stop();
        testObject.Stop();
    }

    [Fact]
    public void IS_Stopping_Correct()
    {
        bool expected = false;
        using var testObject = CreateTestExecutor();

        testObject.Start();
        testObject.Stop();

        Assert.Equal(expected, testObject.IsRunning);
    }

    [Fact]
    public void Is_Messages_Receiving_Breaks_when_Stop_method_called()
    {
        using var testExecutor = CreateTestExecutor();

        testExecutor.Start();
        testExecutor.StartMessagesReceiving();
        testExecutor.Stop();

        Assert.False(testExecutor.IsRunning);
        Assert.False(testExecutor.IsMessagesReceiving);
    }

    [Fact]
    public void IS_Messages_Receiving_Starting_Correct()
    {
        bool expected = true;
        using var testExecutor = CreateTestExecutor();

        testExecutor.Start();
        testExecutor.StartMessagesReceiving();

        Assert.Equal(expected, testExecutor.IsMessagesReceiving);
    }

    [Fact]
    public void IS_Messages_Receiving_Starting_NOT_Available_when_executor_was_NOT_started()
    {
        using var testExecutor = CreateTestExecutor();

        Assert.Throws<DomainException>(testExecutor.StartMessagesReceiving);
    }

    [Fact]
    public void IS_Command_Executing_NOT_Available_when_executor_NOT_started()
    {
        using var testExecutor = CreateTestExecutor();

        Assert.Throws<DomainException>(testExecutor.ExecuteCommand);
    }

    [Fact]
    public async Task IS_Executor_Exited_By_Task_Manager_Event_NOT_Raised_when_executor_killed_by_TaskManager()
    {
        using var testExecutor = CreateTestExecutor();
        var eventSpy = new ExecutorExitedByTaskManagerEventSpy();
        testExecutor.ExitedByTaskManager += eventSpy.HandleEvent;

        testExecutor.Start();
        testExecutor.Stop();
        await Task.Delay(BaseEventSpy.EventWaitTimeMs);

        Assert.False(eventSpy.EventHandled);
    }

    [Theory]
    [InlineData(2)]
    [InlineData(5)]
    [InlineData(10)]
    public void IS_Executor_Can_Be_Reusable_when_only_start_and_stop_invoked(int reusingCycleCount)
    {
        using var testExecutor = CreateTestExecutor();

        for (int i = 0; i < reusingCycleCount; i++)
        {
            testExecutor.Start();
            Assert.True(testExecutor.IsRunning);
            testExecutor.Stop();
            Assert.False(testExecutor.IsRunning);
        }
    }

    [Theory]
    [InlineData(2)]
    [InlineData(5)]
    [InlineData(10)]
    public void IS_Executor_Can_Be_Reusable_when_all_executor_methods_invoked(int reusingCycleCount)
    {
        using var testExecutor = CreateTestExecutor();

        for (int i = 0; i < reusingCycleCount; i++)
        {
            testExecutor.Start();
            Assert.True(testExecutor.IsRunning);

            testExecutor.StartMessagesReceiving();
            Assert.True(testExecutor.IsMessagesReceiving);

            testExecutor.ExecuteCommand();

            testExecutor.StopMessagesReceiving();
            Assert.False(testExecutor.IsMessagesReceiving);

            testExecutor.Stop();
            Assert.False(testExecutor.IsRunning);
        }
    }

    private static ScriptExecutor CreateTestExecutor()
    {
        var script = Script.Create("Random Script", $"ping www.google.com");

        return ScriptExecutor.Create(script);
    }
}
