namespace StateMachineCrud.Domain;

public interface IConclusionsRepository
{
    Task<IApprovableHolidays> GetForApproval(Guid id);
    Task<NewHolidayConclusion> GetForRejection(Guid id);
    Task<ICancellableHolidays> GetForCancellation(Guid id);
    Task<Guid> Upsert(HolidayConclusionBase conclusion);
}