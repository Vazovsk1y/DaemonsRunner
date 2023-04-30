﻿using DemonsRunner.Domain.Interfaces;
using DemonsRunner.Domain.Models;
using DemonsRunner.Domain.Responses;

namespace DemonsRunner.Domain.Services
{
    public interface IScriptExecutorService
    {
        public Task<IDataResponse<PHPScriptExecutor>> StartAsync(PHPScript script, bool showExecutingWindow);

        public Task<IBaseResponse> StopAsync(IEnumerable<PHPScriptExecutor> executingScripts);
    }
}
