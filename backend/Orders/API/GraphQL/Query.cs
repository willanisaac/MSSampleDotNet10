using Orders.Data;

namespace Orders.API.GraphQL;

public class Query
{
    [GraphQLName("getOrder")]
    public async Task<Order?> GetOrder(string id, [Service] OrdersRepository repository)
    {
        return await repository.GetByIdAsync(id);
    }

    [GraphQLName("getOrders")]
    public async Task<List<Order>> GetOrders(string userId, [Service] OrdersRepository repository)
    {
        return await repository.GetByUserAsync(userId);
    }
}
