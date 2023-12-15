using DaemonsRunner.Core.Enums;
using DaemonsRunner.Core.Models;

namespace DaemonsRunner.Application.Responses.DTOs;

public record ScriptDTO(ScriptId Id, string Title, string Command, RuntimeType RuntimeType, string? WorkingDirectoryPath = null);
