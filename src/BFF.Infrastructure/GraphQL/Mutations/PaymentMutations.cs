namespace BFF.Infrastructure.GraphQL.Mutations;

public static class PaymentMutations
{
    public const string ProcessPayment = @"
        mutation ProcessPayment($input: ProcessPaymentInput!) {
            processPayment(input: $input) {
                id
                orderId
                amount
                status
                paymentMethod
                transactionId
                createdAt
            }
        }";

    public const string RefundPayment = @"
        mutation RefundPayment($id: String!) {
            refundPayment(id: $id) {
                id
                orderId
                amount
                status
                paymentMethod
                transactionId
                createdAt
            }
        }";
}
