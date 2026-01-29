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

public class PaymentsGraphQLClient : IPaymentsGraphQLClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<PaymentsGraphQLClient> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public PaymentsGraphQLClient(HttpClient httpClient, ILogger<PaymentsGraphQLClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }

    public async Task<PaymentGraphQLResponse?> GetPaymentByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var request = new GraphQLRequest
        {
            Query = PaymentQueries.GetPaymentById,
            Variables = new { id }
        };

        return await ExecuteQueryAsync(request, cancellationToken);
    }

    public async Task<PaymentGraphQLResponse?> GetPaymentsByOrderAsync(string orderId, CancellationToken cancellationToken = default)
    {
        var request = new GraphQLRequest
        {
            Query = PaymentQueries.GetPaymentsByOrder,
            Variables = new { orderId }
        };

        return await ExecuteQueryAsync(request, cancellationToken);
    }

    public async Task<PaymentGraphQLResponse?> ProcessPaymentAsync(object input, CancellationToken cancellationToken = default)
    {
        var request = new GraphQLRequest
        {
            Query = PaymentMutations.ProcessPayment,
            Variables = new { input }
        };

        return await ExecuteQueryAsync(request, cancellationToken);
    }

    public async Task<PaymentGraphQLResponse?> RefundPaymentAsync(string id, CancellationToken cancellationToken = default)
    {
        var request = new GraphQLRequest
        {
            Query = PaymentMutations.RefundPayment,
            Variables = new { id }
        };

        return await ExecuteQueryAsync(request, cancellationToken);
    }

    private async Task<PaymentGraphQLResponse?> ExecuteQueryAsync(GraphQLRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("graphql", request, _jsonOptions, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<PaymentGraphQLResponse>(_jsonOptions, cancellationToken);

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
            _logger.LogError(ex, "HTTP request failed while calling Payments GraphQL service");
            throw new ServiceUnavailableException("Payments Service", ex);
        }
        catch (GraphQLClientException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while calling Payments GraphQL service");
            throw new ServiceUnavailableException("Payments Service", ex);
        }
    }
}
