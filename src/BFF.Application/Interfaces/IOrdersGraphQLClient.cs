using BFF.Application.DTOs.GraphQL;

namespace BFF.Application.Interfaces;

public interface IOrdersGraphQLClient
{
    Task<OrderGraphQLResponse?> GetOrderByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<OrderGraphQLResponse?> GetOrdersByUserAsync(string userId, CancellationToken cancellationToken = default);
    Task<OrderGraphQLResponse?> CreateOrderAsync(object input, CancellationToken cancellationToken = default);
    Task<OrderGraphQLResponse?> CancelOrderAsync(string id, CancellationToken cancellationToken = default);
}
