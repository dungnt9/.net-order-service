using MediatR;
using Application.Common.Interfaces;
using Domain.Entities;

namespace Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, CreateOrderResponse>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductServiceClient _productServiceClient;

    public CreateOrderHandler(IOrderRepository orderRepository, IProductServiceClient productServiceClient)
    {
        _orderRepository = orderRepository;
        _productServiceClient = productServiceClient;
    }

    public async Task<CreateOrderResponse> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        // Gọi sang ProductService để lấy thông tin sản phẩm
        var product = await _productServiceClient.GetProductAsync(request.ProductId);
        
        if (product == null)
        {
            throw new Exception($"Product with ID {request.ProductId} not found");
        }

        if (product.Stock < request.Quantity)
        {
            throw new Exception($"Insufficient stock. Available: {product.Stock}, Requested: {request.Quantity}");
        }

        var totalAmount = product.Price * request.Quantity;

        var order = new Order
        {
            CustomerName = request.CustomerName,
            CustomerEmail = request.CustomerEmail,
            ProductId = request.ProductId,
            ProductName = product.Name, // Cache thông tin từ ProductService
            ProductPrice = product.Price,
            Quantity = request.Quantity,
            TotalAmount = totalAmount,
            Status = "Pending"
        };

        var createdOrder = await _orderRepository.CreateAsync(order);

        return new CreateOrderResponse(
            createdOrder.Id,
            createdOrder.CustomerName,
            createdOrder.CustomerEmail,
            createdOrder.ProductId,
            createdOrder.ProductName,
            createdOrder.ProductPrice,
            createdOrder.Quantity,
            createdOrder.TotalAmount,
            createdOrder.Status,
            createdOrder.CreatedAt
        );
    }
}
