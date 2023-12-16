using DaemonsRunner.Core.Models;

namespace DaemonsRunner.WPF.ViewModels.Interfaces;

internal interface IScriptExecutorViewModelFactory
{
    ScriptExecutorViewModel CreateViewModel(ScriptExecutor scriptExecutor);
}
