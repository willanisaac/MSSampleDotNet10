namespace BFF.Infrastructure.GraphQL.Queries;

public static class PaymentQueries
{
    public const string GetPaymentById = @"
        query GetPayment($id: ID!) {
            payment(id: $id) {
                id
                orderId
                amount
                status
                paymentMethod
                transactionId
                createdAt
            }
        }";

    public const string GetPaymentsByOrder = @"
        query GetPaymentsByOrder($orderId: ID!) {
            payments(orderId: $orderId) {
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
