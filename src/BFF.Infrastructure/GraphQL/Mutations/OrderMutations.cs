namespace BFF.Infrastructure.GraphQL.Mutations;

public static class OrderMutations
{
    public const string CreateOrder = @"
        mutation CreateOrder($input: CreateOrderInput!) {
            createOrder(input: $input) {
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

    public const string CancelOrder = @"
        mutation CancelOrder($id: ID!) {
            cancelOrder(id: $id) {
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
