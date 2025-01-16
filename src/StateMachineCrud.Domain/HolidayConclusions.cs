namespace StateMachineCrud.Domain;

public abstract class HolidayConclusionBase
{
    public Guid Id { get; protected set; }

    public static explicit operator HolidayConclusionBase(object v)
    {
        throw new NotImplementedException();
    }
};

public interface IApprovableHolidays
{
    public ApprovedHolidayConclusion Approve(string approvedBy, DateTimeOffset approvedDate) => new(approvedBy, approvedDate);
};

public class NewHolidayConclusion : HolidayConclusionBase, IApprovableHolidays
{
    public string EmployeeName { get; init; }
    public DateTimeOffset StartDate { get; init; }
    public DateTimeOffset EndDate { get; init; }

    public NewHolidayConclusion(string employeeName, DateTimeOffset startData, DateTimeOffset endData)
    {
        EmployeeName = employeeName;
        StartDate = startData;
        EndDate = endData;
        Id = Guid.NewGuid();
    }
    
    public RejectedHolidayConclusion Reject(string rejectedBy, DateTimeOffset rejectedDate, string rejectedReason) => new(rejectedBy, rejectedDate, rejectedReason);
}

public class ApprovedHolidayConclusion(string approvedBy, DateTimeOffset approvedDate) : HolidayConclusionBase
{
    public string ApprovedBy { get; init; } = approvedBy;
    public DateTimeOffset ApprovedDate { get; init; } = approvedDate;

    public CancelledHolidayConclusion Cancel(string cancelledBy, DateTimeOffset cancelledDate) => new(cancelledBy, cancelledDate);
}

public class RejectedHolidayConclusion(string rejectedBy, DateTimeOffset rejectedDate, string rejectionReason) : HolidayConclusionBase, IApprovableHolidays
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
