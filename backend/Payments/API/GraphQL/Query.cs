using Payments.Data;

namespace Payments.API.GraphQL;

public class Query
{
    [GraphQLName("getPayment")]
    public async Task<Payment?> GetPayment(string id, [Service] PaymentsRepository repository)
    {
        return await repository.GetByIdAsync(id);
    }

    [GraphQLName("getPayments")]
    public async Task<List<Payment>> GetPayments(string orderId, [Service] PaymentsRepository repository)
    {
        return await repository.GetByOrderAsync(orderId);
    }
}
