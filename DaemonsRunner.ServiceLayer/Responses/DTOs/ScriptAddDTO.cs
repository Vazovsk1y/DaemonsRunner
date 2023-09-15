namespace DaemonsRunner.ServiceLayer.Responses.DTOs;

public record ScriptAddDTO(string Title, string Command, string? ExecutableFilePath = null);