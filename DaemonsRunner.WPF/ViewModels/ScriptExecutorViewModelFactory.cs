using DaemonsRunner.BuisnessLayer.Services.Interfaces;
using DaemonsRunner.Domain.Models;
using DaemonsRunner.ViewModels.Interfaces;

namespace DaemonsRunner.ViewModels;

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
