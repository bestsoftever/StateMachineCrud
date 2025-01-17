using MediatR;
using StateMachineCrud.Domain.Entities;
using StateMachineCrud.Domain.Repositories;
using StateMachineCrud.Domain.ViewModels;

namespace StateMachineCrud.Application;

public record CreateHolidaysRequest(string EmployeeName, DateTimeOffset StartDate, DateTimeOffset EndDate)
    : IRequest<Guid>;

public class CreateHolidaysUseCase(IConclusionsRepository conclusionsRepository)
    : IRequestHandler<CreateHolidaysRequest, Guid>
{
    public async Task<Guid> Handle(CreateHolidaysRequest request, CancellationToken cancellationToken)
    {
        var holidayConclusion = new NewHolidayConclusion(Guid.NewGuid(), request.EmployeeName, request.StartDate, request.EndDate);
        var id = await conclusionsRepository.Upsert(holidayConclusion);
        return id;
    }
}

public record ApproveHolidaysRequest(Guid Id) : IRequest<ApproveHolidaysRequest>;

public class ApproveHolidayUseCase(IConclusionsRepository conclusionsRepository)
    : IRequestHandler<ApproveHolidaysRequest, ApproveHolidaysRequest>
{
    public async Task<ApproveHolidaysRequest> Handle(ApproveHolidaysRequest request, CancellationToken cancellationToken)
    {
        var existingConclusion = await conclusionsRepository.GetForApproval(request.Id);
        var approvedConclusion = existingConclusion.Approve("Dyrektorek", DateTimeOffset.UtcNow);
        await conclusionsRepository.Upsert(approvedConclusion);
        return request;
    }
}

public record RejectHolidaysRequest(Guid Id, string Reason) : IRequest<RejectHolidaysRequest>;

public class RejectHolidayUseCase(IConclusionsRepository conclusionsRepository)
    : IRequestHandler<RejectHolidaysRequest, RejectHolidaysRequest>
{
    public async Task<RejectHolidaysRequest> Handle(RejectHolidaysRequest request, CancellationToken cancellationToken)
    {
        var existingConclusion = await conclusionsRepository.GetForRejection(request.Id);
        var approvedConclusion = existingConclusion.Reject("Dyrektorek", DateTimeOffset.UtcNow, request.Reason);
        await conclusionsRepository.Upsert(approvedConclusion);
        return request;
    }
}


public record CancelHolidaysRequest(Guid Id) : IRequest<CancelHolidaysRequest>;

public class CancelHolidayUseCase(IConclusionsRepository conclusionsRepository)
    : IRequestHandler<CancelHolidaysRequest, CancelHolidaysRequest>
{
    public async Task<CancelHolidaysRequest> Handle(CancelHolidaysRequest request, CancellationToken cancellationToken)
    {
        var existingConclusion = await conclusionsRepository.GetForCancellation(request.Id);
        var approvedConclusion = existingConclusion.Cancel("Pokorny pracownik", DateTimeOffset.UtcNow);
        await conclusionsRepository.Upsert(approvedConclusion);
        return request;
    }
}

public record GetAllHolidaysRequest : IRequest<IEnumerable<HolidaysViewModel>>;

public class GetAllHolidaysUseCase(IHolidaysViewModelReader vmReader)
    : IRequestHandler<GetAllHolidaysRequest, IEnumerable<HolidaysViewModel>>
{
    public async Task<IEnumerable<HolidaysViewModel>> Handle(GetAllHolidaysRequest request, CancellationToken cancellationToken)
    {
        return await vmReader.GetAll();
    }
}
