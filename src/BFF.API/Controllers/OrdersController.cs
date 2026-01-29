using BFF.Application.DTOs.Requests;
using BFF.Application.Services.Interfaces;
using BFF.Domain.Constants;
using Microsoft.AspNetCore.Mvc;

namespace BFF.API.Controllers;

[ApiController]
[Route(ApiRoutes.Orders.Base)]
[Produces("application/json")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(IOrderService orderService, ILogger<OrdersController> logger)
    {
        _orderService = orderService;
        _logger = logger;
    }

    /// <summary>
    /// Get order by ID
    /// </summary>
    [HttpGet(ApiRoutes.Orders.GetById)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOrderById(string id, CancellationToken cancellationToken)
    {
        var order = await _orderService.GetOrderByIdAsync(id, cancellationToken);
        
        if (order == null)
        {
            return NotFound(new { message = $"Order with id '{id}' not found" });
        }

        return Ok(order);
    }

    /// <summary>
    /// Get orders by user ID
    /// </summary>
    [HttpGet(ApiRoutes.Orders.GetByUser)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOrdersByUser(string userId, CancellationToken cancellationToken)
    {
        var orders = await _orderService.GetOrdersByUserAsync(userId, cancellationToken);
        return Ok(orders);
    }

    /// <summary>
    /// Create a new order
    /// </summary>
    [HttpPost(ApiRoutes.Orders.Create)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request, CancellationToken cancellationToken)
    {
        var order = await _orderService.CreateOrderAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order);
    }

    /// <summary>
    /// Cancel an order
    /// </summary>
    [HttpPost(ApiRoutes.Orders.Cancel)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelOrder(string id, CancellationToken cancellationToken)
    {
        var order = await _orderService.CancelOrderAsync(id, cancellationToken);
        
        if (order == null)
        {
            return NotFound(new { message = $"Order with id '{id}' not found" });
        }

        return Ok(order);
    }
}
