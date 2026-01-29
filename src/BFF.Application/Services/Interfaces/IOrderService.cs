using BFF.Application.DTOs.Requests;
using BFF.Application.DTOs.Responses;

namespace BFF.Application.Services.Interfaces;

public interface IOrderService
{
    Task<OrderResponse?> GetOrderByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<List<OrderResponse>> GetOrdersByUserAsync(string userId, CancellationToken cancellationToken = default);
    Task<OrderResponse> CreateOrderAsync(CreateOrderRequest request, CancellationToken cancellationToken = default);
    Task<OrderResponse?> CancelOrderAsync(string id, CancellationToken cancellationToken = default);
}
