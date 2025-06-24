using MediatR;
using Application.Common.Interfaces;

namespace Application.Features.Orders.Queries.GetOrder;

public class GetOrderHandler : IRequestHandler<GetOrderQuery, GetOrderResponse?>
{
    private readonly IOrderRepository _repository;

    public GetOrderHandler(IOrderRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetOrderResponse?> Handle(GetOrderQuery request, CancellationToken cancellationToken)
    {
        var order = await _repository.GetByIdAsync(request.Id);
        
        if (order == null)
            return null;

        return new GetOrderResponse(
            order.Id,
            order.CustomerName,
            order.CustomerEmail,
            order.ProductId,
            order.ProductName,
            order.ProductPrice,
            order.Quantity,
            order.TotalAmount,
            order.Status,
            order.CreatedAt
        );
    }
}