namespace Application.Common.Interfaces;

public interface IProductServiceClient
{
    Task<ProductDto?> GetProductAsync(int productId);
}

public record ProductDto(
    int Id,
    string Name,
    string Brand,
    decimal Price,
    string Description,
    int Stock,
    bool IsActive,
    DateTime CreatedAt
);