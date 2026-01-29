using AutoMapper;
using BFF.Application.DTOs.Requests;
using BFF.Application.DTOs.Responses;
using BFF.Application.Interfaces;
using BFF.Application.Services.Interfaces;

namespace BFF.Application.Services;

public class OrderService : IOrderService
{
    private readonly IOrdersGraphQLClient _graphQLClient;
    private readonly IMapper _mapper;

    public OrderService(IOrdersGraphQLClient graphQLClient, IMapper mapper)
    {
        _graphQLClient = graphQLClient;
        _mapper = mapper;
    }

    public async Task<OrderResponse?> GetOrderByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var response = await _graphQLClient.GetOrderByIdAsync(id, cancellationToken);
        
        if (response?.Data?.Order == null)
        {
            return null;
        }

        return _mapper.Map<OrderResponse>(response.Data.Order);
    }

    public async Task<List<OrderResponse>> GetOrdersByUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        var response = await _graphQLClient.GetOrdersByUserAsync(userId, cancellationToken);
        
        if (response?.Data?.Orders == null || response.Data.Orders.Count == 0)
        {
            return new List<OrderResponse>();
        }

        return _mapper.Map<List<OrderResponse>>(response.Data.Orders);
    }

    public async Task<OrderResponse> CreateOrderAsync(CreateOrderRequest request, CancellationToken cancellationToken = default)
    {
        var input = new
        {
            userId = request.UserId,
            items = request.Items.Select(i => new
            {
                productId = i.ProductId,
                productName = i.ProductName,
                quantity = i.Quantity,
                price = i.Price
            }).ToList()
        };

        var response = await _graphQLClient.CreateOrderAsync(input, cancellationToken);
        
        if (response?.Data?.Order == null)
        {
            throw new InvalidOperationException("Failed to create order");
        }

        return _mapper.Map<OrderResponse>(response.Data.Order);
    }

    public async Task<OrderResponse?> CancelOrderAsync(string id, CancellationToken cancellationToken = default)
    {
        var response = await _graphQLClient.CancelOrderAsync(id, cancellationToken);
        
        if (response?.Data?.Order == null)
        {
            return null;
        }

        return _mapper.Map<OrderResponse>(response.Data.Order);
    }
}
