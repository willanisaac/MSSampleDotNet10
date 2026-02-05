# Guía de Uso: BFF y Microservicios

## 🚀 Inicio Rápido

**Levantar servicios backend (GraphQL + HotChocolate):**
```bash
# Terminal 1 - Users Service (puerto 5001)
dotnet run --project backend/Users/Users.Service.csproj

# Terminal 2 - Orders Service (puerto 5002)
dotnet run --project backend/Orders/Orders.Service.csproj

# Terminal 3 - Payments Service (puerto 5003)
dotnet run --project backend/Payments/Payments.Service.csproj
```

**Levantar BFF (REST):**
```bash
# Terminal 4 - BFF API (puerto 5169)
dotnet run --project src/BFF.API/BFF.API.csproj
```

**Explorador GraphQL interactivo (Banana Cake Pop):**
- Users: http://localhost:5001/graphql
- Orders: http://localhost:5002/graphql
- Payments: http://localhost:5003/graphql

---

## 📋 BFF REST API (http://localhost:5169)

### Users
**Listar todos**
```bash
curl -s http://localhost:5169/api/users | jq
```

**Obtener por ID**
```bash
curl -s http://localhost:5169/api/users/u1 | jq
```

**Crear usuario**
```bash
curl -s -X POST http://localhost:5169/api/users \
  -H "Content-Type: application/json" \
  -d '{"name":"John Doe","email":"john@example.com","phone":"+1-555-0300","role":"Customer","isActive":true}' | jq
```

**Actualizar usuario**
```bash
curl -s -X PUT http://localhost:5169/api/users/u1 \
  -H "Content-Type: application/json" \
  -d '{"name":"Alice Updated","email":"alice.new@example.com","phone":"+1-555-0999"}' | jq
```

**Eliminar usuario**
```bash
curl -s -X DELETE http://localhost:5169/api/users/u3 | jq
```

### Orders
**Listar órdenes por usuario**
```bash
curl -s http://localhost:5169/api/orders/user/u1 | jq
```

**Obtener orden por ID**
```bash
curl -s http://localhost:5169/api/orders/o1 | jq
```

**Crear orden**
```bash
curl -s -X POST http://localhost:5169/api/orders \
  -H "Content-Type: application/json" \
  -d '{"userId":"u1","items":[{"productId":"p-900","productName":"4K Monitor","quantity":1,"price":299.99}]}' | jq
```

**Cancelar orden**
```bash
curl -s -X POST http://localhost:5169/api/orders/o2/cancel | jq
```

### Payments
**Listar pagos por orden**
```bash
curl -s http://localhost:5169/api/payments/order/o1 | jq
```

**Obtener pago por ID**
```bash
curl -s http://localhost:5169/api/payments/pay-1 | jq
```

**Procesar pago**
```bash
curl -s -X POST http://localhost:5169/api/payments \
  -H "Content-Type: application/json" \
  -d '{"orderId":"o2","amount":45.00,"paymentMethod":"CreditCard"}' | jq
```

**Reembolsar pago**
```bash
curl -s -X POST http://localhost:5169/api/payments/pay-1/refund | jq
```

---

## 🔷 Microservicios GraphQL Directos (HotChocolate)

### Users Service (http://localhost:5001/graphql)

**Obtener usuario completo**
```bash
curl -s -X POST http://localhost:5001/graphql \
  -H "Content-Type: application/json" \
  -d '{"query":"query{ getUser(id:\"u1\"){ id name email phone role isActive createdAt }}"}' | jq
```

**Field selection - Solo ID y email**
```bash
curl -s -X POST http://localhost:5001/graphql \
  -H "Content-Type: application/json" \
  -d '{"query":"query{ getUser(id:\"u1\"){ id email }}"}' | jq
```

**Listar todos los usuarios (filtrado)**
```bash
curl -s -X POST http://localhost:5001/graphql \
  -H "Content-Type: application/json" \
  -d '{"query":"query{ getUsers{ id name email role }}"}' | jq
```

