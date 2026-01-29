namespace BFF.Infrastructure.GraphQL.Queries;

public static class UserQueries
{
    public const string GetUserById = @"
        query GetUser($id: ID!) {
            user(id: $id) {
                id
                name
                email
                phone
                role
                isActive
                createdAt
            }
        }";

    public const string GetAllUsers = @"
        query GetAllUsers {
            users {
                id
                name
                email
                phone
                role
                isActive
                createdAt
            }
        }";
}
