namespace StateMachineCrud.Domain;

public abstract class HolidayConclusionBase { };

public class NewHolidayConclusion(string employeeName, DateTimeOffset startData, DateTimeOffset endData) : HolidayConclusionBase
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string EmployeeName { get; init; } = employeeName;
    public DateTimeOffset StartDate { get; init; } = startData;
    public DateTimeOffset EndDate { get; init; } = endData;

    public ApprovedHolidayConclusion Approve(string approvedBy, DateTimeOffset approvedDate) => new(approvedBy, approvedDate);
    public RejectedHolidayConclusion Reject(string rejectedBy, DateTimeOffset rejectedDate, string rejectedReason) => new(rejectedBy, rejectedDate, rejectedReason);
}

public class ApprovedHolidayConclusion(string approvedBy, DateTimeOffset approvedDate) : HolidayConclusionBase
{
    public string ApprovedBy { get; init; } = approvedBy;
    public DateTimeOffset ApprovedDate { get; init; } = approvedDate;

    public CancelledHolidayConclusion Cancel(string cancelledBy, DateTimeOffset cancelledDate) => new(cancelledBy, cancelledDate);
}

public class RejectedHolidayConclusion(string rejectedBy, DateTimeOffset rejectedDate, string rejectionReason) : HolidayConclusionBase
{
    public string RejectedBy { get; init; } = rejectedBy;
    public DateTimeOffset RejectedDate { get; init; } = rejectedDate;
    public string RejectionReason { get; } = rejectionReason;
}

public class CancelledHolidayConclusion(string cancelledBy, DateTimeOffset cancelledDate) : HolidayConclusionBase
{
    public string CancelledBy { get; } = cancelledBy;
    public DateTimeOffset CancelledDate { get; } = cancelledDate;
}
