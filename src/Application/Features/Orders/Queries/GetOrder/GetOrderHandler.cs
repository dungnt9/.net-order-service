using MediatR;
using Application.Common.Interfaces;

namespace Application.Features.Orders.Queries.GetOrder;

public class GetOrderHandler : IRequestHandler<GetOrderQuery, GetOrderResponse?>
{
    private readonly IOrderRepository _repository;
    private readonly IProductGrpcClient _productGrpcClient;

    public GetOrderHandler(IOrderRepository repository, IProductGrpcClient productGrpcClient)
    {
        _repository = repository;
        _productGrpcClient = productGrpcClient;
    }

    public async Task<GetOrderResponse?> Handle(GetOrderQuery request, CancellationToken cancellationToken)
    {
        var order = await _repository.GetByIdAsync(request.Id);
        
        if (order == null)
            return null;

        // Lấy thông tin product từ Product Service qua gRPC
        var product = await _productGrpcClient.GetProductAsync(order.ProductId);
        
        // Sử dụng thông tin từ gRPC nếu có, nếu không thì dùng cached data
        var productName = product?.Name ?? order.ProductName;
        var productPrice = product?.Price ?? order.ProductPrice;

        return new GetOrderResponse(
            order.Id,
            order.CustomerName,
            order.CustomerEmail,
            order.ProductId,
            productName,
            productPrice,
            order.Quantity,
            order.TotalAmount,
            order.Status,
            order.CreatedAt
        );
    }
}