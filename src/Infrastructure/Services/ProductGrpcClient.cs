using Application.Common.Interfaces;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ProductService.Grpc;

namespace Infrastructure.Services;

/// <summary>
/// gRPC client implementation for Product Service
/// </summary>
public class ProductGrpcClient : IProductGrpcClient, IDisposable
{
    private readonly GrpcChannel _channel;
    private readonly ProductGrpc.ProductGrpcClient _client;
    private readonly ILogger<ProductGrpcClient> _logger;

    public ProductGrpcClient(IConfiguration configuration, ILogger<ProductGrpcClient> logger)
    {
        _logger = logger;
        
        var grpcAddress = configuration["Services:ProductService:GrpcUrl"] ?? "http://localhost:6002";
        
        _logger.LogInformation("Initializing gRPC client with address: {GrpcAddress}", grpcAddress);
        
        _channel = GrpcChannel.ForAddress(grpcAddress);
        _client = new ProductGrpc.ProductGrpcClient(_channel);
    }

    public async Task<ProductDto?> GetProductAsync(int productId)
    {
        try
        {
            _logger.LogInformation("gRPC: Getting product {ProductId}", productId);

            var request = new GetProductRequest { Id = productId };
            var response = await _client.GetProductAsync(request);

            if (!response.Found)
            {
                _logger.LogWarning("gRPC: Product {ProductId} not found", productId);
                return null;
            }

            var product = MapToProductDto(response);
            _logger.LogInformation("gRPC: Successfully retrieved product {ProductId}", productId);
            
            return product;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "gRPC: Error getting product {ProductId}", productId);
            return null;
        }
    }

    public async Task<IEnumerable<ProductDto>> GetProductsAsync(IEnumerable<int> productIds)
    {
        try
        {
            var idsList = productIds.ToList();
            _logger.LogInformation("gRPC: Getting {Count} products", idsList.Count);

            var request = new GetProductsRequest();
            request.Ids.AddRange(idsList);

            var response = await _client.GetProductsAsync(request);

            var products = response.Products
                .Where(p => p.Found)
                .Select(MapToProductDto)
                .ToList();

            _logger.LogInformation("gRPC: Successfully retrieved {Count} products", products.Count);
            
            return products;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "gRPC: Error getting products");
            return Enumerable.Empty<ProductDto>();
        }
    }

    private static ProductDto MapToProductDto(ProductResponse response)
    {
        return new ProductDto(
            response.Id,
            response.Name,
            response.Brand,
            decimal.TryParse(response.Price, out var price) ? price : 0,
            response.Description,
            response.Stock,
            response.IsActive,
            DateTime.TryParse(response.CreatedAt, out var createdAt) ? createdAt : DateTime.MinValue
        );
    }

    public void Dispose()
    {
        _channel?.Dispose();
    }
}
