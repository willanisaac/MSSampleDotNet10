using BFF.Application.DTOs.Requests;
using BFF.Application.Services.Interfaces;
using BFF.Domain.Constants;
using Microsoft.AspNetCore.Mvc;

namespace BFF.API.Controllers;

[ApiController]
[Route(ApiRoutes.Payments.Base)]
[Produces("application/json")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly ILogger<PaymentsController> _logger;

    public PaymentsController(IPaymentService paymentService, ILogger<PaymentsController> logger)
    {
        _paymentService = paymentService;
        _logger = logger;
    }

    /// <summary>
    /// Get payment by ID
    /// </summary>
    [HttpGet(ApiRoutes.Payments.GetById)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPaymentById(string id, CancellationToken cancellationToken)
    {
        var payment = await _paymentService.GetPaymentByIdAsync(id, cancellationToken);
        
        if (payment == null)
        {
            return NotFound(new { message = $"Payment with id '{id}' not found" });
        }

        return Ok(payment);
    }

    /// <summary>
    /// Get payments by order ID
    /// </summary>
    [HttpGet(ApiRoutes.Payments.GetByOrder)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPaymentsByOrder(string orderId, CancellationToken cancellationToken)
    {
        var payments = await _paymentService.GetPaymentsByOrderAsync(orderId, cancellationToken);
        return Ok(payments);
    }

    /// <summary>
    /// Process a payment
    /// </summary>
    [HttpPost(ApiRoutes.Payments.Process)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ProcessPayment([FromBody] ProcessPaymentRequest request, CancellationToken cancellationToken)
    {
        var payment = await _paymentService.ProcessPaymentAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetPaymentById), new { id = payment.Id }, payment);
    }

    /// <summary>
    /// Refund a payment
    /// </summary>
    [HttpPost(ApiRoutes.Payments.Refund)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RefundPayment(string id, CancellationToken cancellationToken)
    {
        var payment = await _paymentService.RefundPaymentAsync(id, cancellationToken);
        
        if (payment == null)
        {
            return NotFound(new { message = $"Payment with id '{id}' not found" });
        }

        return Ok(payment);
    }
}
