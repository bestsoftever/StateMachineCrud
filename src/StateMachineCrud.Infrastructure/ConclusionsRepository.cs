using System.Text.Json;
using StateMachineCrud.Domain.Entities;
using StateMachineCrud.Domain.Repositories;
using StateMachineCrud.Domain.ViewModels;

namespace StateMachineCrud.Infrastructure;

internal record HolidayConclusionDocument(
    Guid Id, string EmployeeName, DateTimeOffset StartDate, DateTimeOffset EndDate,
    string? ApprovedBy = null, DateTimeOffset? ApprovedDate = null,
    string? RejectedBy = null, DateTimeOffset? RejectedDate = null, string RejectedReason = null,
    string? CancelledBy = null, DateTimeOffset? cancelledDate = null);

public class HolidayConclusionViewModelReader(Oracle oracle) : IHolidaysViewModelReader
{
    public async Task<IEnumerable<HolidaysViewModel>> GetAll()
    {
        var rows = await oracle.GetAll();
        return rows.Select(row =>
        {
            var data = JsonSerializer.Deserialize<HolidayConclusionDocument>(row.Data)!;
            return new HolidaysViewModel(row.Type.ToString(),
                            data.Id, data.EmployeeName, data.StartDate, data.EndDate, data.ApprovedBy, data.ApprovedDate,
                            data.RejectedBy, data.RejectedDate, data.RejectedReason, data.CancelledBy, data.cancelledDate);
        });
    }
}

public class ConclusionsRepository(Oracle oracle) : IConclusionsRepository
{
    public async Task<Guid> Upsert(HolidayConclusionBase conclusion)
    {
        static HolidayConclusionDocument MapDocumentForUpdate(HolidayConclusionBase c, HolidayConclusionDocument d) => c switch
        {
            ApprovedHolidayConclusion ah => d with { ApprovedBy = ah.ApprovedBy, ApprovedDate = ah.ApprovedDate },
            RejectedHolidayConclusion rh => d with { RejectedBy = rh.RejectedBy, RejectedDate = rh.RejectedDate, RejectedReason = rh.RejectionReason },
            CancelledHolidayConclusion ch => d with { CancelledBy = ch.CancelledBy, cancelledDate = ch.CancelledDate },
            _ => throw new InvalidOperationException(),
        };

        if (conclusion is NewHolidayConclusion nh)
        {
            var document = new HolidayConclusionDocument(nh.Id, nh.EmployeeName, nh.StartDate, nh.EndDate);
            var data = JsonSerializer.Serialize(document);
            await oracle.Insert(nh.Id.ToString(), (nameof(NewHolidayConclusion), data));
        }
        else
        {
            var document = await ReadDocument(conclusion.Id);
            var updated = MapDocumentForUpdate(conclusion, document.Document);
            var data = JsonSerializer.Serialize(updated);
            await oracle.Update(conclusion.Id.ToString(), (conclusion.GetType().Name, data));
        }

        return conclusion.Id;
    }

    public async Task<IApprovableHolidays> GetForApproval(Guid id) => (IApprovableHolidays)MapToEntity(await ReadDocument(id));

    public async Task<ICancellableHolidays> GetForCancellation(Guid id) => (ICancellableHolidays)MapToEntity(await ReadDocument(id));

    public async Task<NewHolidayConclusion> GetForRejection(Guid id) => (NewHolidayConclusion)MapToEntity(await ReadDocument(id));

    private static HolidayConclusionBase MapToEntity((string Type, HolidayConclusionDocument Document) data)
    {
        var d = data.Document!;
        return data.Type switch
        {
            nameof(NewHolidayConclusion) => new NewHolidayConclusion(d.Id, d.EmployeeName, d.StartDate, d.EndDate),
            nameof(RejectedHolidayConclusion) => new RejectedHolidayConclusion(d.Id, d.RejectedBy, d.RejectedDate.Value, d.RejectedReason),
            nameof(ApprovedHolidayConclusion) => new ApprovedHolidayConclusion(d.Id, d.ApprovedBy, d.ApprovedDate.Value),
            nameof(CancelledHolidayConclusion) => new CancelledHolidayConclusion(d.Id, d.CancelledBy, d.cancelledDate.Value),
            _ => throw new InvalidOperationException("Not found!"),
        };
    }

    private async Task<(string Type, HolidayConclusionDocument Document)> ReadDocument(Guid id)
    {
        var row = await oracle.Select(id.ToString());
        return (row.Type, JsonSerializer.Deserialize<HolidayConclusionDocument>(row.Data)!);
    }
}
