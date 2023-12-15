using DaemonsRunner.Core.Enums;
using DaemonsRunner.Core.Exceptions.Base;
using DaemonsRunner.Core.Models;

namespace DaemonsRunner.Core.Tests;

public class ScritpExecutorTests
{
    [Fact]
    public void IS_Object_Disposed_Exception_Thrown_on_disposed_object_whenPropertiesUsing()
    {
        using var testObject = CreateTestExecutor();

        testObject.Dispose();

        Assert.Throws<ObjectDisposedException>(() => testObject.IsRunning);
        Assert.Throws<ObjectDisposedException>(() => testObject.IsMessagesReceiving);
        Assert.Throws<ObjectDisposedException>(() => testObject.ExecutableScript);
    }

    [Fact]
    public void IS_Object_Disposed_Exception_Thrown_on_disposed_object_whenMethodsUsing()
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
    public void IS_Starting_correct()
    {
        bool expected = true;
        using var testObject = CreateTestExecutor();

        testObject.Start();

        Assert.Equal(expected, testObject.IsRunning);
        Assert.False(testObject.IsMessagesReceiving);
    }

	[Fact]
	public void IS_Stopping_correct()
	{
		bool expected = false;
		using var testObject = CreateTestExecutor();

		testObject.Start();
		testObject.Stop();

		Assert.Equal(expected, testObject.IsRunning);
        Assert.False(testObject.IsMessagesReceiving);
	}

	[Fact]
	public void IS_Messages_Receiving_Starting_correct()
	{
		bool expected = true;
		using var testExecutor = CreateTestExecutor();

		testExecutor.Start();
		testExecutor.StartMessagesReceiving();

		Assert.Equal(expected, testExecutor.IsMessagesReceiving);
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
    public void Is_Messages_Receiving_Breaks_when_stop_method_called()
    {
        using var testExecutor = CreateTestExecutor();

        testExecutor.Start();
        testExecutor.StartMessagesReceiving();
        testExecutor.Stop();

        Assert.False(testExecutor.IsRunning);
        Assert.False(testExecutor.IsMessagesReceiving);
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
	public void IS_Executor_Exited_By_Task_Manager_Event_NOT_Raised_when_executor_stopped_by_calling_stop_method()
	{
        bool expected = false;
		bool actual = false;
		using var testExecutor = CreateTestExecutor();

		testExecutor.Start();
        testExecutor.ExitedByTaskManager += (_, _) => actual = true;
        testExecutor.Stop();

		Assert.False(testExecutor.IsRunning);
        Assert.Equal(expected, actual);
		testExecutor.ExitedByTaskManager -= (_, _) => actual = true;
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
        var script = new Script 
        {
            Command = $"ping www.google.com",
            Title = "Random Script",
            RuntimeType = RuntimeType.Cmd,
		};

        return ScriptExecutor.Create(script);
    }
}
