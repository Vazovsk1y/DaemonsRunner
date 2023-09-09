using DaemonsRunner.ViewModels;

namespace DaemonsRunner.Infrastructure.Messages;

internal enum ExitType
{
    ByTaskManager,
    ByButtonInsideApp
}

internal record ScriptExitedMessage(ScriptExecutorViewModel Sender, ExitType ExitType);
