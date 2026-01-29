using AutoMapper;
using BFF.Application.DTOs.Requests;
using BFF.Application.DTOs.Responses;
using BFF.Application.Interfaces;
using BFF.Application.Services.Interfaces;

namespace BFF.Application.Services;

public class UserService : IUserService
{
    private readonly IUsersGraphQLClient _graphQLClient;
    private readonly IMapper _mapper;

    public UserService(IUsersGraphQLClient graphQLClient, IMapper mapper)
    {
        _graphQLClient = graphQLClient;
        _mapper = mapper;
    }

    public async Task<UserResponse?> GetUserByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var response = await _graphQLClient.GetUserByIdAsync(id, cancellationToken);
        
        if (response?.Data?.User == null)
        {
            return null;
        }

        return _mapper.Map<UserResponse>(response.Data.User);
    }

    public async Task<List<UserResponse>> GetAllUsersAsync(CancellationToken cancellationToken = default)
    {
        var response = await _graphQLClient.GetAllUsersAsync(cancellationToken);
        
        if (response?.Data?.Users == null || response.Data.Users.Count == 0)
        {
            return new List<UserResponse>();
        }

        return _mapper.Map<List<UserResponse>>(response.Data.Users);
    }

    public async Task<UserResponse> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        var input = new
        {
            name = request.Name,
            email = request.Email,
            phone = request.Phone
        };

        var response = await _graphQLClient.CreateUserAsync(input, cancellationToken);
        
        if (response?.Data?.User == null)
        {
            throw new InvalidOperationException("Failed to create user");
        }

        return _mapper.Map<UserResponse>(response.Data.User);
    }

    public async Task<UserResponse?> UpdateUserAsync(string id, UpdateUserRequest request, CancellationToken cancellationToken = default)
    {
        var input = new
        {
            name = request.Name,
            email = request.Email,
            phone = request.Phone
        };

        var response = await _graphQLClient.UpdateUserAsync(id, input, cancellationToken);
        
        if (response?.Data?.User == null)
        {
            return null;
        }

        return _mapper.Map<UserResponse>(response.Data.User);
    }

    public async Task<bool> DeleteUserAsync(string id, CancellationToken cancellationToken = default)
    {
        var result = await _graphQLClient.DeleteUserAsync(id, cancellationToken);
        return result;
    }
}
