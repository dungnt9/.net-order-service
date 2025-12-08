using MediatR;
using Application.Common.Interfaces;

namespace Application.Features.Orders.Queries.GetOrders;

public class GetOrdersHandler : IRequestHandler<GetOrdersQuery, IEnumerable<GetOrdersResponse>>
{
    private readonly IOrderRepository _repository;
    private readonly IProductGrpcClient _productGrpcClient;

    public GetOrdersHandler(IOrderRepository repository, IProductGrpcClient productGrpcClient)
    {
        _repository = repository;
        _productGrpcClient = productGrpcClient;
    }

    public async Task<IEnumerable<GetOrdersResponse>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await _repository.GetAllAsync();
        var ordersList = orders.ToList();
        
        // Lấy danh sách product IDs để query gRPC một lần
        var productIds = ordersList.Select(o => o.ProductId).Distinct().ToList();
        
        // Lấy thông tin products từ Product Service qua gRPC
        var products = await _productGrpcClient.GetProductsAsync(productIds);
        var productDict = products.ToDictionary(p => p.Id);
        
        return ordersList.Select(o => {
            // Sử dụng thông tin từ gRPC nếu có, nếu không thì dùng cached data
            var productName = productDict.TryGetValue(o.ProductId, out var product) 
                ? product.Name 
                : o.ProductName;
            
            return new GetOrdersResponse(
                o.Id,
                o.CustomerName,
                productName,
                o.Quantity,
                o.TotalAmount,
                o.Status,
                o.CreatedAt
            );
        });
    }
}