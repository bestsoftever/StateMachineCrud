using MediatR;
using StateMachineCrud.Domain;

namespace StateMachineCrud.Application;

public record CreateHolidaysRequest(string EmployeeName, DateTimeOffset StartDate, DateTimeOffset EndDate)
    : IRequest<CreateHolidaysRequest>;

public class CreateHolidaysUseCase(IConclusionsRepository conclusionsRepository)
    : IRequestHandler<CreateHolidaysRequest, CreateHolidaysRequest>
{
    public async Task<CreateHolidaysRequest> Handle(CreateHolidaysRequest request, CancellationToken cancellationToken)
    {
        var holidayConclusion = new NewHolidayConclusion(request.EmployeeName, request.StartDate, request.EndDate);
        await conclusionsRepository.Upsert(holidayConclusion);
        return request;
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
