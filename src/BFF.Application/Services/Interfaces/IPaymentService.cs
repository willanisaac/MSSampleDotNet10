using BFF.Application.DTOs.Requests;
using BFF.Application.DTOs.Responses;

namespace BFF.Application.Services.Interfaces;

public interface IPaymentService
{
    Task<PaymentResponse?> GetPaymentByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<List<PaymentResponse>> GetPaymentsByOrderAsync(string orderId, CancellationToken cancellationToken = default);
    Task<PaymentResponse> ProcessPaymentAsync(ProcessPaymentRequest request, CancellationToken cancellationToken = default);
    Task<PaymentResponse?> RefundPaymentAsync(string id, CancellationToken cancellationToken = default);
}
