namespace DaemonsRunner.Core.Tests.Infrastructure.EventSpies;

internal class ExecutorExitedByTaskManagerEventSpy : BaseEventSpy
{
    public void HandleEvent(object? sender, EventArgs e)
    {
        EventHandled = true;
    }
}
