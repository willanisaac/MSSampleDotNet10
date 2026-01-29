namespace BFF.Infrastructure.GraphQL.Mutations;

public static class UserMutations
{
    public const string CreateUser = @"
        mutation CreateUser($input: CreateUserInput!) {
            createUser(input: $input) {
                id
                name
                email
                phone
                role
                isActive
                createdAt
            }
        }";

    public const string UpdateUser = @"
        mutation UpdateUser($id: ID!, $input: UpdateUserInput!) {
            updateUser(id: $id, input: $input) {
                id
                name
                email
                phone
                role
                isActive
                createdAt
            }
        }";

    public const string DeleteUser = @"
        mutation DeleteUser($id: ID!) {
            deleteUser(id: $id)
        }";
}
