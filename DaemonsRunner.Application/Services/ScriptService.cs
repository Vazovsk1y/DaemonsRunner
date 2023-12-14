using DaemonsRunner.Application.Responses;
using DaemonsRunner.Application.Services.Interfaces;
using DaemonsRunner.DAL.Repositories.Interfaces;
using DaemonsRunner.Core.Models;
using DaemonsRunner.Application.Extensions;
using DaemonsRunner.Application.Responses.DTOs;

namespace DaemonsRunner.Application.Services;

public class ScriptService : IScriptService
{
	private readonly IScriptRepository _scriptRepository;

	public ScriptService(IScriptRepository scriptRepository)
	{
		_scriptRepository = scriptRepository;
	}

	public Task<Response> DeleteAsync(ScriptId scriptId, CancellationToken cancellationToken = default)
	{
		var result = _scriptRepository.Remove(scriptId);

		if (!result)
		{
			return Task.FromResult(Response.Fail("Script with passed id is not exists."));
		}
		
		return Task.FromResult(Response.Success($"Script was successfully deleted."));
	}

	public Task<DataResponse<IEnumerable<ScriptDTO>>> GetAllAsync(CancellationToken cancellationToken = default)
	{
		var result = _scriptRepository.GetAll().Select(e => e.ToDTO());
		string description = $"[{result.Count()}] scripts were successfully received.";
		return Task.FromResult(Response.Success(result, description));
	}

	public Task<DataResponse<ScriptId>> SaveAsync(ScriptAddDTO scriptAddDTO, CancellationToken cancellationToken = default)
	{
	    WorkingDirectory? workingDirectory = string.IsNullOrWhiteSpace(scriptAddDTO.WorkingDirectoryPath) ? 
			null : new WorkingDirectory { Path = scriptAddDTO.WorkingDirectoryPath };

		var script = new Script 
		{
			Title = scriptAddDTO.Title,
			Command = scriptAddDTO.Command,
			WorkingDirectory = workingDirectory,
			RuntimeType = scriptAddDTO.RuntimeType,
		}; 

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

		if (scriptToExecute.WorkingDirectory is not null && !Directory.Exists(scriptToExecute.WorkingDirectory.Path))
		{
			return Task.FromResult(Response.Fail<ScriptExecutor>($"Working directory [{scriptToExecute.WorkingDirectory.Path}] is not exists."));
		}

		var executor = ScriptExecutor.Create(scriptToExecute);
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
