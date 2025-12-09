using MediatR;

namespace Application.Features.Orders.Commands.UpdateOrderStatus;

public record UpdateOrderStatusCommand(
    int Id,
    string Status
) : IRequest<UpdateOrderStatusResponse>;

public record UpdateOrderStatusResponse(
    int Id,
    string Status,
    DateTime? UpdatedAt
);
