using MediatR;
using Microsoft.AspNetCore.Mvc;
using Application.Features.Orders.Commands.CreateOrder;
using Application.Features.Orders.Commands.UpdateOrder;
using Application.Features.Orders.Commands.UpdateOrderStatus;
using Application.Features.Orders.Commands.DeleteOrder;
using Application.Features.Orders.Queries.GetOrder;
using Application.Features.Orders.Queries.GetOrders;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GetOrdersResponse>>> GetOrders()
    {
        var query = new GetOrdersQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetOrderResponse>> GetOrder(int id)
    {
        var query = new GetOrderQuery(id);
        var result = await _mediator.Send(query);
        
        if (result == null)
            return NotFound();
            
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<CreateOrderResponse>> CreateOrder(CreateOrderCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetOrder), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<UpdateOrderResponse>> UpdateOrder(int id, UpdateOrderCommand command)
    {
        if (id != command.Id)
            return BadRequest("ID mismatch");

        try
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPatch("{id}/status")]
    public async Task<ActionResult<UpdateOrderStatusResponse>> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusRequest request)
    {
        try
        {
            var command = new UpdateOrderStatusCommand(id, request.Status);
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<DeleteOrderResponse>> DeleteOrder(int id)
    {
        var command = new DeleteOrderCommand(id);
        var result = await _mediator.Send(command);
        
        if (!result.Success)
            return NotFound(new { message = result.Message });
            
        return Ok(result);
    }
}

public record UpdateOrderStatusRequest(string Status);