using MediatR;
using Application.Common.Interfaces;

namespace Application.Features.Orders.Commands.UpdateOrder;

public class UpdateOrderHandler : IRequestHandler<UpdateOrderCommand, UpdateOrderResponse>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductServiceClient _productServiceClient;

    public UpdateOrderHandler(
        IOrderRepository orderRepository,
        IProductServiceClient productServiceClient)
    {
        _orderRepository = orderRepository;
        _productServiceClient = productServiceClient;
    }

    public async Task<UpdateOrderResponse> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.Id);
        
        if (order == null)
        {
            throw new Exception($"Order with ID {request.Id} not found");
        }

        // Check if product changed or quantity increased
        var quantityDiff = request.Quantity - order.Quantity;
        var productChanged = request.ProductId != order.ProductId;
        
        // Get product info
        var product = await _productServiceClient.GetProductAsync(request.ProductId);
        if (product == null)
        {
            throw new Exception($"Product with ID {request.ProductId} not found");
        }

        // Check stock if product changed or quantity increased
        if (productChanged || quantityDiff > 0)
        {
            var requiredQuantity = productChanged ? request.Quantity : quantityDiff;
            var stockCheck = await _productServiceClient.CheckStockAsync(request.ProductId, requiredQuantity);
            
            if (!stockCheck.IsAvailable)
            {
                throw new Exception(stockCheck.Message);
            }
        }

        // If product changed, restore old product stock
        if (productChanged)
        {
            await _productServiceClient.UpdateStockAsync(order.ProductId, order.Quantity);
            // Decrease new product stock
            await _productServiceClient.UpdateStockAsync(request.ProductId, -request.Quantity);
        }
        else if (quantityDiff != 0)
        {
            // Update stock difference
            await _productServiceClient.UpdateStockAsync(request.ProductId, -quantityDiff);
        }

        // Update order
        order.CustomerName = request.CustomerName;
        order.CustomerEmail = request.CustomerEmail;
        order.ProductId = request.ProductId;
        order.ProductName = product.Name;
        order.ProductPrice = product.Price;
        order.Quantity = request.Quantity;
        order.TotalAmount = product.Price * request.Quantity;

        var updatedOrder = await _orderRepository.UpdateAsync(order);

        return new UpdateOrderResponse(
            updatedOrder.Id,
            updatedOrder.CustomerName,
            updatedOrder.CustomerEmail,
            updatedOrder.ProductId,
            updatedOrder.ProductName,
            updatedOrder.ProductPrice,
            updatedOrder.Quantity,
            updatedOrder.TotalAmount,
            updatedOrder.Status,
            updatedOrder.CreatedAt,
            updatedOrder.UpdatedAt
        );
    }
}
