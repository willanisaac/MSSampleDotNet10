## Microservicios GraphQL (HTTP) - HotChocolate

Tres servicios independientes (Users, Orders, Payments) con dos capas: API (endpoint "/graphql" + "/health") y Data (JSON persistido). Se exponen en los puertos 5001/5002/5003 usando HotChocolate para GraphQL completo con field selection automática.

### Layout
- Users: backend/Users (API/Program.cs, API/GraphQL/Query.cs, API/GraphQL/Mutation.cs, Data/UsersRepository.cs, Data/users.json)
- Orders: backend/Orders (API/Program.cs, API/GraphQL/Query.cs, API/GraphQL/Mutation.cs, Data/OrdersRepository.cs, Data/orders.json)
- Payments: backend/Payments (API/Program.cs, API/GraphQL/Query.cs, API/GraphQL/Mutation.cs, Data/PaymentsRepository.cs, Data/payments.json)

### Esquema soportado (compatibles con BFF)
- Users: getUser(id: String!), getUsers, createUser(input: CreateUserInput!), updateUser(id: String!, input: UpdateUserInput!), deleteUser(id: String!)
- Orders: getOrder(id: String!), getOrders(userId: String!), createOrder(input: CreateOrderInput!), cancelOrder(id: String!)
- Payments: getPayment(id: String!), getPayments(orderId: String!), processPayment(input: ProcessPaymentInput!), refundPayment(id: String!)

### Datos seed (coherentes)
- Users: u1 Alice (Customer), u2 Bob (Manager), u3 Carol (Admin)
- Orders: o1 (u1, Completed, total 258), o2 (u1, Pending, total 45), o3 (u2, Cancelled, total 120)
- Payments: pay-1 (o1, Completed 258), pay-2 (o2, Pending 45), pay-3 (o3, Refunded 120)

### Ejecutar
1) Restaurar (solo primera vez): dotnet restore backend/Users/Users.Service.csproj backend/Orders/Orders.Service.csproj backend/Payments/Payments.Service.csproj
2) Levantar cada servicio (una terminal por servicio):
   - dotnet run --project backend/Users/Users.Service.csproj
   - dotnet run --project backend/Orders/Orders.Service.csproj
   - dotnet run --project backend/Payments/Payments.Service.csproj

### Smoke (ejemplos con field selection)
- Users GetAll con solo id y email: POST http://localhost:5001/graphql con cuerpo { "query": "query { getUsers { id email } }" }
- Users GetUser completo: POST http://localhost:5001/graphql con { "query": "query{ getUser(id:\"u1\"){ id name email phone role isActive createdAt }}" }
- Orders create: POST http://localhost:5002/graphql con variables: { "input": { "userId": "u1", "items": [{"productId": "p-900", "productName": "Monitor", "quantity": 1, "price": 199.99}] } } y query de CreateOrder
- Payments process: POST http://localhost:5003/graphql con variables: { "input": { "orderId": "o2", "amount": 45, "paymentMethod": "CreditCard" } } y query de ProcessPayment

### Características GraphQL
- Field selection: HotChocolate devuelve solo los campos solicitados en el query
- Introspección: navega a http://localhost:5001/graphql en el browser para Banana Cake Pop UI
- Errores estándar: excepciones se convierten automáticamente en formato GraphQL errors
