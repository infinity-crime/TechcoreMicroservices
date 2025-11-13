using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechcoreMicroservices.BookService.Application.Common.Interfaces.Persistence;
using TechcoreMicroservices.BookService.Application.Common.Interfaces.Persistence.Dapper;
using TechcoreMicroservices.BookService.Infrastructure.Data;
using TechcoreMicroservices.BookService.Infrastructure.Data.Repositories.Dapper;
using TechcoreMicroservices.BookService.Infrastructure.Data.Repositories.EFCore;

namespace TechcoreMicroservices.BookService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            AddDbContext(services, configuration);

            AddEfCoreRepositories(services);
            AddDapperRepositories(services);

            return services;
        }

        private static void AddDbContext(IServiceCollection services, IConfiguration configuration)
        {
            var connectionStringDb = configuration.GetConnectionString("DefaultConnection");

            // Регистрация контекста базы данных с использованием Npgsql
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(connectionStringDb);
            });
        }

        private static void AddEfCoreRepositories(IServiceCollection services)
        {
            services.AddScoped<IAuthorRepository, AuthorRepository>();
            services.AddScoped<IBookRepository, BookRepository>();
        }

        private static void AddDapperRepositories(IServiceCollection services)
        {
            services.AddScoped<IBookDapperRepository, BookDapperRepository>();
        }
    }
}
