using System.Text.Json;
using StateMachineCrud.Domain.Entities;
using StateMachineCrud.Domain.Repositories;
using StateMachineCrud.Domain.ViewModels;

namespace StateMachineCrud.Infrastructure;

internal record HolidayRequestDocument(
Guid Id, string EmployeeName, DateTimeOffset StartDate, DateTimeOffset EndDate,
string? ApprovedBy = null, DateTimeOffset? ApprovedDate = null,
string? RejectedBy = null, DateTimeOffset? RejectedDate = null, string RejectedReason = null,
string? CancelledBy = null, DateTimeOffset? cancelledDate = null);

public class HolidayRequestViewModelReader(Oracle oracle) : IHolidaysViewModelReader
{
	public async Task<IEnumerable<HolidaysViewModel>> GetAll()
	{
		var rows = await oracle.GetAll();
		return rows.Select(row =>
		{
			var data = JsonSerializer.Deserialize<HolidayRequestDocument>(row.Data)!;
			return new HolidaysViewModel(row.Type.ToString(),
							data.Id, data.EmployeeName, data.StartDate, data.EndDate, data.ApprovedBy, data.ApprovedDate,
							data.RejectedBy, data.RejectedDate, data.RejectedReason, data.CancelledBy, data.cancelledDate);
		});
	}
}

public class RequestsRepository(Oracle oracle) : IRequestsRepository
{
	public async Task<Guid> Upsert(HolidayRequestBase conclusion)
	{
		static HolidayRequestDocument MapDocumentForUpdate(HolidayRequestBase c, HolidayRequestDocument d) => c switch
		{
			ApprovedHolidayRequest ah => d with { ApprovedBy = ah.ApprovedBy, ApprovedDate = ah.ApprovedDate },
			RejectedHolidayRequest rh => d with { RejectedBy = rh.RejectedBy, RejectedDate = rh.RejectedDate, RejectedReason = rh.RejectionReason },
			CancelledHolidayRequest ch => d with { CancelledBy = ch.CancelledBy, cancelledDate = ch.CancelledDate },
			_ => throw new InvalidOperationException(),
		};

		if (conclusion is NewHolidayRequest nh)
		{
			var document = new HolidayRequestDocument(nh.Id, nh.EmployeeName, nh.StartDate, nh.EndDate);
			var data = JsonSerializer.Serialize(document);
			await oracle.Insert(nh.Id.ToString(), (nameof(NewHolidayRequest), data));
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

	public async Task<NewHolidayRequest> GetForRejection(Guid id) => (NewHolidayRequest)MapToEntity(await ReadDocument(id));

	private static HolidayRequestBase MapToEntity((string Type, HolidayRequestDocument Document) data)
	{
		var d = data.Document!;
		return data.Type switch
		{
			nameof(NewHolidayRequest) => new NewHolidayRequest(d.Id, d.EmployeeName, d.StartDate, d.EndDate),
			nameof(RejectedHolidayRequest) => new RejectedHolidayRequest(d.Id, d.RejectedBy, d.RejectedDate.Value, d.RejectedReason),
			nameof(ApprovedHolidayRequest) => new ApprovedHolidayRequest(d.Id, d.ApprovedBy, d.ApprovedDate.Value),
			nameof(CancelledHolidayRequest) => new CancelledHolidayRequest(d.Id, d.CancelledBy, d.cancelledDate.Value),
			_ => throw new InvalidOperationException("Not found!"),
		};
	}

	private async Task<(string Type, HolidayRequestDocument Document)> ReadDocument(Guid id)
	{
		var row = await oracle.Select(id.ToString());
		return (row.Type, JsonSerializer.Deserialize<HolidayRequestDocument>(row.Data)!);
	}
}
