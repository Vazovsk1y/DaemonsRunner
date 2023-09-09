using DaemonsRunner.DAL;

namespace DaemonsRunner.WPF;

internal class Storage : IStorage
{
	public string FullPath { get; }

	public Storage(string fullPath)
	{
		FullPath = fullPath;
	}
}
