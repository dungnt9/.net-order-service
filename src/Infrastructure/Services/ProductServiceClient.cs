using Application.Common.Interfaces;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class ProductServiceClient : IProductServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ProductServiceClient> _logger;

    public ProductServiceClient(HttpClient httpClient, ILogger<ProductServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<ProductDto?> GetProductAsync(int productId)
    {
        try
        {
            _logger.LogInformation("Calling ProductService to get product {ProductId}", productId);
            
            var response = await _httpClient.GetAsync($"/api/products/{productId}");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                
                var product = JsonSerializer.Deserialize<ProductDto>(content, options);
                _logger.LogInformation("Successfully retrieved product {ProductId}", productId);
                return product;
            }
            
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogWarning("Product {ProductId} not found", productId);
                return null;
            }
            
            _logger.LogError("Failed to get product {ProductId}. Status: {StatusCode}", productId, response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling ProductService for product {ProductId}", productId);
            return null;
        }
    }
}