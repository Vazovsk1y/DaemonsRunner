using DaemonsRunner.DAL.Extensions;
using DaemonsRunner.DAL.Repositories.Interfaces;
using DaemonsRunner.Domain.Models;
using Newtonsoft.Json;

namespace DaemonsRunner.DAL.Repositories;

public class ScriptRepository : IScriptRepository
{
	private readonly string _storageFilePath;
	private readonly object _locker = new();

	public ScriptRepository(IStorage storageDirectory)
	{
		_storageFilePath = Path.Combine(storageDirectory.FullPath, "executablesFiles.json");
	}

	public void Insert(Script entity)
	{
		lock (_locker)
		{
			var currentEntities = new List<ScriptJsonModel>();
			currentEntities.AddRange(GetAll().Select(e => e.ToJsonModel()));

			using var writer = new StreamWriter(_storageFilePath);
			currentEntities.Add(entity.ToJsonModel());

			string json = JsonConvert.SerializeObject(currentEntities, Formatting.Indented);
			writer.Write(json);
		}
	}

	public IEnumerable<Script> GetAll()
	{
		if (!File.Exists(_storageFilePath))
		{
			return Enumerable.Empty<Script>();
		}

		lock (_locker)
		{
			using var reader = new StreamReader(_storageFilePath);
			string json = reader.ReadToEnd();
			return JsonConvert.DeserializeObject<IEnumerable<ScriptJsonModel>>(json)?.Select(e => e.ToModel()) ?? Enumerable.Empty<Script>();
		}
	}

	public void Remove(ScriptId scriptId)
	{
		lock (_locker)
		{
			var currentEntities = GetAll().ToList();

			var itemToRemove = currentEntities.FirstOrDefault(e => e.Id == scriptId);
			if (itemToRemove is null)
			{
				return;
			}

			currentEntities.Remove(itemToRemove);
			using var writer = new StreamWriter(_storageFilePath);
			string json = JsonConvert.SerializeObject(currentEntities, Formatting.Indented);
			writer.Write(json);
		}
	}
}

public class ScriptJsonModel
{
	public required Guid Id { get; init; }

	public string Title { get; set; } = null!;

	public string Command { get; set; } = null!;

	public string? ExecutableFilePath { get; set; }
}
