using DaemonsRunner.Application.Responses;
using DaemonsRunner.Core.Models;
using DaemonsRunner.Application.Responses.DTOs;

namespace DaemonsRunner.Application.Services.Interfaces;

public interface IScriptService
{
	Task<DataResponse<IEnumerable<ScriptDTO>>> GetAllAsync(CancellationToken cancellationToken = default);

	Task<DataResponse<ScriptId>> SaveAsync(ScriptAddDTO scriptAddDTO, CancellationToken cancellationToken = default);

	Task<Response> DeleteAsync(ScriptId scriptId, CancellationToken cancellationToken = default);

	Task<DataResponse<ScriptExecutor>> StartAsync(
		ScriptId scriptId,
		bool startMessagesReceiving = true,
		bool executeCommand = true,
		CancellationToken cancellationToken = default);
}
