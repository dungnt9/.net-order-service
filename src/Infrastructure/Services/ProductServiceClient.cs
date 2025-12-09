using Application.Common.Interfaces;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using ProductService.Grpc;

namespace Infrastructure.Services;

public class ProductServiceClient : IProductServiceClient
{
    private readonly ProductGrpc.ProductGrpcClient _grpcClient;
    private readonly HttpClient _httpClient;
    private readonly ILogger<ProductServiceClient> _logger;
    private readonly bool _useGrpc;

    public ProductServiceClient(
        ProductGrpc.ProductGrpcClient grpcClient,
        HttpClient httpClient, 
        ILogger<ProductServiceClient> logger)
    {
        _grpcClient = grpcClient;
        _httpClient = httpClient;
        _logger = logger;
        _useGrpc = true; // Default to gRPC
    }

    public async Task<ProductDto?> GetProductAsync(int productId)
    {
        if (_useGrpc)
        {
            return await GetProductViaGrpcAsync(productId);
        }
        return await GetProductViaHttpAsync(productId);
    }

    private async Task<ProductDto?> GetProductViaGrpcAsync(int productId)
    {
        try
        {
            _logger.LogInformation("Calling ProductService via gRPC to get product {ProductId}", productId);
            
            var request = new GetProductRequest { Id = productId };
            var response = await _grpcClient.GetProductAsync(request);
            
            if (!response.Found)
            {
                _logger.LogWarning("Product {ProductId} not found via gRPC", productId);
                return null;
            }

            _logger.LogInformation("Successfully retrieved product {ProductId} via gRPC", productId);
            return new ProductDto(
                response.Id,
                response.Name,
                response.Brand,
                decimal.Parse(response.Price),
                response.Description,
                response.Stock,
                response.IsActive,
                response.CategoryId,
                response.CategoryName,
                DateTime.Parse(response.CreatedAt)
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling ProductService via gRPC for product {ProductId}", productId);
            return null;
        }
    }

    private async Task<ProductDto?> GetProductViaHttpAsync(int productId)
    {
        try
        {
            _logger.LogInformation("Calling ProductService via HTTP to get product {ProductId}", productId);
            
            var response = await _httpClient.GetAsync($"/api/products/{productId}");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                
                var product = JsonSerializer.Deserialize<ProductDto>(content, options);
                _logger.LogInformation("Successfully retrieved product {ProductId} via HTTP", productId);
                return product;
            }
            
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogWarning("Product {ProductId} not found via HTTP", productId);
                return null;
            }
            
            _logger.LogError("Failed to get product {ProductId}. Status: {StatusCode}", productId, response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling ProductService via HTTP for product {ProductId}", productId);
            return null;
        }
    }

    public async Task<StockCheckResult> CheckStockAsync(int productId, int quantity)
    {
        try
        {
            _logger.LogInformation("Calling ProductService via gRPC to check stock for product {ProductId}, quantity {Quantity}", 
                productId, quantity);
            
            var request = new CheckStockRequest { ProductId = productId, Quantity = quantity };
            var response = await _grpcClient.CheckStockAsync(request);
            
            _logger.LogInformation("Stock check result for product {ProductId}: Available={IsAvailable}, CurrentStock={CurrentStock}", 
                productId, response.IsAvailable, response.CurrentStock);
            
            return new StockCheckResult(
                response.IsAvailable,
                response.CurrentStock,
                response.Message
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking stock for product {ProductId}", productId);
            return new StockCheckResult(false, 0, $"Error checking stock: {ex.Message}");
        }
    }

    public async Task<StockUpdateResult> UpdateStockAsync(int productId, int quantityChange)
    {
        try
        {
            _logger.LogInformation("Calling ProductService via gRPC to update stock for product {ProductId}, change {QuantityChange}", 
                productId, quantityChange);
            
            var request = new UpdateStockRequest { ProductId = productId, QuantityChange = quantityChange };
            var response = await _grpcClient.UpdateStockAsync(request);
            
            _logger.LogInformation("Stock update result for product {ProductId}: Success={Success}, NewStock={NewStock}", 
                productId, response.Success, response.NewStock);
            
            return new StockUpdateResult(
                response.Success,
                response.NewStock,
                response.Message
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating stock for product {ProductId}", productId);
            return new StockUpdateResult(false, 0, $"Error updating stock: {ex.Message}");
        }
    }
}