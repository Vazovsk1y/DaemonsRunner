using DaemonsRunner.Core.Enums;

namespace DaemonsRunner.Application.Responses.DTOs;

public record ScriptAddDTO(string Title, string Command, RuntimeType RuntimeType, string? WorkingDirectoryPath = null);