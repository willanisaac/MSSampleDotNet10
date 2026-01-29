using BFF.Application.DTOs.GraphQL;

namespace BFF.Application.Interfaces;

public interface IUsersGraphQLClient
{
    Task<UserGraphQLResponse?> GetUserByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<UserGraphQLResponse?> GetAllUsersAsync(CancellationToken cancellationToken = default);
    Task<UserGraphQLResponse?> CreateUserAsync(object input, CancellationToken cancellationToken = default);
    Task<UserGraphQLResponse?> UpdateUserAsync(string id, object input, CancellationToken cancellationToken = default);
    Task<bool> DeleteUserAsync(string id, CancellationToken cancellationToken = default);
}
