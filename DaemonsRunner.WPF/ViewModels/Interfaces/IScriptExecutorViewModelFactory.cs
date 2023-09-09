using DaemonsRunner.Domain.Models;

namespace DaemonsRunner.ViewModels.Interfaces;

internal interface IScriptExecutorViewModelFactory
{
    ScriptExecutorViewModel CreateViewModel(ScriptExecutor scriptExecutor);
}
