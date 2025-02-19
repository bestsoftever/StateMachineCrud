using StateMachineCrud.Domain.Entities;

namespace StateMachineCrud.Domain.Repositories;

public interface IRequestsRepository
{
	Task<IApprovableHolidays> GetForApproval(Guid id);
	Task<NewHolidayRequest> GetForRejection(Guid id);
	Task<ICancellableHolidays> GetForCancellation(Guid id);
	Task<Guid> Upsert(HolidayRequestBase conclusion);
}
