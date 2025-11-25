using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace TechcoreMicroservices.ApiGateway.OcelotGw.Extensions;

public static class DependencyExtensions
{
    public static IServiceCollection AddBearerAuth(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSection = configuration.GetSection("JwtSettings");

        Console.WriteLine(jwtSection["Issuer"]);
        Console.WriteLine(jwtSection["Audience"]);
        Console.WriteLine(jwtSection["Secret"]);


        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = jwtSection["Issuer"],
                    ValidAudience = jwtSection["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Secret"]!))
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var has = context.Request.Headers.ContainsKey("Authorization");
                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>()
                            .CreateLogger("JwtAuth");

                        logger.LogInformation("OnMessageReceived. Authorization header present: {HasHeader}", has);

                        return Task.CompletedTask;
                    },

                    OnAuthenticationFailed = context =>
                    {
                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>()
                           .CreateLogger("JwtAuth");

                        logger.LogError(context.Exception, $"Authentication Failed: {context.Exception.Message}");
                        if(context.Exception.InnerException != null)
                        {
                            logger.LogError($"Inner exception: {context.Exception.InnerException.Message}");
                        }

                        return Task.CompletedTask;
                    }
                };
            });

        services.AddAuthorization();

        return services;
    }
}
