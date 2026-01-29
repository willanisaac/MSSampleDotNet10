namespace BFF.Infrastructure.GraphQL.Queries;

public static class OrderQueries
{
    public const string GetOrderById = @"
        query GetOrder($id: ID!) {
            order(id: $id) {
                id
                userId
                status
                totalAmount
                items {
                    productId
                    productName
                    quantity
                    price
                    subtotal
                }
                createdAt
            }
        }";

    public const string GetOrdersByUser = @"
        query GetOrdersByUser($userId: ID!) {
            orders(userId: $userId) {
                id
                userId
                status
                totalAmount
                items {
                    productId
                    productName
                    quantity
                    price
                    subtotal
                }
                createdAt
            }
        }";
}
