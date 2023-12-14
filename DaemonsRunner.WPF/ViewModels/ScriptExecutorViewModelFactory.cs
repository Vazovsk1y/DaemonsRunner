using DaemonsRunner.Application.Services.Interfaces;
using DaemonsRunner.Core.Models;
using DaemonsRunner.WPF.ViewModels.Interfaces;

namespace DaemonsRunner.WPF.ViewModels;

internal class ScriptExecutorViewModelFactory : IScriptExecutorViewModelFactory
{
	private readonly IDataBus _dataBus;

	public ScriptExecutorViewModelFactory(
		IDataBus dataBus)
	{
		_dataBus = dataBus;
	}

	public ScriptExecutorViewModel CreateViewModel(ScriptExecutor scriptExecutor)
	{
		var viewModel = new ScriptExecutorViewModel(scriptExecutor, _dataBus);

		return viewModel;
	}
}
