using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PgScalabilityTest.Data;

namespace PgScalabilityTest.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCoreDataServices(this IServiceCollection services)
    {
        services.AddOptions<DbSettings>();
        services.AddDbContext<AnalyticsDbContext>((sp, optionsBuilder) => {
            var settings = sp.GetRequiredService<IOptions<DbSettings>>();
            var cs = $"{settings.Value.ConnectionString};Minimum Pool Size=100;Maximum Pool Size=500;";
            optionsBuilder.UseNpgsql(cs, options => options.EnableRetryOnFailure());
            
        });
        services.AddScoped<EmailEventService>();
        //services.AddSingleton<SimulatedWorkloadService>();
        return services;
    }
}
