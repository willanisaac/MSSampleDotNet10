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
        mutation UpdateUser($id: String!, $input: UpdateUserInput!) {
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
        mutation DeleteUser($id: String!) {
            deleteUser(id: $id)
        }";
}
