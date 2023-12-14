using DaemonsRunner.Core.Models;

namespace DaemonsRunner.DAL.Repositories.Interfaces;

public interface IScriptRepository
{
	IEnumerable<Script> GetAll();

	void Insert(Script entity);

	bool Remove(ScriptId scriptId);
}
