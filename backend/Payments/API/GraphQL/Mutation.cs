using Payments.Data;

namespace Payments.API.GraphQL;

public class Mutation
{
    [GraphQLName("processPayment")]
    public async Task<Payment> ProcessPayment(ProcessPaymentInput input, [Service] PaymentsRepository repository)
    {
        if (string.IsNullOrWhiteSpace(input.OrderId) || input.Amount <= 0 || string.IsNullOrWhiteSpace(input.PaymentMethod))
        {
            throw new GraphQLException("OrderId, Amount, and PaymentMethod are required");
        }

        return await repository.ProcessAsync(input);
    }

    [GraphQLName("refundPayment")]
    public async Task<Payment?> RefundPayment(string id, [Service] PaymentsRepository repository)
    {
        var refunded = await repository.RefundAsync(id);
        
        if (refunded == null)
        {
            throw new GraphQLException("Payment not found");
        }

        return refunded;
    }
}
