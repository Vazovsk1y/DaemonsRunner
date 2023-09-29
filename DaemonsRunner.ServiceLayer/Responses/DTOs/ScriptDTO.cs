using DaemonsRunner.Domain.Models;

namespace DaemonsRunner.ServiceLayer.Responses.DTOs;

public record ScriptDTO(ScriptId Id, string Title, string Command, ExecutableFileDTO? ExecutableFile = null);
