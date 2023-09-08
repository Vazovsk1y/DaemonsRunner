using DaemonsRunner.BuisnessLayer.Responses;
using DaemonsRunner.BuisnessLayer.Services.Interfaces;
using DaemonsRunner.DAL.Repositories.Interfaces;
using DaemonsRunner.Domain.Models;
using DaemonsRunner.Domain.Responses;
using DaemonsRunner.ServiceLayer.Extensions;
using DaemonsRunner.ServiceLayer.Responses.DTOs;

namespace DaemonsRunner.BuisnessLayer.Services;

public class ScriptService : IScriptService
{
	private readonly IRepository<Script> _scriptRepository;

	public ScriptService(IRepository<Script> scriptRepository)
	{
		_scriptRepository = scriptRepository;
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
}
