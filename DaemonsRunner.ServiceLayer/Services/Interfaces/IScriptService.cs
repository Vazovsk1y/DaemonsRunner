using DaemonsRunner.BuisnessLayer.Responses;
using DaemonsRunner.Domain.Models;
using DaemonsRunner.ServiceLayer.Responses.DTOs;

namespace DaemonsRunner.BuisnessLayer.Services.Interfaces;

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
