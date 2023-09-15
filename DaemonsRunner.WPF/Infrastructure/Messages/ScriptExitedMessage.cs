using DaemonsRunner.ViewModels;

namespace DaemonsRunner.Infrastructure.Messages;

internal record ScriptExitedMessage(ScriptExecutorViewModel Sender, bool ExitedByTaskManager = false);
