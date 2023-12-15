using DaemonsRunner.Application.Responses;
using DaemonsRunner.Core.Models;
using DaemonsRunner.Application.Responses.DTOs;

namespace DaemonsRunner.Application.Services.Interfaces;

public interface IScriptService
{
	Task<DataResponse<IEnumerable<ScriptDTO>>> GetAllAsync(CancellationToken cancellationToken = default);

	Task<DataResponse<ScriptId>> SaveAsync(ScriptAddDTO scriptAddDTO, CancellationToken cancellationToken = default);

	Task<Response> DeleteAsync(ScriptId scriptId, CancellationToken cancellationToken = default);

	Task<DataResponse<ScriptExecutor>> StartAsync(StartScriptOptions startScriptOptions, CancellationToken cancellationToken = default);
}

public record StartScriptOptions(ScriptId ScriptId, bool StartMessagesReceiving = true, bool ExecuteCommand = true);