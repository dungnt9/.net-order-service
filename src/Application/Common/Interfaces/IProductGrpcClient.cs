namespace Application.Common.Interfaces;

/// <summary>
/// Interface for Product Service gRPC client
/// Used to get product information via gRPC protocol
/// </summary>
public interface IProductGrpcClient
{
    /// <summary>
    /// Get product by ID via gRPC
    /// </summary>
    Task<ProductDto?> GetProductAsync(int productId);

    /// <summary>
    /// Get multiple products by IDs via gRPC
    /// </summary>
    Task<IEnumerable<ProductDto>> GetProductsAsync(IEnumerable<int> productIds);
}
