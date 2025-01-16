using System.Text.Json;
using StateMachineCrud.Domain;

namespace StateMachineCrud.Infrastructure;

public class ConclusionsRepository(Oracle oracle) : IConclusionsRepository
{
    record HolidaysDocument(string Type, Guid Id, string EmployeeName,
        DateTimeOffset StartDate, DateTimeOffset EndDate,
        string? ApprovedBy, DateTimeOffset? ApprovedDate,
        string? RejectedBy, DateTimeOffset? RejectedDate, string RejectedReason,
        string? CancelledBy, DateTimeOffset? cancelledDate);

    public async Task<Guid> Upsert(HolidayConclusionBase conclusion)
    {
        var data = JsonSerializer.Serialize(conclusion);
        await oracle.Insert(conclusion.Id.ToString(), new(conclusion.GetType().Name, data));
        return conclusion.Id;
    }

    public async Task<IApprovableHolidays> GetForApproval(Guid id)
    {
        var rawData = await ReadFromDb(id, typeof(NewHolidayConclusion).Name, typeof(RejectedHolidayConclusion).Name);
        return (IApprovableHolidays)rawData;
    }

    public async Task<ICancellableHolidays> GetForCancellation(Guid id)
    {
        var rawData = await ReadFromDb(id, typeof(NewHolidayConclusion).Name, typeof(ApprovedHolidayConclusion).Name);
        return (ICancellableHolidays)rawData;
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
