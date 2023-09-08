using DaemonsRunner.Domain.Models;

namespace DaemonsRunner.DAL.Repositories.Interfaces;

public interface IScriptRepository
{
	IEnumerable<Script> GetAll();

	void Insert(Script entity);

	void Remove(ScriptId scriptId);
}
