using DaemonsRunner.WPF.ViewModels;

namespace DaemonsRunner.WPF.Infrastructure.Messages;

internal record ScriptExitedMessage(ScriptExecutorViewModel Sender, bool ExitedByTaskManager = false);
