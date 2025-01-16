using System.Text.Json;
using StateMachineCrud.Domain;

namespace StateMachineCrud.Infrastructure;

public class ConclusionsRepository(Oracle oracle) : IConclusionsRepository
{
    public async Task Upsert(HolidayConclusionBase conclusion)
    {
        var data = JsonSerializer.Serialize(conclusion);
        await oracle.Insert(conclusion.Id.ToString(), new(conclusion.GetType().Name, data));
    }

    public async Task<IApprovableHolidays> GetForApproval(Guid id)
    {
        var rawData = await ReadFromDb(id, typeof(NewHolidayConclusion).Name, typeof(RejectedHolidayConclusion).Name);
        return (IApprovableHolidays)rawData;
    }

    public async Task<HolidayConclusionBase> GetForCancellation(Guid id)
    {
        var rawData = await ReadFromDb(id, typeof(NewHolidayConclusion).Name, typeof(ApprovedHolidayConclusion).Name);
        return (HolidayConclusionBase)rawData;
    }

    public async Task<NewHolidayConclusion> GetForRejection(Guid id)
    {
        var rawData = await ReadFromDb(id, typeof(NewHolidayConclusion).Name);
        return (NewHolidayConclusion)rawData;
    }

    private async Task<object> ReadFromDb(Guid id, params string[] validTypes)
    {
        var row = await oracle.Select(id.ToString());
        if (!validTypes.Contains(row.Type))
        {
            throw new InvalidOperationException("Not found!");
        }

        var type = Type.GetType(row.Type, true, true);
        return JsonSerializer.Deserialize(row.Data, type);
    }
}
