using Users.Data;

namespace Users.API.GraphQL;

public class Mutation
{
    [GraphQLName("createUser")]
    public async Task<User> CreateUser(CreateUserInput input, [Service] UsersRepository repository)
    {
        if (string.IsNullOrWhiteSpace(input.Name) || string.IsNullOrWhiteSpace(input.Email))
        {
            throw new GraphQLException("Name and Email are required");
        }

        return await repository.AddAsync(input);
    }

    [GraphQLName("updateUser")]
    public async Task<User?> UpdateUser(string id, UpdateUserInput input, [Service] UsersRepository repository)
    {
        var updated = await repository.UpdateAsync(id, input);
        
        if (updated == null)
        {
            throw new GraphQLException("User not found");
        }

        return updated;
    }

    [GraphQLName("deleteUser")]
    public async Task<bool> DeleteUser(string id, [Service] UsersRepository repository)
    {
        var deleted = await repository.DeleteAsync(id);
        
        if (!deleted)
        {
            throw new GraphQLException("User not found");
        }

        return true;
    }
}
