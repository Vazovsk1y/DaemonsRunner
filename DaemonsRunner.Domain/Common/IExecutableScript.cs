using DaemonsRunner.Domain.Models;

namespace DaemonsRunner.Domain.Common;

public interface IExecutableScript
{
	ExecutableFile? ExecutableFile { get; }

	string Title { get; }

	string Command { get; }
}