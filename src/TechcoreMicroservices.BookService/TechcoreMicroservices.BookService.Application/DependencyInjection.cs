using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TechcoreMicroservices.BookService.Application.Common.Interfaces.HttpServices;
using TechcoreMicroservices.BookService.Application.Common.Interfaces.Services;
using TechcoreMicroservices.BookService.Application.HttpServices;
using TechcoreMicroservices.BookService.Application.Services;

namespace TechcoreMicroservices.BookService.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            AddApplicationServices(services);

            // Для общения с микросервисом отзывов по Http
            AddHttpClientFactory(services);

            return services;
        }

        private static void AddApplicationServices(IServiceCollection services)
        {
            services.AddScoped<IBookService, TechcoreMicroservices.BookService.Application.Services.BookService>();
            services.AddScoped<IAuthorService, AuthorService>();
            services.AddScoped<IBookDetailsService, BookDetailsService>();
        }

        private static void AddHttpClientFactory(IServiceCollection services)
        {
            services.AddHttpClient<IBookReviewHttpService, BookReviewHttpService>(client =>
            {
                client.BaseAddress = new Uri("https://localhost:7227");
            });
        }
    }
}
