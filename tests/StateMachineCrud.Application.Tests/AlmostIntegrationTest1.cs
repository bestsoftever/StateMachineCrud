using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using StateMachineCrud.Domain.Repositories;
using StateMachineCrud.Domain.ViewModels;
using StateMachineCrud.Infrastructure;

namespace StateMachineCrud.Application.Tests;

public class AlmostIntegrationTest1
{
    [Fact]
    public async Task EverythingWorksAsync()
    {
        var mediator = ConfigureMediatr();

        var id = await mediator.Send(new CreateHolidaysRequest("Piotr", new DateTime(2025, 1, 20), new DateTime(2025, 12, 31)));
        await mediator.Send(new RejectHolidaysRequest(id, "Urlop jest dla dyrekcji!"));
        await mediator.Send(new ApproveHolidaysRequest(id));
        await mediator.Send(new CancelHolidaysRequest(id));
        var view = await mediator.Send(new GetAllHolidaysRequest());

        view.ShouldHaveSingleItem();
    }

    private static IMediator ConfigureMediatr()
    {
        var services = new ServiceCollection();
        services.AddSingleton<Oracle>();
        services.AddTransient<IConclusionsRepository, ConclusionsRepository>();
        services.AddTransient<IHolidaysViewModelReader, HolidayConclusionViewModelReader>();
        var serviceProvider = services
            .AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(CreateHolidaysUseCase).Assembly))
            .BuildServiceProvider();
        var mediator = serviceProvider.GetRequiredService<IMediator>();

        return mediator;
    }
}