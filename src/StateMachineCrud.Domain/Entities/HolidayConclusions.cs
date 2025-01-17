namespace StateMachineCrud.Domain.Entities;

public abstract class HolidayConclusionBase
{
    public Guid Id { get; protected set; }
}

public interface IApprovableHolidays
{
    ApprovedHolidayConclusion Approve(string approvedBy, DateTimeOffset approvedDate);
};

public interface ICancellableHolidays
{
    CancelledHolidayConclusion Cancel(string cancelledBy, DateTimeOffset cancelledDate);
};

public class NewHolidayConclusion
    : HolidayConclusionBase, IApprovableHolidays, ICancellableHolidays
{
    public string EmployeeName { get; init; }
    public DateTimeOffset StartDate { get; init; }
    public DateTimeOffset EndDate { get; init; }

    public NewHolidayConclusion(Guid id, string employeeName, DateTimeOffset startData, DateTimeOffset endData)
    {
        Id = id;
        EmployeeName = employeeName;
        StartDate = startData;
        EndDate = endData;
    }

    public RejectedHolidayConclusion Reject(string rejectedBy, DateTimeOffset rejectedDate, string rejectedReason) =>
        new(Id, rejectedBy, rejectedDate, rejectedReason);

    public ApprovedHolidayConclusion Approve(string approvedBy, DateTimeOffset approvedDate) => new(Id, approvedBy, approvedDate);

    public CancelledHolidayConclusion Cancel(string cancelledBy, DateTimeOffset cancelledDate) => new(Id, cancelledBy, cancelledDate);
}

public class ApprovedHolidayConclusion : HolidayConclusionBase, ICancellableHolidays
{
    public string ApprovedBy { get; init; }
    public DateTimeOffset ApprovedDate { get; init; }

    public ApprovedHolidayConclusion(Guid id, string approvedBy, DateTimeOffset approvedDate)
    {
        Id = id;
        ApprovedBy = approvedBy;
        ApprovedDate = approvedDate;
    }

    public CancelledHolidayConclusion Cancel(string cancelledBy, DateTimeOffset cancelledDate) => new(Id, cancelledBy, cancelledDate);
}

public class RejectedHolidayConclusion : HolidayConclusionBase, IApprovableHolidays
{
    public string RejectedBy { get; init; }
    public DateTimeOffset RejectedDate { get; init; }
    public string RejectionReason { get; }

    public RejectedHolidayConclusion(Guid id, string rejectedBy, DateTimeOffset rejectedDate, string rejectionReason)
    {
        Id = id;
        RejectedBy = rejectedBy;
        RejectedDate = rejectedDate;
        RejectionReason = rejectionReason;
    }

    public ApprovedHolidayConclusion Approve(string approvedBy, DateTimeOffset approvedDate) => new(Id, approvedBy, approvedDate);
}

public class CancelledHolidayConclusion : HolidayConclusionBase
{
    public CancelledHolidayConclusion(Guid id, string cancelledBy, DateTimeOffset cancelledDate)
    {
        Id = id;
        CancelledBy = cancelledBy;
        CancelledDate = cancelledDate;
    }

    public string CancelledBy { get; }
    public DateTimeOffset CancelledDate { get; }
}
