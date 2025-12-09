using MediatR;
using Application.Common.Interfaces;

namespace Application.Features.Orders.Commands.UpdateOrderStatus;

public class UpdateOrderStatusHandler : IRequestHandler<UpdateOrderStatusCommand, UpdateOrderStatusResponse>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductServiceClient _productServiceClient;
    private readonly INotificationService _notificationService;

    private static readonly string[] ValidStatuses = { "Pending", "Processing", "Shipped", "Completed", "Cancelled" };

    public UpdateOrderStatusHandler(
        IOrderRepository orderRepository,
        IProductServiceClient productServiceClient,
        INotificationService notificationService)
    {
        _orderRepository = orderRepository;
        _productServiceClient = productServiceClient;
        _notificationService = notificationService;
    }

    public async Task<UpdateOrderStatusResponse> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        // Validate status
        if (!ValidStatuses.Contains(request.Status))
        {
            throw new Exception($"Invalid status '{request.Status}'. Valid statuses are: {string.Join(", ", ValidStatuses)}");
        }

        var order = await _orderRepository.GetByIdAsync(request.Id);
        
        if (order == null)
        {
            throw new Exception($"Order with ID {request.Id} not found");
        }

        var oldStatus = order.Status;

        // If order is being cancelled, restore stock
        if (request.Status == "Cancelled" && oldStatus != "Cancelled")
        {
            await _productServiceClient.UpdateStockAsync(order.ProductId, order.Quantity);
        }

        order.Status = request.Status;
        var updatedOrder = await _orderRepository.UpdateAsync(order);

        // Send notification for status change
        await _notificationService.SendNotificationAsync(new NotificationEvent
        {
            EventName = "ORDER_STATUS_UPDATED",
            Message = $"Order #{order.Id} status changed from {oldStatus} to {request.Status}",
            Data = new Dictionary<string, object>
            {
                { "orderId", order.Id },
                { "customerName", order.CustomerName },
                { "oldStatus", oldStatus },
                { "newStatus", request.Status },
                { "updatedAt", updatedOrder.UpdatedAt ?? DateTime.UtcNow }
            }
        });

        return new UpdateOrderStatusResponse(
            updatedOrder.Id,
            updatedOrder.Status,
            updatedOrder.UpdatedAt
        );
    }
}
