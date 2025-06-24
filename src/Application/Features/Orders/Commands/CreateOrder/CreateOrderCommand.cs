using MediatR;

namespace Application.Features.Orders.Commands.CreateOrder;

public record CreateOrderCommand(
    string CustomerName,
    string CustomerEmail,
    int ProductId,
    int Quantity
) : IRequest<CreateOrderResponse>;

public record CreateOrderResponse(
    int Id,
    string CustomerName,
    string CustomerEmail,
    int ProductId,
    string ProductName,
    decimal ProductPrice,
    int Quantity,
    decimal TotalAmount,
    string Status,
    DateTime CreatedAt
);