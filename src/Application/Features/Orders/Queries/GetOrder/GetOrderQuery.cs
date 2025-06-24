using MediatR;

namespace Application.Features.Orders.Queries.GetOrder;

public record GetOrderQuery(int Id) : IRequest<GetOrderResponse?>;

public record GetOrderResponse(
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