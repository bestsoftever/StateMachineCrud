
namespace StateMachineCrud.Infrastructure;

public class Oracle
{
    private Dictionary<string, (string Type, string Data)> _mysql = [];

    public async Task<(string Type, string Data)> Select(string id)
    {
        await Task.Yield();
        return _mysql[id];
    }

    public async Task Insert(string id, (string Type, string Data) record)
    {
        await Task.Yield();
        _mysql.Add(id, record);
    }

    public async Task Update(string id, (string Type, string Data) record)
    {
        await Task.Yield();
        _mysql[id] = record;
    }

    public async Task<IEnumerable<(string Type, string Data)>> GetAll()
    {
        await Task.Yield();
        return _mysql.Values;
    }
}
