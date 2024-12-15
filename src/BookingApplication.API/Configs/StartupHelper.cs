using System;
using BookingApplication.Core.Repositories;
using BookingApplication.Infrastructure.Configs;
using BookingApplication.Infrastructure.Contexts;
using BookingApplication.Infrastructure.ModelValidators;
using BookingApplication.Infrastructure.Repositories;
using BookingApplication.Infrastructure.Services;
using BookingApplication.Infrastructure.Validators;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace BookingApplication.API.Configs;

public static class StartupHelper
{
    public static IServiceCollection ConfigureAppServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers().AddNewtonsoftJson();
        services.AddSingleton<TimeProvider>(TimeProvider.System);
        services.AddDbContext<BookingContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("BookingDatabase");
            options.UseSqlite(connectionString, x => x.MigrationsAssembly("BookingApplication.API"));

        });
        services.Configure<BookingConfigOption>(configuration.GetSection(BookingConfigOption.BookingConfig));
        services.AddValidatorsFromAssemblyContaining<BookingCreationValidator>();
        services.AddScoped<IBookingRepository, BookingRepository>();
        services.AddTransient<IBookingValidator, BookingValidator>();
        services.AddTransient<IBookingService, BookingService>();
        return services;
    }

    public static async Task ResetDatabaseAsync(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            try
            {
                var context = scope.ServiceProvider.GetService<BookingContext>();
                if (context != null)
                {
                    await context.Database.EnsureDeletedAsync();
                    await context.Database.MigrateAsync();
                }
            }
            catch (Exception ex)
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger>();
                logger.LogError(ex, "An error occurred while migrating the database.");
            }
        }
    }

}
