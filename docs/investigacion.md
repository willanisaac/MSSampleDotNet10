# 🔬 Investigación: Protocolos y Paradigmas de Comunicación

## 📡 Comparativa de Protocolos de Comunicación

---

## 1. HTTP vs gRPC vs AMQP

### Tabla Comparativa General

| Característica | HTTP/HTTPS | gRPC | AMQP |
|----------------|------------|------|------|
| **Tipo** | Request-Response | Request-Response + Streaming | Message Queue |
| **Protocolo Base** | TCP/IP | HTTP/2 | TCP/IP |
| **Formato de Datos** | Texto (JSON/XML) o Binario | Binario (Protocol Buffers) | Binario |
| **Patrón** | Síncrono | Síncrono + Asíncrono | Asíncrono |
| **Performance** | ⭐⭐⭐ Bueno | ⭐⭐⭐⭐⭐ Excelente | ⭐⭐⭐⭐ Muy Bueno |
| **Latencia** | 10-50ms | 1-10ms | 5-20ms (+ queue) |
| **Overhead** | Alto (headers HTTP) | Bajo (binario compacto) | Medio (metadata) |
| **Streaming** | ❌ HTTP/1.1, ✅ HTTP/2 | ✅✅✅ Nativo | ❌ No aplica |
| **Bidireccional** | ❌ HTTP/1.1, ⚠️ HTTP/2 | ✅ Full-duplex | ✅ Publish-Subscribe |
| **Persistencia** | ❌ Stateless | ❌ Stateless | ✅ Mensajes persistidos |
| **Retry Automático** | ❌ Manual | ❌ Manual | ✅ Nativo |
| **Load Balancing** | ✅ Fácil | ⚠️ Complejo | ✅ Nativo (workers) |
| **Firewall Friendly** | ✅✅✅ Siempre pasa | ⚠️ Bloqueado frecuentemente | ⚠️ Puertos específicos |
| **Debugging** | ✅✅✅ Muy fácil | ⭐⭐ Difícil | ⭐⭐⭐ Medio |
| **Tooling** | ✅✅✅ Excelente | ⭐⭐⭐ Limitado | ⭐⭐⭐ Bueno |
| **Curva Aprendizaje** | ✅ Fácil | ⭐⭐ Media-Difícil | ⭐⭐⭐ Media |
| **Browser Support** | ✅ Nativo | ❌ gRPC-Web required | ❌ No soportado |
| **Uso Principal** | APIs públicas, Web | Microservicios internos | Event-driven, Jobs |

---

### 📊 HTTP/HTTPS

#### Descripción
Protocolo estándar de la web. Request-Response sobre TCP/IP. Ampliamente soportado y maduro.

#### Características Técnicas
```
- Protocolo: HTTP/1.1 o HTTP/2
- Puerto: 80 (HTTP), 443 (HTTPS)
- Formato: Texto plano (JSON, XML, HTML) o binario
- Métodos: GET, POST, PUT, DELETE, PATCH, OPTIONS, HEAD
- Headers: Metadata extensa (Content-Type, Authorization, etc.)
- Stateless: Cada request es independiente
```

#### ✅ Ventajas
- **Universal**: Soportado por todos los lenguajes y plataformas
- **Debugging simple**: Herramientas como curl, Postman, browser DevTools
- **Cacheable**: HTTP caching nativo (ETag, Cache-Control)
- **Firewall friendly**: Siempre permitido (puerto 80/443)
- **HTTP/2 features**: Multiplexing, server push, header compression
- **WebSockets**: Comunicación bidireccional sobre HTTP
- **Documentación**: Swagger/OpenAPI para REST APIs

#### ❌ Desventajas
- **Overhead**: Headers HTTP verbosos (especialmente HTTP/1.1)
- **No streaming nativo**: HTTP/1.1 requiere workarounds
- **Performance**: Más lento que protocolos binarios
- **No type-safe**: Requiere validación manual o generadores
- **Serialización**: JSON es más lento que Protocol Buffers

#### 🎯 Casos de Uso Ideales
- APIs públicas (REST, GraphQL)
- Aplicaciones web
- Mobile apps
- Servicios que necesitan máxima compatibilidad
- Integraciones con terceros
- APIs documentadas para desarrolladores externos

