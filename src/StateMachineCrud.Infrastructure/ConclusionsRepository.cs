using System.Text.Json;
using StateMachineCrud.Domain;

namespace StateMachineCrud.Infrastructure;

public class ConclusionsRepository
{
    private Oracle oracle = new();

    public async Task<NewHolidayConclusion> GetNewAsync(Guid id)
    {
        var row = await oracle.Select(id.ToString());
        return JsonSerializer.Deserialize<NewHolidayConclusion>(row.Data);
    }

    public async Task<bool> Upsert(HolidayConclusionBase conclusion)
    {
        await oracle.Insert(conclusion.Id)
    }
}
