using Users.Data;

namespace Users.API.GraphQL;

public class Query
{
    [GraphQLName("getUser")]
    public async Task<User?> GetUser(string id, [Service] UsersRepository repository)
    {
        return await repository.GetByIdAsync(id);
    }

    [GraphQLName("getUsers")]
    public async Task<List<User>> GetUsers([Service] UsersRepository repository)
    {
        return await repository.GetAllAsync();
    }
}