**Crear usuario**
```bash
curl -s -X POST http://localhost:5001/graphql \
  -H "Content-Type: application/json" \
  -d '{"query":"mutation($input:CreateUserInput!){ createUser(input:$input){ id name email createdAt }}","variables":{"input":{"name":"Jane Smith","email":"jane@example.com","phone":"+1-555-0400","role":"Customer","isActive":true}}}' | jq
```

**Actualizar usuario**
```bash
curl -s -X POST http://localhost:5001/graphql \
  -H "Content-Type: application/json" \
  -d '{"query":"mutation($id:String!,$input:UpdateUserInput!){ updateUser(id:$id,input:$input){ id name email }}","variables":{"id":"u1","input":{"name":"Alice Modified","email":"alice@updated.com","phone":"+1-555-1111"}}}' | jq
```

**Eliminar usuario**
```bash
curl -s -X POST http://localhost:5001/graphql \
  -H "Content-Type: application/json" \
  -d '{"query":"mutation($id:String!){ deleteUser(id:$id) }","variables":{"id":"u3"}}' | jq
```

### Orders Service (http://localhost:5002/graphql)

**Obtener orden por ID**
```bash
curl -s -X POST http://localhost:5002/graphql \
  -H "Content-Type: application/json" \
  -d '{"query":"query{ getOrder(id:\"o1\"){ id userId status totalAmount items{ productName quantity price subtotal } createdAt }}"}' | jq
```

**Listar órdenes por usuario (solo totales)**
```bash
curl -s -X POST http://localhost:5002/graphql \
  -H "Content-Type: application/json" \
  -d '{"query":"query{ getOrders(userId:\"u1\"){ id status totalAmount createdAt }}"}' | jq
```

**Crear orden**
```bash
curl -s -X POST http://localhost:5002/graphql \
  -H "Content-Type: application/json" \
  -d '{"query":"mutation($input:CreateOrderInput!){ createOrder(input:$input){ id userId status totalAmount items{ productId productName quantity price subtotal } createdAt }}","variables":{"input":{"userId":"u1","items":[{"productId":"p-901","productName":"Mechanical Keyboard","quantity":1,"price":149.99}]}}}' | jq
```

**Cancelar orden**
```bash
curl -s -X POST http://localhost:5002/graphql \
  -H "Content-Type: application/json" \
  -d '{"query":"mutation($id:String!){ cancelOrder(id:$id){ id status }}","variables":{"id":"o2"}}' | jq
```

### Payments Service (http://localhost:5003/graphql)

**Obtener pago por ID**
```bash
curl -s -X POST http://localhost:5003/graphql \
  -H "Content-Type: application/json" \
  -d '{"query":"query{ getPayment(id:\"pay-1\"){ id orderId amount status paymentMethod transactionId createdAt }}"}' | jq
```

**Listar pagos por orden (solo status y monto)**
```bash
curl -s -X POST http://localhost:5003/graphql \
  -H "Content-Type: application/json" \
  -d '{"query":"query{ getPayments(orderId:\"o1\"){ id amount status paymentMethod }}"}' | jq
```

**Procesar pago**
```bash
curl -s -X POST http://localhost:5003/graphql \
  -H "Content-Type: application/json" \
  -d '{"query":"mutation($input:ProcessPaymentInput!){ processPayment(input:$input){ id orderId amount status paymentMethod transactionId createdAt }}","variables":{"input":{"orderId":"o2","amount":45.00,"paymentMethod":"CreditCard"}}}' | jq
```

**Reembolsar pago**
```bash
curl -s -X POST http://localhost:5003/graphql \
  -H "Content-Type: application/json" \
  -d '{"query":"mutation($id:String!){ refundPayment(id:$id){ id status transactionId }}","variables":{"id":"pay-2"}}' | jq
```

---

## 💡 Características GraphQL

- **Field Selection**: Solicita solo los campos que necesitas
- **Introspección**: Usa Banana Cake Pop UI en el navegador para explorar el schema
- **Type Safety**: HotChocolate valida tipos automáticamente
- **Error Handling**: Errores devueltos en formato estándar GraphQL
