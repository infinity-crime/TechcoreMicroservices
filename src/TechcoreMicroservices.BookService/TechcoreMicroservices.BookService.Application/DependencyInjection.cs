using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TechcoreMicroservices.BookService.Application.Common.Interfaces.Services;
using TechcoreMicroservices.BookService.Application.Services;

namespace TechcoreMicroservices.BookService.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            AddApplicationServices(services);

            return services;
        }

        private static void AddApplicationServices(IServiceCollection services)
        {
            services.AddScoped<IBookService, TechcoreMicroservices.BookService.Application.Services.BookService>();
            services.AddScoped<IAuthorService, AuthorService>();
        }
    }
}
