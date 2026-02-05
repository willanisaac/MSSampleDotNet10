namespace BFF.Infrastructure.GraphQL.Queries;

public static class OrderQueries
{
    public const string GetOrderById = @"
        query GetOrder($id: String!) {
            getOrder(id: $id) {
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
        query GetOrdersByUser($userId: String!) {
            getOrders(userId: $userId) {
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
