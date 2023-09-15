namespace DaemonsRunner.Domain.Tests.Infrastructure.EventSpies;

internal abstract class BaseEventSpy
{
    public bool EventHandled { get; protected set; }

    public static int EventWaitTimeMs => 100;
}