#### 📝 Ejemplo .NET 10

```csharp
// Cliente HTTP
var httpClient = new HttpClient();
httpClient.BaseAddress = new Uri("https://api.example.com");

var response = await httpClient.GetAsync("/users/123");
var user = await response.Content.ReadFromJsonAsync<User>();

// Servidor HTTP (REST API)
app.MapGet("/users/{id}", async (string id, IUserRepository repo) =>
{
    var user = await repo.GetByIdAsync(id);
    return user is not null ? Results.Ok(user) : Results.NotFound();
});
```

---

### ⚡ gRPC (Google Remote Procedure Call)

#### Descripción
Framework RPC moderno de Google. Usa HTTP/2 y Protocol Buffers. Diseñado para comunicación eficiente entre microservicios.

#### Características Técnicas
```
- Protocolo: HTTP/2
- Puerto: Configurable (típicamente 5001, 5002)
- Formato: Protocol Buffers (binario)
- Streaming: Unary, Server, Client, Bidirectional
- Code generation: Compilador protoc genera código
- Fuertemente tipado: Schemas en .proto files
- Multiplexing: Múltiples requests en una conexión
```

#### ✅ Ventajas
- **Performance excepcional**: 5-10x más rápido que JSON/HTTP
- **Payload pequeño**: Protocol Buffers muy compacto
- **Streaming nativo**: 4 tipos de streaming soportados
- **Type-safe**: Schemas .proto definen contratos
- **Code generation**: Cliente y servidor auto-generados
- **HTTP/2**: Multiplexing, flow control, header compression
- **Interoperabilidad**: Multi-lenguaje (C#, Go, Java, Python, etc.)
- **Deadline/Timeout**: Manejo de timeouts integrado

#### ❌ Desventajas
- **No browser support**: Requiere gRPC-Web (proxy Envoy)
- **Debugging difícil**: No hay herramientas HTTP estándar
- **Firewall issues**: Bloqueado por algunos firewalls corporativos
- **Curva aprendizaje**: Protocol Buffers, .proto files
- **Load balancing complejo**: Requiere L7 load balancer o service mesh
- **No human-readable**: Binario, no se puede inspeccionar fácilmente
- **Versioning**: Cambios en .proto requieren cuidado

#### 🎯 Casos de Uso Ideales
- Comunicación entre microservicios (backend-to-backend)
- Alta carga y baja latencia requerida
- Streaming de datos en tiempo real
- IoT devices con ancho de banda limitado
- Sistemas distribuidos donde performance es crítica
- Arquitecturas de microservicios en Kubernetes

#### 📝 Ejemplo .NET 10

```csharp
// Archivo .proto
syntax = "proto3";

service UserService {
  rpc GetUser (GetUserRequest) returns (UserResponse);
  rpc StreamUsers (StreamUsersRequest) returns (stream UserResponse);
}

message GetUserRequest {
  string id = 1;
}

message UserResponse {
  string id = 1;
  string name = 2;
  string email = 3;
}

// Cliente gRPC (.NET 10)
var channel = GrpcChannel.ForAddress("https://localhost:5001");
var client = new UserService.UserServiceClient(channel);

var response = await client.GetUserAsync(new GetUserRequest { Id = "123" });
Console.WriteLine($"User: {response.Name}");

// Servidor gRPC (.NET 10)
public class UserServiceImpl : UserService.UserServiceBase
{
    private readonly IUserRepository _repository;
    
    public override async Task<UserResponse> GetUser(
        GetUserRequest request,
        ServerCallContext context)
    {
        var user = await _repository.GetByIdAsync(request.Id);
        
        return new UserResponse
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email
        };
    }
    
    // Server streaming
    public override async Task StreamUsers(
        StreamUsersRequest request,
        IServerStreamWriter<UserResponse> responseStream,
        ServerCallContext context)
    {
        var users = await _repository.GetAllAsync();
        
        foreach (var user in users)
        {
            await responseStream.WriteAsync(new UserResponse
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email
            });
        }
    }
}

// Configuración servidor
builder.Services.AddGrpc();
app.MapGrpcService<UserServiceImpl>();
```

---

### 📬 AMQP (Advanced Message Queuing Protocol)

#### Descripción
Protocolo de mensajería asíncrona. Diseñado para comunicación confiable mediante colas de mensajes. Implementado por RabbitMQ, Azure Service Bus, etc.

#### Características Técnicas
```
- Protocolo: TCP/IP (puerto 5672)
- Formato: Binario con metadata
- Patrones: Point-to-Point, Publish-Subscribe, Request-Reply
- Persistencia: Mensajes guardados en disco
- Acknowledgments: Confirmación manual/automática
- Dead Letter Queues: Manejo de errores
- TTL: Time-to-live para mensajes
- Priority Queues: Priorización de mensajes
```

#### ✅ Ventajas
- **Desacoplamiento total**: Productor y consumidor independientes
- **Asíncrono nativo**: Fire-and-forget, no espera respuesta
- **Persistencia**: Mensajes no se pierden si servicio está down
- **Retry automático**: Reintento nativo en caso de fallo
- **Load leveling**: Procesa carga a ritmo controlado
- **Escalabilidad**: Workers compiten por mensajes (competing consumers)
- **Dead Letter Queues**: Manejo robusto de errores
- **Garantías de entrega**: At-least-once, at-most-once, exactly-once
- **Priority**: Mensajes con prioridad
- **Delay/Schedule**: Envío programado de mensajes

#### ❌ Desventajas
- **Eventual consistency**: No inmediatez, latencia inherente
- **Complejidad infraestructura**: Requiere message broker (RabbitMQ, etc.)
- **Debugging difícil**: Flujos asíncronos complejos de trazar
- **No request-response**: Requiere patterns adicionales (RPC over AMQP)
- **Overhead**: Mensaje + metadata + queue storage
- **Costo**: Infraestructura adicional (broker, almacenamiento)
- **Monitoreo complejo**: Requiere herramientas especializadas

#### 🎯 Casos de Uso Ideales
- Procesamiento asíncrono (emails, notificaciones, reportes)
- Event-driven architecture
- Workflows de múltiples pasos
- Sistemas con alta carga (throttling/rate limiting)
- Integraciones con sistemas externos lentos
- Background jobs (procesamiento de imágenes, videos)
- Operaciones que no requieren respuesta inmediata
- Garantías de entrega críticas

#### 📝 Ejemplo .NET 10

```csharp
// MassTransit con RabbitMQ (.NET 10)

// Mensaje
public record OrderCreatedEvent
{
    public string OrderId { get; init; }
    public string UserId { get; init; }
    public decimal Total { get; init; }
}

// Publisher
public class OrderService
{
    private readonly IPublishEndpoint _publishEndpoint;
    
    public async Task CreateOrderAsync(CreateOrderCommand command)
    {
        // 1. Crear orden en DB
        var order = await _repository.CreateAsync(command);
        
        // 2. Publicar evento asíncrono
        await _publishEndpoint.Publish(new OrderCreatedEvent
        {
            OrderId = order.Id,
            UserId = command.UserId,
            Total = order.Total
        });
        
        // 3. Retornar inmediatamente (no espera procesamiento)
        return order;
    }
}

// Consumer (Payment Service)
public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
{
    private readonly IPaymentService _paymentService;
    
    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        var orderEvent = context.Message;
        
        try
        {
            // Procesar pago (puede tardar segundos)
            await _paymentService.ProcessPaymentAsync(
                orderEvent.OrderId,
                orderEvent.Total);
            
            // Publicar evento de pago procesado
            await context.Publish(new PaymentProcessedEvent
            {
                OrderId = orderEvent.OrderId
            });
        }
        catch (Exception ex)
        {
            // Mensaje va a DLQ automáticamente después de reintentos
            throw;
        }
    }
}

// Configuración MassTransit
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<OrderCreatedConsumer>();
    
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq://localhost", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });
        
        cfg.ReceiveEndpoint("orders-queue", e =>
        {
            e.ConfigureConsumer<OrderCreatedConsumer>(context);
            
            // Configuración de reintentos
            e.UseMessageRetry(r => r.Incremental(3, 
                TimeSpan.FromSeconds(1), 
                TimeSpan.FromSeconds(2)));
        });
    });
});
```

---

### 📊 Comparativa de Performance

| Métrica | HTTP/JSON | gRPC/Protobuf | AMQP |
|---------|-----------|---------------|------|
| **Latencia** | 10-50ms | 1-10ms | 5-20ms + queue |
| **Throughput** | 1,000-5,000 req/s | 10,000-50,000 req/s | 5,000-20,000 msg/s |
| **Payload Size** | 100% (baseline) | 20-30% del JSON | 40-60% del JSON |
| **CPU Usage** | Alto (JSON parsing) | Bajo (binario) | Medio |
| **Memory** | Alto (deserialización) | Bajo | Medio (queue storage) |
| **Network** | Alto (texto + headers) | Bajo (binario compacto) | Medio |

---

### 🎯 Matriz de Decisión

| Escenario | Recomendación | Razón |
|-----------|---------------|-------|
| **API pública para web/mobile** | HTTP/REST o GraphQL | Compatibilidad universal, documentación |
| **Backend microservicios** | gRPC | Performance, type-safe, streaming |
| **Procesamiento asíncrono** | AMQP | Desacoplamiento, retry, persistencia |
| **Real-time updates** | HTTP/WebSocket o gRPC streaming | Bidireccional, baja latencia |
| **Alta carga** | gRPC + AMQP | Performance + load leveling |
| **Event-driven** | AMQP | Pub-sub nativo, desacoplamiento |
| **Legacy integrations** | HTTP/REST | Compatibilidad, simplicidad |
| **IoT devices** | gRPC o AMQP | Payload pequeño, confiabilidad |

---

### 🔄 Combinaciones Comunes

#### 1. HTTP + gRPC
```
Frontend (Web/Mobile) → HTTP/REST → BFF
                                     ↓
                                   gRPC
                                     ↓
                            Microservicios
```
✅ Frontend simple, backend eficiente

#### 2. HTTP + AMQP
```
Frontend → HTTP/REST → API
                       ↓
                    AMQP Queue
                       ↓
                  Background Workers
```
✅ Respuestas rápidas, procesamiento asíncrono

#### 3. gRPC + AMQP
```
BFF → gRPC → Microservicio A (sync)
     ↓
   AMQP Queue
     ↓
Microservicio B (async)
```
✅ Performance + operaciones largas

#### 4. HTTP + gRPC + AMQP (Arquitectura híbrida)
```
Frontend → HTTP → BFF
                   ├─→ gRPC (queries rápidas)
                   │   ↓
                   │   Microservicios
                   │
                   └─→ AMQP (operaciones largas)
                       ↓
                       Workers
```
✅ Máxima flexibilidad

---

## 🔗 Comparativa de Paradigmas de APIs

---

## 2. REST vs GraphQL vs gRPC vs WebSockets

### Tabla Comparativa General

| Característica | REST | GraphQL | gRPC | WebSockets |
|----------------|------|---------|------|------------|
| **Año Creación** | 2000 | 2015 (Facebook) | 2016 (Google) | 2011 |
| **Protocolo** | HTTP/HTTPS | HTTP/HTTPS | HTTP/2 | TCP/WebSocket |
| **Formato** | JSON/XML | JSON | Protocol Buffers | Cualquiera |
| **Endpoints** | Múltiples | Único (/graphql) | Por servicio | Único |
| **Type System** | ❌ No | ✅ Schema | ✅ .proto | ❌ No |
| **Versioning** | URL o headers | ✅ Schema evolution | ✅ Backward compatible | Manual |
| **Caching** | ✅ HTTP nativo | ⚠️ Complejo | ❌ No | ❌ No |
| **Over-fetching** | ❌ Problema común | ✅ Eliminado | ✅ Eliminado | ✅ Control total |
| **Under-fetching** | ❌ N+1 queries | ✅ Resuelto | ✅ Resuelto | ✅ Push server |
| **Real-time** | ❌ Polling | ✅ Subscriptions | ✅ Streaming | ✅✅✅ Nativo |
| **Batching** | ❌ Manual | ✅ DataLoader | ✅ Nativo | ✅ Manual |
| **Performance** | ⭐⭐⭐ | ⭐⭐⭐⭐ | ⭐⭐⭐⭐⭐ | ⭐⭐⭐⭐ |
| **Learning Curve** | ✅ Fácil | ⭐⭐ Media | ⭐⭐⭐ Difícil | ⭐⭐ Media |
| **Tooling** | ✅✅✅ Excelente | ✅✅ Muy bueno | ⭐⭐⭐ Limitado | ⭐⭐⭐ Medio |
| **Browser Support** | ✅ Nativo | ✅ Nativo | ❌ gRPC-Web | ✅ Nativo |
| **Uso Principal** | APIs públicas | SPAs, Mobile apps | Microservicios | Chat, Gaming |

---

### 🔵 REST (Representational State Transfer)

#### Descripción
Estilo arquitectónico estándar para APIs sobre HTTP. Recursos identificados por URLs, operaciones con verbos HTTP.

#### Principios REST
```
1. Client-Server: Separación de responsabilidades
2. Stateless: Cada request tiene toda la info necesaria
3. Cacheable: Respuestas deben indicar si son cacheables
4. Uniform Interface: URLs, HTTP verbs, status codes
5. Layered System: Cliente no sabe si habla con servidor final
6. Code on Demand (opcional): Servidor puede enviar código ejecutable
```

#### ✅ Ventajas
- **Simplicidad**: Fácil de entender y usar
- **Estándar HTTP**: Métodos, status codes, headers bien conocidos
- **Caching**: HTTP caching nativo (Varnish, CDN, browser)
- **Stateless**: Escalabilidad horizontal simple
- **Tooling maduro**: Swagger/OpenAPI, Postman, Insomnia
- **Wide adoption**: Cualquier lenguaje lo soporta
- **Documentación**: OpenAPI Specification (Swagger)

#### ❌ Desventajas
- **Over-fetching**: GET /users devuelve todos los campos
- **Under-fetching**: Necesitas múltiples requests (N+1)
- **Versionado complejo**: /v1/users vs /v2/users
- **No type-safe**: Requiere validación manual
- **Rigid structure**: URLs fijas, no flexibilidad

#### 📝 Ejemplo .NET 10

```csharp
// REST API (.NET 10)
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _repository;
    
    // GET /api/users
    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        var users = await _repository.GetAllAsync();
        return Ok(users);
    }
    
    // GET /api/users/{id}
    [HttpGet("{id}")]
    [ResponseCache(Duration = 300)] // HTTP caching
    public async Task<ActionResult<User>> GetUser(string id)
    {
        var user = await _repository.GetByIdAsync(id);
        return user is not null ? Ok(user) : NotFound();
    }
    
    // POST /api/users
    [HttpPost]
    public async Task<ActionResult<User>> CreateUser(CreateUserDto dto)
    {
        var user = await _repository.CreateAsync(dto);
        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
    }
    
    // PUT /api/users/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(string id, UpdateUserDto dto)
    {
        await _repository.UpdateAsync(id, dto);
        return NoContent();
    }
    
    // DELETE /api/users/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        await _repository.DeleteAsync(id);
        return NoContent();
    }
}

// Cliente REST (TypeScript/Angular)
@Injectable()
export class UsersService {
  constructor(private http: HttpClient) {}
  
  getUsers(): Observable<User[]> {
    return this.http.get<User[]>('/api/users');
  }
  
  getUser(id: string): Observable<User> {
    return this.http.get<User>(`/api/users/${id}`);
  }
  
  createUser(user: CreateUserDto): Observable<User> {
    return this.http.post<User>('/api/users', user);
  }
}
```

---

### 🟢 GraphQL

#### Descripción
Lenguaje de consulta y runtime para APIs. Cliente especifica exactamente qué datos necesita. Desarrollado por Facebook.

#### Conceptos Clave
```
- Schema: Definición de tipos y operaciones
- Query: Lectura de datos
- Mutation: Modificación de datos
- Subscription: Updates en tiempo real (WebSocket)
- Resolver: Función que obtiene datos para un campo
- DataLoader: Batching y caching de queries
```

#### ✅ Ventajas
- **Sin over-fetching**: Cliente pide exactamente lo que necesita
- **Sin under-fetching**: Una query obtiene datos relacionados
- **Type-safe**: Schema define contratos
- **Introspection**: Schema auto-documentado
- **Versionado**: Schema evolution sin breaking changes
- **Developer experience**: Autocompletado, validación IDE
- **GraphQL Playground**: Explorador interactivo
- **Real-time**: Subscriptions nativas

#### ❌ Desventajas
- **Complejidad**: Curva de aprendizaje mayor que REST
- **Caching complejo**: No se puede usar HTTP caching estándar
- **N+1 problema**: Si no implementas DataLoader
- **Rate limiting**: Difícil limitar por query complexity
- **File upload**: Requiere workarounds (multipart)
- **Performance**: Queries complejas pueden ser costosas
- **Overhead**: Parsing de queries en servidor

#### 📝 Ejemplo .NET 10

```csharp
// Schema GraphQL (HotChocolate 14 + .NET 10)

// Tipos
public class User
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    
    // Relación
    public async Task<List<Order>> GetOrders(
        [Service] IOrderRepository orderRepo)
    {
        return await orderRepo.GetByUserIdAsync(Id);
    }
}

public class Order
{
    public string Id { get; set; }
    public decimal Total { get; set; }
    public DateTime CreatedAt { get; set; }
}

// Query
[ExtendObjectType(typeof(Query))]
public class UserQueries
{
    // Query: user(id: "123")
    public async Task<User?> GetUser(
        string id,
        [Service] IUserRepository repo)
    {
        return await repo.GetByIdAsync(id);
    }
    
    // DataLoader para N+1 prevention
    [DataLoader]
    public async Task<IReadOnlyDictionary<string, User>> GetUsersBatch(
        IReadOnlyList<string> ids,
        [Service] IUserRepository repo)
    {
        var users = await repo.GetByIdsAsync(ids);
        return users.ToDictionary(u => u.Id);
    }
}

// Mutation
[ExtendObjectType(typeof(Mutation))]
public class UserMutations
{
    public async Task<User> CreateUser(
        CreateUserInput input,
        [Service] IUserRepository repo)
    {
        var user = new User
        {
            Name = input.Name,
            Email = input.Email
        };
        
        return await repo.CreateAsync(user);
    }
}

// Subscription
[ExtendObjectType(typeof(Subscription))]
public class UserSubscriptions
{
    [Subscribe]
    public User OnUserCreated(
        [EventMessage] User user) => user;
}

// Configuración
builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    .AddSubscriptionType<Subscription>()
    .AddType<UserQueries>()
    .AddType<UserMutations>()
    .AddType<UserSubscriptions>()
    .AddDataLoader<UserDataLoader>();

app.MapGraphQL();

// Cliente (TypeScript/Apollo Angular)
const GET_USER = gql`
  query GetUser($id: ID!) {
    user(id: $id) {
      id
      name
      email
      orders {
        id
        total
        createdAt
      }
    }
  }
`;

@Injectable()
export class UsersService {
  constructor(private apollo: Apollo) {}
  
  getUser(id: string) {
    return this.apollo.query({
      query: GET_USER,
      variables: { id }
    });
  }
}

// Subscription (WebSocket)
const USER_CREATED = gql`
  subscription OnUserCreated {
    onUserCreated {
      id
      name
      email
    }
  }
`;

this.apollo.subscribe({ query: USER_CREATED })
  .subscribe(result => {
    console.log('New user:', result.data.onUserCreated);
  });
```

---

### ⚡ gRPC (Ya cubierto arriba, resumen para comparación)

#### Características Clave
- Framework RPC de Google
- HTTP/2 + Protocol Buffers
- 4 tipos de streaming
- Type-safe con .proto
- Performance excepcional
- No browser support (requiere gRPC-Web)

---

### 🔴 WebSockets

#### Descripción
Protocolo de comunicación bidireccional full-duplex sobre una sola conexión TCP. Ideal para aplicaciones real-time.

#### Características Técnicas
```
- Protocolo: WebSocket (ws:// o wss://)
- Puerto: 80 (ws) o 443 (wss)
- Handshake: Upgrade HTTP → WebSocket
- Bidireccional: Cliente y servidor pueden enviar cuando quieran
- Persistente: Conexión abierta constantemente
- Low overhead: Sin headers HTTP en cada mensaje
```

#### ✅ Ventajas
- **Real-time**: Latencia mínima (ms)
- **Bidireccional**: Cliente y servidor envían/reciben
- **Eficiente**: Sin overhead de HTTP en cada mensaje
- **Persistente**: Conexión permanente abierta
- **Push nativo**: Servidor envía sin solicitud cliente
- **Browser support**: Nativo en todos los navegadores modernos

#### ❌ Desventajas
- **Stateful**: Conexión persistente (no escala fácilmente)
- **Load balancing**: Requiere sticky sessions
- **Firewall**: Algunos corporativos bloquean WebSocket
- **Complejidad**: Manejo de reconexiones, heartbeats
- **No caching**: No se puede cachear
- **Debugging**: Más difícil que HTTP

#### 🎯 Casos de Uso Ideales
- Chat applications
- Live notifications
- Real-time dashboards
- Collaborative editing (Google Docs)
- Gaming multiplayer
- Financial trading platforms
- IoT device monitoring

#### 📝 Ejemplo .NET 10

```csharp
// Servidor WebSocket (.NET 10)
app.UseWebSockets();

app.Map("/ws", async context =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
        await HandleWebSocketAsync(webSocket);
    }
    else
    {
        context.Response.StatusCode = 400;
    }
});

async Task HandleWebSocketAsync(WebSocket webSocket)
{
    var buffer = new byte[1024 * 4];
    
    while (webSocket.State == WebSocketState.Open)
    {
        var result = await webSocket.ReceiveAsync(
            new ArraySegment<byte>(buffer), 
            CancellationToken.None);
        
        if (result.MessageType == WebSocketMessageType.Text)
        {
            var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
            Console.WriteLine($"Received: {message}");
            
            // Echo back
            var responseMessage = Encoding.UTF8.GetBytes($"Echo: {message}");
            await webSocket.SendAsync(
                new ArraySegment<byte>(responseMessage),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);
        }
        else if (result.MessageType == WebSocketMessageType.Close)
        {
            await webSocket.CloseAsync(
                WebSocketCloseStatus.NormalClosure,
                "Closing",
                CancellationToken.None);
        }
    }
}

// Cliente JavaScript
const ws = new WebSocket('wss://example.com/ws');

ws.onopen = () => {
  console.log('Connected');
  ws.send('Hello Server!');
};

ws.onmessage = (event) => {
  console.log('Message:', event.data);
};

ws.onerror = (error) => {
  console.error('Error:', error);
};

ws.onclose = () => {
  console.log('Disconnected');
};

// SignalR (abstracción sobre WebSockets en .NET)
// Servidor
public class ChatHub : Hub
{
    public async Task SendMessage(string user, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }
}

builder.Services.AddSignalR();
app.MapHub<ChatHub>("/chatHub");

// Cliente TypeScript/Angular
import { HubConnectionBuilder } from '@microsoft/signalr';

const connection = new HubConnectionBuilder()
  .withUrl('/chatHub')
  .build();

connection.on('ReceiveMessage', (user, message) => {
  console.log(`${user}: ${message}`);
});

connection.start();
connection.invoke('SendMessage', 'John', 'Hello!');
```

---

### 🆚 Comparativa Detallada

#### Over-fetching / Under-fetching

```typescript
// ❌ REST - Over-fetching
GET /api/users/123
// Devuelve TODO el user (50 campos)
// Solo necesitabas name y email

// ❌ REST - Under-fetching
GET /api/users/123       // User data
GET /api/users/123/orders // Orders (N+1)
GET /api/orders/456      // Order details
GET /api/orders/789      // Order details

// ✅ GraphQL - Perfecto
query {
  user(id: "123") {
    name        # Solo lo que necesitas
    email
    orders {    # Todo en una query
      id
      total
    }
  }
}

// ✅ gRPC - Controlado en .proto
message UserRequest {
  string id = 1;
  repeated string fields = 2; // ["name", "email"]
}
```

#### Real-time Updates

```typescript
// ❌ REST - Polling (ineficiente)
setInterval(() => {
  fetch('/api/orders/status')
    .then(res => res.json())
    .then(data => updateUI(data));
}, 5000); // Cada 5 segundos

// ⚠️ REST - Long polling
async function longPoll() {
  const res = await fetch('/api/orders/status?timeout=30');
  updateUI(await res.json());
  longPoll(); // Recurse
}

// ✅ GraphQL - Subscriptions
subscription {
  orderStatusChanged(orderId: "123") {
    id
    status
  }
}

// ✅ gRPC - Server streaming
service OrderService {
  rpc WatchOrder(OrderRequest) returns (stream OrderStatus);
}

// ✅ WebSocket - Nativo
ws.onmessage = (event) => {
  const order = JSON.parse(event.data);
  updateUI(order);
};
```

---

### 📊 Matriz de Decisión por Caso de Uso

| Caso de Uso | REST | GraphQL | gRPC | WebSocket |
|-------------|------|---------|------|-----------|
| **API pública** | ✅✅✅ | ✅✅ | ❌ | ❌ |
| **SPA/Mobile app** | ✅✅ | ✅✅✅ | ❌ | ⚠️ |
| **Microservicios** | ⚠️ | ⚠️ | ✅✅✅ | ❌ |
| **Real-time app** | ❌ | ✅✅ | ✅✅ | ✅✅✅ |
| **High performance** | ⭐⭐ | ⭐⭐⭐ | ⭐⭐⭐⭐⭐ | ⭐⭐⭐⭐ |
| **CRUD simple** | ✅✅✅ | ⚠️ | ❌ | ❌ |
| **Flexible queries** | ❌ | ✅✅✅ | ⚠️ | ⚠️ |
| **Low bandwidth** | ⭐⭐ | ⭐⭐⭐ | ⭐⭐⭐⭐⭐ | ⭐⭐⭐ |
| **Legacy systems** | ✅✅✅ | ⚠️ | ❌ | ❌ |
| **Gaming** | ❌ | ❌ | ⚠️ | ✅✅✅ |
| **Chat** | ❌ | ⚠️ | ⚠️ | ✅✅✅ |
| **IoT** | ⭐⭐ | ❌ | ✅✅✅ | ✅✅ |

---

### 🏗️ Arquitecturas Híbridas Recomendadas

#### 1. **REST + WebSocket** (Simple real-time)
```
Frontend
├── REST API → CRUD operations
└── WebSocket → Real-time notifications
```
✅ Fácil de implementar, buena para dashboards

#### 2. **GraphQL + Subscriptions** (Modern SPA)
```
Frontend
├── GraphQL Queries/Mutations → Data operations
└── GraphQL Subscriptions (WebSocket) → Real-time
```
✅ Mejor developer experience, type-safe

#### 3. **REST (Frontend) + gRPC (Backend)** (Performance)
```
Frontend → REST → BFF
                   ↓ gRPC
              Microservicios
```
✅ Frontend simple, backend eficiente

#### 4. **GraphQL + gRPC + WebSocket** (Enterprise)
```
Frontend
├── GraphQL → BFF
│             ├─→ gRPC (microservicios)
│             └─→ WebSocket (real-time)
```
✅ Máxima flexibilidad y performance

---

### 📈 Evolución y Tendencias

```
2000-2010: REST dominante
    ↓
2015-2020: GraphQL gana tracción (Facebook, GitHub, Shopify)
    ↓
2016-presente: gRPC para microservicios (Google, Netflix, Uber)
    ↓
2020-presente: Híbridos (REST + GraphQL + gRPC + WebSocket)
    ↓
Futuro: HTTP/3 + QUIC, WebTransport, GraphQL Federation
```

---

## 🎓 Conclusiones

### Para APIs Públicas:
- **Primera opción:** REST (simplicidad, compatibilidad)
- **Moderna alternativa:** GraphQL (flexibilidad, developer experience)
- **Real-time:** GraphQL Subscriptions o WebSocket

### Para Microservicios Internos:
- **Primera opción:** gRPC (performance, type-safe, streaming)
- **Alternativa:** HTTP/GraphQL si gRPC no es viable
- **Event-driven:** AMQP (RabbitMQ, Azure Service Bus)

### Para Aplicaciones Real-time:
- **Gaming/Chat:** WebSocket con SignalR
- **Dashboards:** GraphQL Subscriptions
- **Monitoring:** gRPC Server Streaming

### Arquitectura Recomendada (Enterprise):
```
Frontend (Web/Mobile)
    ↓ GraphQL/HTTP
BFF (.NET 10)
    ├─→ gRPC (queries/mutations síncronas)
    ├─→ AMQP (operaciones asíncronas)
    └─→ WebSocket (real-time updates)
        ↓
Microservicios (.NET 10)
```

---

**Última actualización**: Enero 2026
