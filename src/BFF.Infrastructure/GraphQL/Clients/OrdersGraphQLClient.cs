using System.Net.Http.Json;
using System.Text.Json;
using BFF.Application.DTOs.GraphQL;
using BFF.Application.Exceptions;
using BFF.Application.Interfaces;
using BFF.Infrastructure.GraphQL.Models;
using BFF.Infrastructure.GraphQL.Mutations;
using BFF.Infrastructure.GraphQL.Queries;
using Microsoft.Extensions.Logging;

namespace BFF.Infrastructure.GraphQL.Clients;

public class OrdersGraphQLClient : IOrdersGraphQLClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<OrdersGraphQLClient> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public OrdersGraphQLClient(HttpClient httpClient, ILogger<OrdersGraphQLClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }

    public async Task<OrderGraphQLResponse?> GetOrderByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var request = new GraphQLRequest
        {
            Query = OrderQueries.GetOrderById,
            Variables = new { id }
        };

        return await ExecuteQueryAsync(request, cancellationToken);
    }

    public async Task<OrderGraphQLResponse?> GetOrdersByUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        var request = new GraphQLRequest
        {
            Query = OrderQueries.GetOrdersByUser,
            Variables = new { userId }
        };

        return await ExecuteQueryAsync(request, cancellationToken);
    }

    public async Task<OrderGraphQLResponse?> CreateOrderAsync(object input, CancellationToken cancellationToken = default)
    {
        var request = new GraphQLRequest
        {
            Query = OrderMutations.CreateOrder,
            Variables = new { input }
        };

        return await ExecuteQueryAsync(request, cancellationToken);
    }

    public async Task<OrderGraphQLResponse?> CancelOrderAsync(string id, CancellationToken cancellationToken = default)
    {
        var request = new GraphQLRequest
        {
            Query = OrderMutations.CancelOrder,
            Variables = new { id }
        };

        return await ExecuteQueryAsync(request, cancellationToken);
    }

    private async Task<OrderGraphQLResponse?> ExecuteQueryAsync(GraphQLRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("graphql", request, _jsonOptions, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<OrderGraphQLResponse>(_jsonOptions, cancellationToken);

            if (result?.Errors != null && result.Errors.Count > 0)
            {
                _logger.LogWarning("GraphQL query returned errors: {Errors}", 
                    JsonSerializer.Serialize(result.Errors, _jsonOptions));
                throw new GraphQLClientException("GraphQL query returned errors", result.Errors);
            }

            return result;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request failed while calling Orders GraphQL service");
            throw new ServiceUnavailableException("Orders Service", ex);
        }
        catch (GraphQLClientException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while calling Orders GraphQL service");
            throw new ServiceUnavailableException("Orders Service", ex);
        }
    }
}
