using MediatR;

namespace Application.Features.Orders.Queries.GetOrders;

public record GetOrdersQuery() : IRequest<IEnumerable<GetOrdersResponse>>;

public record GetOrdersResponse(
    int Id,
    string CustomerName,
    string ProductName,
    int Quantity,
    decimal TotalAmount,
    string Status,
    DateTime CreatedAt
);