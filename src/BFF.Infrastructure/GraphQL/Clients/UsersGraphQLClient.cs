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

public class UsersGraphQLClient : IUsersGraphQLClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<UsersGraphQLClient> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public UsersGraphQLClient(HttpClient httpClient, ILogger<UsersGraphQLClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }

    public async Task<UserGraphQLResponse?> GetUserByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var request = new GraphQLRequest
        {
            Query = UserQueries.GetUserById,
            Variables = new { id }
        };

        return await ExecuteQueryAsync(request, cancellationToken);
    }

    public async Task<UserGraphQLResponse?> GetAllUsersAsync(CancellationToken cancellationToken = default)
    {
        var request = new GraphQLRequest
        {
            Query = UserQueries.GetAllUsers
        };

        return await ExecuteQueryAsync(request, cancellationToken);
    }

    public async Task<UserGraphQLResponse?> CreateUserAsync(object input, CancellationToken cancellationToken = default)
    {
        var request = new GraphQLRequest
        {
            Query = UserMutations.CreateUser,
            Variables = new { input }
        };

        return await ExecuteQueryAsync(request, cancellationToken);
    }

    public async Task<UserGraphQLResponse?> UpdateUserAsync(string id, object input, CancellationToken cancellationToken = default)
    {
        var request = new GraphQLRequest
        {
            Query = UserMutations.UpdateUser,
            Variables = new { id, input }
        };

        return await ExecuteQueryAsync(request, cancellationToken);
    }

    public async Task<bool> DeleteUserAsync(string id, CancellationToken cancellationToken = default)
    {
        var request = new GraphQLRequest
        {
            Query = UserMutations.DeleteUser,
            Variables = new { id }
        };

        var response = await ExecuteQueryAsync(request, cancellationToken);
        return response?.Errors == null || response.Errors.Count == 0;
    }

    private async Task<UserGraphQLResponse?> ExecuteQueryAsync(GraphQLRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("graphql", request, _jsonOptions, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<UserGraphQLResponse>(_jsonOptions, cancellationToken);

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
            _logger.LogError(ex, "HTTP request failed while calling Users GraphQL service");
            throw new ServiceUnavailableException("Users Service", ex);
        }
        catch (GraphQLClientException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while calling Users GraphQL service");
            throw new ServiceUnavailableException("Users Service", ex);
        }
    }
}
