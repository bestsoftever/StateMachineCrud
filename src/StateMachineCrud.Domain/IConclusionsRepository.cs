namespace StateMachineCrud.Domain;

public interface IConclusionsRepository
{
    Task<IApprovableHolidays> GetForApproval(Guid id);
    Task<NewHolidayConclusion> GetForRejection(Guid id);
    Task<HolidayConclusionBase> GetForCancellation(Guid id);
    Task Upsert(HolidayConclusionBase conclusion);
}