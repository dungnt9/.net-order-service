using MediatR;
using Application.Common.Interfaces;
using Domain.Entities;

namespace Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, CreateOrderResponse>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductServiceClient _productServiceClient;
    private readonly INotificationService _notificationService;

    public CreateOrderHandler(
        IOrderRepository orderRepository, 
        IProductServiceClient productServiceClient,
        INotificationService notificationService)
    {
        _orderRepository = orderRepository;
        _productServiceClient = productServiceClient;
        _notificationService = notificationService;
    }

    public async Task<CreateOrderResponse> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        // Gọi sang ProductService để lấy thông tin sản phẩm
        var product = await _productServiceClient.GetProductAsync(request.ProductId);
        
        if (product == null)
        {
            throw new Exception($"Product with ID {request.ProductId} not found");
        }

        // Check stock via gRPC
        var stockCheck = await _productServiceClient.CheckStockAsync(request.ProductId, request.Quantity);
        if (!stockCheck.IsAvailable)
        {
            throw new Exception(stockCheck.Message);
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

        // Update stock via gRPC (decrease by quantity)
        var stockUpdate = await _productServiceClient.UpdateStockAsync(request.ProductId, -request.Quantity);
        if (!stockUpdate.Success)
        {
            // Rollback order if stock update fails
            await _orderRepository.DeleteAsync(createdOrder.Id);
            throw new Exception($"Failed to update stock: {stockUpdate.Message}");
        }
        
        await _notificationService.SendNotificationAsync(new NotificationEvent
        {
            EventName = "ORDER_CREATED",
            Message = $"New order #{createdOrder.Id} has been created",
            Data = new Dictionary<string, object>
            {
                { "orderId", createdOrder.Id },
                { "customerName", createdOrder.CustomerName },
                { "productName", createdOrder.ProductName },
                { "totalAmount", createdOrder.TotalAmount },
                { "createdAt", createdOrder.CreatedAt }
            }
        });

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
