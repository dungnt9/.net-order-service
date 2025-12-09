using MediatR;
using Application.Common.Interfaces;

namespace Application.Features.Orders.Commands.DeleteOrder;

public class DeleteOrderHandler : IRequestHandler<DeleteOrderCommand, DeleteOrderResponse>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductServiceClient _productServiceClient;

    public DeleteOrderHandler(
        IOrderRepository orderRepository,
        IProductServiceClient productServiceClient)
    {
        _orderRepository = orderRepository;
        _productServiceClient = productServiceClient;
    }

    public async Task<DeleteOrderResponse> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.Id);
        
        if (order == null)
        {
            return new DeleteOrderResponse(false, $"Order with ID {request.Id} not found");
        }

        // Restore stock if order is not already cancelled
        if (order.Status != "Cancelled")
        {
            await _productServiceClient.UpdateStockAsync(order.ProductId, order.Quantity);
        }

        await _orderRepository.DeleteAsync(request.Id);

        return new DeleteOrderResponse(true, $"Order #{request.Id} deleted successfully");
    }
}
