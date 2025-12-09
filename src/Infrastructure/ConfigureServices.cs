using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Application.Common.Interfaces;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Services;
using ProductService.Grpc;

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

        // HTTP Client for ProductService (fallback)
        services.AddHttpClient<ProductServiceClient>(client =>
        {
            client.BaseAddress = new Uri(configuration["Services:ProductService:BaseUrl"] ?? "http://productservice:8080");
            client.Timeout = TimeSpan.FromSeconds(30);
        });
        
        // gRPC Client for ProductService
        var grpcAddress = configuration["Services:ProductService:GrpcUrl"] ?? "http://productservice:8081";
        services.AddGrpcClient<ProductGrpc.ProductGrpcClient>(options =>
        {
            options.Address = new Uri(grpcAddress);
        });
        
        // Register ProductServiceClient with gRPC and HTTP support
        services.AddScoped<IProductServiceClient, ProductServiceClient>();
        
        // Register ProductGrpcClient
        services.AddScoped<IProductGrpcClient, ProductGrpcClient>();
        
        services.AddSingleton<INotificationService, RabbitMQNotificationService>();

        return services;
    }
}