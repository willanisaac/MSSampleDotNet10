using Orders.Data;

namespace Orders.API.GraphQL;

public class Mutation
{
    [GraphQLName("createOrder")]
    public async Task<Order> CreateOrder(CreateOrderInput input, [Service] OrdersRepository repository)
    {
        if (string.IsNullOrWhiteSpace(input.UserId) || input.Items.Count == 0)
        {
            throw new GraphQLException("UserId and Items are required");
        }

        return await repository.AddAsync(input);
    }

    [GraphQLName("cancelOrder")]
    public async Task<Order?> CancelOrder(string id, [Service] OrdersRepository repository)
    {
        var canceled = await repository.CancelAsync(id);
        
        if (canceled == null)
        {
            throw new GraphQLException("Order not found");
        }

        return canceled;
    }
}
