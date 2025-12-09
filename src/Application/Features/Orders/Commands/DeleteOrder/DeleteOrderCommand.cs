using MediatR;

namespace Application.Features.Orders.Commands.DeleteOrder;

public record DeleteOrderCommand(int Id) : IRequest<DeleteOrderResponse>;

public record DeleteOrderResponse(
    bool Success,
    string Message
);
