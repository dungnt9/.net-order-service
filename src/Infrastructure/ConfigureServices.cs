using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Application.Common.Interfaces;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Services;

namespace Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseMySql(
                configuration.GetConnectionString("DefaultConnection"),
                new MySqlServerVersion(new Version(8, 0, 21))
            ));

        // Repositories
        services.AddScoped<IOrderRepository, OrderRepository>();

        // HTTP Client for ProductService
        services.AddHttpClient<IProductServiceClient, ProductServiceClient>(client =>
        {
            client.BaseAddress = new Uri(configuration["Services:ProductService:BaseUrl"] ?? "http://productservice:8080");
            client.Timeout = TimeSpan.FromSeconds(30);
        });
        
        services.AddSingleton<INotificationService, RabbitMQNotificationService>();

        return services;
    }
}