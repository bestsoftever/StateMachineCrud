using MediatR;
using StateMachineCrud.Domain;
using StateMachineCrud.Infrastructure;

namespace StateMachineCrud.Application;

public record CreateHolidayRequest(string EmployeeName, DateTimeOffset StartDate, DateTimeOffset EndDate) 
    : IRequest<CreateHolidayRequest>;

public class CreateHolidaysUseCase(ConclusionsRepository conclusionsRepository) 
    : IRequestHandler<CreateHolidayRequest, CreateHolidayRequest>
{
    public async Task<CreateHolidayRequest> Handle(CreateHolidayRequest request, CancellationToken cancellationToken)
    {
        var holidayConclusion = new NewHolidayConclusion(request.EmployeeName, request.StartDate, request.EndDate);
        await conclusionsRepository.Upsert(holidayConclusion);
        return request;
    }
}
