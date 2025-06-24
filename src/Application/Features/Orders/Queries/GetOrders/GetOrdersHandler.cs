using MediatR;
using Application.Common.Interfaces;

namespace Application.Features.Orders.Queries.GetOrders;

public class GetOrdersHandler : IRequestHandler<GetOrdersQuery, IEnumerable<GetOrdersResponse>>
{
    private readonly IOrderRepository _repository;

    public GetOrdersHandler(IOrderRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<GetOrdersResponse>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await _repository.GetAllAsync();
        
        return orders.Select(o => new GetOrdersResponse(
            o.Id,
            o.CustomerName,
            o.ProductName,
            o.Quantity,
            o.TotalAmount,
            o.Status,
            o.CreatedAt
        ));
    }
}