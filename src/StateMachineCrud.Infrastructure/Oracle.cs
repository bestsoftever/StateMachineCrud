namespace StateMachineCrud.Infrastructure;

public class Oracle
{
    public record Record(string Type, string Data);

    private Dictionary<string, Record> _mysql = [];

    public async Task<Record> Select(string id)
    {
        await Task.Yield();
        return _mysql[id];
    }

    public async Task Insert(string id, Record record)
    {
        await Task.Yield();
        _mysql.Add(id, record);
    }
}
