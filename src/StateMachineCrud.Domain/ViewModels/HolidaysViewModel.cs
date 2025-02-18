namespace StateMachineCrud.Domain.ViewModels;

public record HolidaysViewModel(string Type, Guid Id, string EmployeeName,
DateTimeOffset StartDate, DateTimeOffset EndDate,
string? ApprovedBy, DateTimeOffset? ApprovedDate,
string? RejectedBy, DateTimeOffset? RejectedDate, string RejectedReason,
string? CancelledBy, DateTimeOffset? cancelledDate);

public interface IHolidaysViewModelReader
{
	Task<IEnumerable<HolidaysViewModel>> GetAll();
}