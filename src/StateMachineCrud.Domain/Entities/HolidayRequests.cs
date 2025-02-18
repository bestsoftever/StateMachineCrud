namespace StateMachineCrud.Domain.Entities;

public abstract class HolidayRequestBase
{
	public Guid Id { get; protected set; }
}

public interface IApprovableHolidays
{
	ApprovedHolidayRequest Approve(string approvedBy, DateTimeOffset approvedDate);
};

public interface ICancellableHolidays
{
	CancelledHolidayRequest Cancel(string cancelledBy, DateTimeOffset cancelledDate);
};

public class NewHolidayRequest
: HolidayRequestBase, IApprovableHolidays, ICancellableHolidays
{
	public string EmployeeName { get; init; }
	public DateTimeOffset StartDate { get; init; }
	public DateTimeOffset EndDate { get; init; }

	public NewHolidayRequest(Guid id, string employeeName, DateTimeOffset startData, DateTimeOffset endData)
	{
		Id = id;
		EmployeeName = employeeName;
		StartDate = startData;
		EndDate = endData;
	}

	public RejectedHolidayRequest Reject(string rejectedBy, DateTimeOffset rejectedDate, string rejectedReason) =>
		new(Id, rejectedBy, rejectedDate, rejectedReason);

	public ApprovedHolidayRequest Approve(string approvedBy, DateTimeOffset approvedDate) => new(Id, approvedBy, approvedDate);

	public CancelledHolidayRequest Cancel(string cancelledBy, DateTimeOffset cancelledDate) => new(Id, cancelledBy, cancelledDate);
}

public class ApprovedHolidayRequest : HolidayRequestBase, ICancellableHolidays
{
	public string ApprovedBy { get; init; }
	public DateTimeOffset ApprovedDate { get; init; }

	public ApprovedHolidayRequest(Guid id, string approvedBy, DateTimeOffset approvedDate)
	{
		Id = id;
		ApprovedBy = approvedBy;
		ApprovedDate = approvedDate;
	}

	public CancelledHolidayRequest Cancel(string cancelledBy, DateTimeOffset cancelledDate) => new(Id, cancelledBy, cancelledDate);
}

public class RejectedHolidayRequest : HolidayRequestBase, IApprovableHolidays
{
	public string RejectedBy { get; init; }
	public DateTimeOffset RejectedDate { get; init; }
	public string RejectionReason { get; }

	public RejectedHolidayRequest(Guid id, string rejectedBy, DateTimeOffset rejectedDate, string rejectionReason)
	{
		Id = id;
		RejectedBy = rejectedBy;
		RejectedDate = rejectedDate;
		RejectionReason = rejectionReason;
	}

	public ApprovedHolidayRequest Approve(string approvedBy, DateTimeOffset approvedDate) => new(Id, approvedBy, approvedDate);
}

public class CancelledHolidayRequest : HolidayRequestBase
{
	public CancelledHolidayRequest(Guid id, string cancelledBy, DateTimeOffset cancelledDate)
	{
		Id = id;
		CancelledBy = cancelledBy;
		CancelledDate = cancelledDate;
	}

	public string CancelledBy { get; }
	public DateTimeOffset CancelledDate { get; }
}
