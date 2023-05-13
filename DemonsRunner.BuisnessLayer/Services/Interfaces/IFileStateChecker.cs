﻿using DemonsRunner.Domain.Models;
using DemonsRunner.Domain.Responses.Intefaces;

namespace DemonsRunner.BuisnessLayer.Services.Interfaces
{
    public interface IFileStateChecker
    {
        /// <summary>
        /// Checks is file exist on PC. 
        /// </summary>
        /// <returns>
        /// Response where Success status is Exist, and Fail is not Exist.
        /// </returns>
        Task<IResponse> IsFileExistAsync(PHPDemon file);
    }
}