using BFF.Application.DTOs.GraphQL;

namespace BFF.Application.Interfaces;

public interface IPaymentsGraphQLClient
{
    Task<PaymentGraphQLResponse?> GetPaymentByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<PaymentGraphQLResponse?> GetPaymentsByOrderAsync(string orderId, CancellationToken cancellationToken = default);
    Task<PaymentGraphQLResponse?> ProcessPaymentAsync(object input, CancellationToken cancellationToken = default);
    Task<PaymentGraphQLResponse?> RefundPaymentAsync(string id, CancellationToken cancellationToken = default);
}
