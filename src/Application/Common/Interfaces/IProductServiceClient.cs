namespace Application.Common.Interfaces;

public interface IProductServiceClient
{
    Task<ProductDto?> GetProductAsync(int productId);
    Task<StockCheckResult> CheckStockAsync(int productId, int quantity);
    Task<StockUpdateResult> UpdateStockAsync(int productId, int quantityChange);
}

public record ProductDto(
    int Id,
    string Name,
    string Brand,
    decimal Price,
    string Description,
    int Stock,
    bool IsActive,
    int CategoryId,
    string CategoryName,
    DateTime CreatedAt
);

public record StockCheckResult(
    bool IsAvailable,
    int CurrentStock,
    string Message
);

public record StockUpdateResult(
    bool Success,
    int NewStock,
    string Message
);