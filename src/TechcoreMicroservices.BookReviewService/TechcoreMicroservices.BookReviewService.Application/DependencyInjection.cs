using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TechcoreMicroservices.BookReviewService.Application.Common.Interfaces.Services;
using TechcoreMicroservices.BookReviewService.Application.Services;

namespace TechcoreMicroservices.BookReviewService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        AddApplicationServices(services);

        return services;
    }

    private static void AddApplicationServices(IServiceCollection services)
    {
        services.AddScoped<IBookReviewService, Application.Services.BookReviewService>();
    }
}
