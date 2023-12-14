using DaemonsRunner.Core.Models;

namespace DaemonsRunner.Application.Responses.DTOs;

public record ScriptDTO(ScriptId Id, string Title, string Command, string? WorkingDirectoryPath = null);
