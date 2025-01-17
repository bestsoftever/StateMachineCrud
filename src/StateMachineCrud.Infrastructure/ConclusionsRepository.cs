using System.Text.Json;
using StateMachineCrud.Domain.Entities;
using StateMachineCrud.Domain.Repositories;
using StateMachineCrud.Domain.ViewModels;

namespace StateMachineCrud.Infrastructure;

internal enum HolidayConclusionDocumentType { New, Approved, Rejected, Cancelled }

internal record HolidayConclusionDocument(HolidayConclusionDocumentType Type, Guid Id, string EmployeeName,
    DateTimeOffset StartDate, DateTimeOffset EndDate,
    string? ApprovedBy = null, DateTimeOffset? ApprovedDate = null,
    string? RejectedBy = null, DateTimeOffset? RejectedDate = null, string RejectedReason = null,
    string? CancelledBy = null, DateTimeOffset? cancelledDate = null);

public class HolidayConclusionViewModelReader(Oracle oracle) : IHolidaysViewModelReader
{
    public async Task<IEnumerable<HolidaysViewModel>> GetAll()
    {
        var rows = await oracle.GetAll();
        return rows
            .Select(row => JsonSerializer.Deserialize<HolidayConclusionDocument>(row.Data))
            .Select(data => new HolidaysViewModel(data.Type.ToString(),
                data.Id, data.EmployeeName, data.StartDate, data.EndDate, data.ApprovedBy, data.ApprovedDate, 
                data.RejectedBy, data.RejectedDate, data.RejectedReason, data.CancelledBy, data.cancelledDate)
            );
    }
}

public class ConclusionsRepository(Oracle oracle) : IConclusionsRepository
{
    public async Task<Guid> Upsert(HolidayConclusionBase conclusion)
    {
        static HolidayConclusionDocument UpdateDocument(HolidayConclusionBase c, HolidayConclusionDocument d)
        {
            switch (c)
            {
                case ApprovedHolidayConclusion ah:
                    return d with { ApprovedBy = ah.ApprovedBy, ApprovedDate = ah.ApprovedDate };
                case RejectedHolidayConclusion rh:
                    return d with { RejectedBy = rh.RejectedBy, RejectedDate = rh.RejectedDate, RejectedReason = rh.RejectionReason };
                case CancelledHolidayConclusion ch:
                    return d with { CancelledBy = ch.CancelledBy, cancelledDate = ch.CancelledDate };
                default:
                    throw new InvalidOperationException();
            }
        }

        if (conclusion is NewHolidayConclusion nh)
        {
            var document = new HolidayConclusionDocument(HolidayConclusionDocumentType.New, nh.Id, nh.EmployeeName, nh.StartDate, nh.EndDate);
            var data = JsonSerializer.Serialize(document);
            await oracle.Insert(nh.Id.ToString(), (nameof(NewHolidayConclusion), data));
        }
        else
        {
            var document = await ReadFromOracle(conclusion.Id);
            var updated = UpdateDocument(conclusion, document);
            var data = JsonSerializer.Serialize(updated);
            await oracle.Update(conclusion.Id.ToString(), (conclusion.GetType().Name, data));
        }

        return conclusion.Id;
    }

    private async Task<HolidayConclusionDocument> ReadFromOracle(Guid id)
    {
        var row = await oracle.Select(id.ToString());
        return JsonSerializer.Deserialize<HolidayConclusionDocument>(row.Data);
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
