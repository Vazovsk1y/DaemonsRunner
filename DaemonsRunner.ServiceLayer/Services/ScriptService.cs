using DaemonsRunner.BuisnessLayer.Responses;
using DaemonsRunner.BuisnessLayer.Services.Interfaces;
using DaemonsRunner.DAL.Repositories.Interfaces;
using DaemonsRunner.Domain.Models;
using DaemonsRunner.ServiceLayer.Extensions;
using DaemonsRunner.ServiceLayer.Responses.DTOs;

namespace DaemonsRunner.BuisnessLayer.Services;

public class ScriptService : IScriptService
{
	private readonly IScriptRepository _scriptRepository;

	public ScriptService(IScriptRepository scriptRepository)
	{
		_scriptRepository = scriptRepository;
	}

	public Task<Response> DeleteAsync(ScriptId scriptId, CancellationToken cancellationToken = default)
	{
		_scriptRepository.Remove(scriptId);
		return Task.FromResult(Response.Success());
	}

	public Task<DataResponse<IEnumerable<ScriptDTO>>> GetAllAsync(CancellationToken cancellationToken = default)
	{
		var result = _scriptRepository.GetAll().Select(e => e.ToDTO());
		string description = $"[{result.Count()}] were successfully received.";
		return Task.FromResult(Response.Success(result, description));
	}

	public Task<DataResponse<ScriptId>> SaveAsync(ScriptAddDTO scriptAddDTO, CancellationToken cancellationToken = default)
	{
	    ExecutableFile? executableFile = scriptAddDTO.ExecutableFilePath is null ? 
			null : ExecutableFile.Create(scriptAddDTO.ExecutableFilePath);

		var script = Script.Create(scriptAddDTO.Title, scriptAddDTO.Command, executableFile);

		_scriptRepository.Insert(script);

		return Task.FromResult(Response.Success(script.Id));
	}

	public Task<DataResponse<ScriptExecutor>> StartAsync(ScriptId scriptId, 
		bool startMessagesReceiving = true, 
		bool executeCommand = true, 
		CancellationToken cancellationToken = default)
	{
		var scriptToExecute = _scriptRepository.GetAll().FirstOrDefault(e => e.Id == scriptId);
		if (scriptToExecute is null)
		{
			return Task.FromResult(Response.Fail<ScriptExecutor>("Script with passed id is not exists."));
		}

		var executor = ScriptExecutor.Create(scriptToExecute);
		if (executor.ExecutableScript?.ExecutableFile is not null && !File.Exists(executor.ExecutableScript.ExecutableFile.Path))
		{
			return Task.FromResult(Response.Fail<ScriptExecutor>($"Executable file {executor.ExecutableScript.ExecutableFile.Path} is not exists."));
		}

		executor.Start();
		if (startMessagesReceiving)
		{
			executor.StartMessagesReceiving();
		}
		if (executeCommand)
		{
			executor.ExecuteCommand();
		}
		return Task.FromResult(Response.Success(executor));
	}
}
