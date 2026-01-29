using BFF.Application.DTOs.Requests;
using BFF.Application.DTOs.Responses;

namespace BFF.Application.Services.Interfaces;

public interface IUserService
{
    Task<UserResponse?> GetUserByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<List<UserResponse>> GetAllUsersAsync(CancellationToken cancellationToken = default);
    Task<UserResponse> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default);
    Task<UserResponse?> UpdateUserAsync(string id, UpdateUserRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteUserAsync(string id, CancellationToken cancellationToken = default);
}
