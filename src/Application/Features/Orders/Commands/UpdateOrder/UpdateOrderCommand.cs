using MediatR;

namespace Application.Features.Orders.Commands.UpdateOrder;

public record UpdateOrderCommand(
    int Id,
    string CustomerName,
    string CustomerEmail,
    int ProductId,
    int Quantity
) : IRequest<UpdateOrderResponse>;

public record UpdateOrderResponse(
    int Id,
    string CustomerName,
    string CustomerEmail,
    int ProductId,
    string ProductName,
    decimal ProductPrice,
    int Quantity,
    decimal TotalAmount,
    string Status,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);
