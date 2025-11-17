using Microsoft.Extensions.DependencyInjection;
using TechcoreMicroservices.BookOrderService.Application.Common.Interfaces.Services;
using TechcoreMicroservices.BookOrderService.Application.Services;

namespace TechcoreMicroservices.BookOrderService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        AddServices(services);

        return services;
    }

    private static void AddServices(IServiceCollection services)
    {
        services.AddScoped<IOrderService, OrderService>();
    }
}
