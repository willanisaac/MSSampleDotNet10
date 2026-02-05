namespace BFF.Infrastructure.GraphQL.Queries;

public static class UserQueries
{
    public const string GetUserById = @"
        query GetUser($id: String!) {
            getUser(id: $id) {
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
            getUsers {
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
