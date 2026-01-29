# 🎯 Plan de Implementación: BFF .NET 10 con Clean Architecture

## Arquitectura 3: REST Frontend + GraphQL/HTTP Backend

```
┌─────────────────────────────────────────────────────────────┐
│                    [REST] FRONTEND LAYER                    │
│  ┌──────────────────┐         ┌───────────────────────┐     │
│  │   Angular SPA    │         │  Mobile (Kotlin/Swift)│     │
│  │   (HttpClient)   │         │ (Retrofit/Alamofire)  │     │
│  └────────┬─────────┘         └──────────┬────────────┘     │
└───────────┼──────────────────────────────┼──────────────────┘
            │                              │
            └────────────┬─────────────────┘
                         │ REST/HTTP
                         │ (JSON)
                         ▼
┌─────────────────────────────────────────────────────────────┐
│         [HYBRID] BFF LAYER (.NET 10 - REST + GraphQL)       │
│  ┌────────────────────────────────────────────────────┐     │
│  │         REST Controllers (Web API)                 │     │
│  │  • Expone endpoints REST al frontend               │     │
│  │  • Consume GraphQL de microservicios               │     │
│  │  • Traduce REST → GraphQL                          │     │
│  └────────────────────┬───────────────────────────────┘     │
└───────────────────────┼─────────────────────────────────────┘
                        │ GraphQL/HTTP
                        │ POST /graphql
                        ▼
┌─────────────────────────────────────────────────────────────┐
│          [GQL] MICROSERVICES (.NET 10 - GraphQL/HTTP)       │
│  ┌──────────────┐  ┌──────────────┐  ┌─────────────────┐    │
│  │    Users     │  │    Orders    │  │    Payments     │    │
│  │   Service    │  │   Service    │  │    Service      │    │
│  │ (GraphQL API)│  │ (GraphQL API)│  │  (GraphQL API)  │    │
│  │  HTTP Only   │  │  HTTP Only   │  │   HTTP Only     │    │
│  └──────────────┘  └──────────────┘  └─────────────────┘    │
└─────────────────────────────────────────────────────────────┘
```

---

## 📁 Estructura del Proyecto (Clean Architecture)

```
BFF.Solution/
│
├── src/
│   ├── BFF.API/                          # 🌐 Capa de Presentación
│   │   ├── Controllers/                  # REST Controllers
│   │   │   ├── UsersController.cs
│   │   │   ├── OrdersController.cs
│   │   │   └── PaymentsController.cs
│   │   ├── Middleware/
│   │   │   ├── ExceptionHandlingMiddleware.cs
│   │   │   ├── RequestLoggingMiddleware.cs
│   │   │   └── DynatraceMiddleware.cs
│   │   ├── Filters/
│   │   │   ├── ValidationFilter.cs
│   │   │   └── DynatraceActionFilter.cs
│   │   ├── Extensions/
│   │   │   ├── ServiceCollectionExtensions.cs
│   │   │   └── ApplicationBuilderExtensions.cs
│   │   ├── Program.cs
│   │   ├── appsettings.json
│   │   ├── appsettings.Development.json
│   │   └── BFF.API.csproj
│   │
│   ├── BFF.Application/                  # 📋 Capa de Aplicación
│   │   ├── Services/                     # Application Services
│   │   │   ├── Interfaces/
│   │   │   │   ├── IUserService.cs
│   │   │   │   ├── IOrderService.cs
│   │   │   │   └── IPaymentService.cs
│   │   │   ├── UserService.cs
│   │   │   ├── OrderService.cs
│   │   │   └── PaymentService.cs
│   │   ├── DTOs/                         # Data Transfer Objects
│   │   │   ├── Requests/
│   │   │   │   ├── CreateUserRequest.cs
│   │   │   │   ├── CreateOrderRequest.cs
│   │   │   │   └── ProcessPaymentRequest.cs
│   │   │   ├── Responses/
│   │   │   │   ├── UserResponse.cs
│   │   │   │   ├── OrderResponse.cs
│   │   │   │   └── PaymentResponse.cs
│   │   │   └── GraphQL/
│   │   │       ├── UserGraphQLResponse.cs
│   │   │       ├── OrderGraphQLResponse.cs
│   │   │       └── PaymentGraphQLResponse.cs
│   │   ├── Mappings/                     # AutoMapper Profiles
│   │   │   ├── UserMappingProfile.cs
│   │   │   ├── OrderMappingProfile.cs
│   │   │   └── PaymentMappingProfile.cs
│   │   ├── Validators/                   # FluentValidation
│   │   │   ├── CreateUserRequestValidator.cs
│   │   │   ├── CreateOrderRequestValidator.cs
│   │   │   └── ProcessPaymentRequestValidator.cs
│   │   ├── Exceptions/
│   │   │   ├── GraphQLClientException.cs
│   │   │   ├── ValidationException.cs
│   │   │   └── ServiceUnavailableException.cs
│   │   └── BFF.Application.csproj
│   │
│   ├── BFF.Infrastructure/               # 🔧 Capa de Infraestructura
│   │   ├── GraphQL/                      # GraphQL Clients
│   │   │   ├── Clients/
│   │   │   │   ├── IUsersGraphQLClient.cs
│   │   │   │   ├── UsersGraphQLClient.cs
│   │   │   │   ├── IOrdersGraphQLClient.cs
│   │   │   │   ├── OrdersGraphQLClient.cs
│   │   │   │   ├── IPaymentsGraphQLClient.cs
│   │   │   │   └── PaymentsGraphQLClient.cs
│   │   │   ├── Queries/
│   │   │   │   ├── UserQueries.cs
│   │   │   │   ├── OrderQueries.cs
│   │   │   │   └── PaymentQueries.cs
│   │   │   ├── Mutations/
│   │   │   │   ├── UserMutations.cs
│   │   │   │   ├── OrderMutations.cs
│   │   │   │   └── PaymentMutations.cs
│   │   │   └── Models/
│   │   │       └── GraphQLRequest.cs
│   │   ├── HttpClients/                  # HTTP Client Configuration
│   │   │   ├── GraphQLHttpClientFactory.cs
│   │   │   └── GraphQLHttpMessageHandler.cs
│   │   ├── Telemetry/                    # Dynatrace Telemetry
│   │   │   ├── DynatraceService.cs
│   │   │   ├── IDynatraceService.cs
│   │   │   ├── MetricsCollector.cs
│   │   │   └── TraceContextPropagator.cs
│   │   ├── Resilience/                   # Polly Policies
│   │   │   ├── RetryPolicies.cs
│   │   │   ├── CircuitBreakerPolicies.cs
│   │   │   └── TimeoutPolicies.cs
│   │   ├── Caching/
│   │   │   ├── ICacheService.cs
│   │   │   └── MemoryCacheService.cs
│   │   ├── Configuration/
│   │   │   ├── GraphQLServiceSettings.cs
│   │   │   └── DynatraceSettings.cs
│   │   └── BFF.Infrastructure.csproj
│   │
│   └── BFF.Domain/                       # 📦 Capa de Dominio
│       ├── Models/                       # Modelos de Dominio (sin lógica)
│       │   ├── User.cs
│       │   ├── Order.cs
│       │   ├── Payment.cs
│       │   └── Common/
│       │       ├── BaseEntity.cs
│       │       └── ErrorResult.cs
│       ├── Enums/
│       │   ├── OrderStatus.cs
│       │   ├── PaymentStatus.cs
│       │   └── UserRole.cs
│       ├── Constants/
│       │   ├── GraphQLConstants.cs
│       │   └── ApiRoutes.cs
│       └── BFF.Domain.csproj
│
├── tests/
│   ├── BFF.API.Tests/
│   │   ├── Controllers/
│   │   │   ├── UsersControllerTests.cs
│   │   │   ├── OrdersControllerTests.cs
│   │   │   └── PaymentsControllerTests.cs
│   │   └── BFF.API.Tests.csproj
│   │
│   ├── BFF.Application.Tests/
│   │   ├── Services/
│   │   │   ├── UserServiceTests.cs
│   │   │   ├── OrderServiceTests.cs
│   │   │   └── PaymentServiceTests.cs
│   │   └── BFF.Application.Tests.csproj
│   │
│   └── BFF.Infrastructure.Tests/
│       ├── GraphQL/
│       │   ├── UsersGraphQLClientTests.cs
│       │   └── OrdersGraphQLClientTests.cs
│       └── BFF.Infrastructure.Tests.csproj
│
├── docker/
│   ├── Dockerfile
│   ├── docker-compose.yml
│   └── .dockerignore
│
├── k8s/
│   ├── deployment.yaml
│   ├── service.yaml
│   ├── configmap.yaml
│   └── ingress.yaml
│
├── .github/
│   └── workflows/
│       └── ci-cd.yml
│
├── BFF.Solution.sln
└── README.md
```

---

## 🎯 Fase 1: Configuración del Proyecto

### 1.1 Crear Solución y Proyectos

```bash
# Crear solución
dotnet new sln -n BFF.Solution

# Crear estructura de carpetas
mkdir -p src tests

# Crear proyectos con .NET 10 en carpeta src/
dotnet new webapi -n BFF.API -o src/BFF.API -f net10.0
dotnet new classlib -n BFF.Application -o src/BFF.Application -f net10.0
dotnet new classlib -n BFF.Infrastructure -o src/BFF.Infrastructure -f net10.0
dotnet new classlib -n BFF.Domain -o src/BFF.Domain -f net10.0

# Crear proyectos de pruebas en carpeta tests/
dotnet new xunit -n BFF.API.Tests -o tests/BFF.API.Tests -f net10.0
dotnet new xunit -n BFF.Application.Tests -o tests/BFF.Application.Tests -f net10.0
dotnet new xunit -n BFF.Infrastructure.Tests -o tests/BFF.Infrastructure.Tests -f net10.0

# Agregar proyectos a la solución
dotnet sln add src/BFF.API/BFF.API.csproj
dotnet sln add src/BFF.Application/BFF.Application.csproj
dotnet sln add src/BFF.Infrastructure/BFF.Infrastructure.csproj
dotnet sln add src/BFF.Domain/BFF.Domain.csproj
dotnet sln add tests/BFF.API.Tests/BFF.API.Tests.csproj
dotnet sln add tests/BFF.Application.Tests/BFF.Application.Tests.csproj
dotnet sln add tests/BFF.Infrastructure.Tests/BFF.Infrastructure.Tests.csproj

# Referencias entre proyectos principales
dotnet add src/BFF.API/BFF.API.csproj reference src/BFF.Application/BFF.Application.csproj
dotnet add src/BFF.API/BFF.API.csproj reference src/BFF.Infrastructure/BFF.Infrastructure.csproj

dotnet add src/BFF.Application/BFF.Application.csproj reference src/BFF.Domain/BFF.Domain.csproj

dotnet add src/BFF.Infrastructure/BFF.Infrastructure.csproj reference src/BFF.Application/BFF.Application.csproj
dotnet add src/BFF.Infrastructure/BFF.Infrastructure.csproj reference src/BFF.Domain/BFF.Domain.csproj

# Referencias de tests a proyectos principales
dotnet add tests/BFF.API.Tests/BFF.API.Tests.csproj reference src/BFF.API/BFF.API.csproj
dotnet add tests/BFF.Application.Tests/BFF.Application.Tests.csproj reference src/BFF.Application/BFF.Application.csproj
dotnet add tests/BFF.Infrastructure.Tests/BFF.Infrastructure.Tests.csproj reference src/BFF.Infrastructure/BFF.Infrastructure.csproj
```

### 1.2 Instalar NuGet Packages

```xml
<!-- BFF.API.csproj -->
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <!-- ASP.NET Core -->
    <PackageReference Include="Microsoft.AspNetCore.OpenApi" Version="10.0.0" />
    <PackageReference Include="Swashbuckle.AspNetCore" Version="7.0.0" />
    
    <!-- Validation -->
    <PackageReference Include="FluentValidation.AspNetCore" Version="11.3.0" />
    
    <!-- Dynatrace -->
    <PackageReference Include="Dynatrace.OneAgent.Sdk" Version="1.8.0" />
    
    <!-- OpenTelemetry -->
    <PackageReference Include="OpenTelemetry.Exporter.OpenTelemetryProtocol" Version="1.9.0" />
    <PackageReference Include="OpenTelemetry.Extensions.Hosting" Version="1.9.0" />
    <PackageReference Include="OpenTelemetry.Instrumentation.AspNetCore" Version="1.9.0" />
    <PackageReference Include="OpenTelemetry.Instrumentation.Http" Version="1.9.0" />
    
    <!-- Logging -->
    <PackageReference Include="Serilog.AspNetCore" Version="8.0.0" />
    <PackageReference Include="Serilog.Sinks.Console" Version="6.0.0" />
    <PackageReference Include="Serilog.Enrichers.Environment" Version="3.0.0" />
  </ItemGroup>
</Project>

<!-- BFF.Application.csproj -->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <!-- AutoMapper -->
    <PackageReference Include="AutoMapper" Version="13.0.0" />
    <PackageReference Include="AutoMapper.Extensions.Microsoft.DependencyInjection" Version="13.0.0" />
    
    <!-- FluentValidation -->
    <PackageReference Include="FluentValidation" Version="11.9.0" />
    <PackageReference Include="FluentValidation.DependencyInjectionExtensions" Version="11.9.0" />
  </ItemGroup>
</Project>

<!-- BFF.Infrastructure.csproj -->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <!-- HTTP Client -->
    <PackageReference Include="Microsoft.Extensions.Http" Version="10.0.0" />
    <PackageReference Include="Microsoft.Extensions.Http.Polly" Version="10.0.0" />
    
    <!-- Polly (Resilience) -->
    <PackageReference Include="Polly" Version="8.4.0" />
    <PackageReference Include="Polly.Extensions.Http" Version="3.0.0" />
    
    <!-- JSON -->
    <PackageReference Include="System.Text.Json" Version="10.0.0" />
    
    <!-- Caching -->
    <PackageReference Include="Microsoft.Extensions.Caching.Memory" Version="10.0.0" />
    
    <!-- Dynatrace -->
    <PackageReference Include="Dynatrace.OneAgent.Sdk" Version="1.8.0" />
  </ItemGroup>
</Project>
```

---

## 🎯 Fase 2: Implementación de la Capa de Dominio

### 2.1 Modelos de Dominio (Sin Lógica de Negocio)

```csharp
// BFF.Domain/Models/Common/BaseEntity.cs
namespace BFF.Domain.Models.Common;

public abstract class BaseEntity
{
    public string Id { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

// BFF.Domain/Models/User.cs
namespace BFF.Domain.Models;

public class User : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public bool IsActive { get; set; }
}

// BFF.Domain/Models/Order.cs
namespace BFF.Domain.Models;

public class Order : BaseEntity
{
    public string UserId { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public OrderStatus Status { get; set; }
    public List<OrderItem> Items { get; set; } = new();
}

public class OrderItem
{
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Subtotal { get; set; }
}

// BFF.Domain/Models/Payment.cs
namespace BFF.Domain.Models;

public class Payment : BaseEntity
{
    public string OrderId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public PaymentStatus Status { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string TransactionId { get; set; } = string.Empty;
}

// BFF.Domain/Models/Common/ErrorResult.cs
namespace BFF.Domain.Models.Common;

public class ErrorResult
{
    public string Code { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public Dictionary<string, string[]>? Errors { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
```

### 2.2 Enumeraciones

```csharp
// BFF.Domain/Enums/OrderStatus.cs
namespace BFF.Domain.Enums;

public enum OrderStatus
{
    Pending = 0,
    Confirmed = 1,
    Processing = 2,
    Shipped = 3,
    Delivered = 4,
    Cancelled = 5
}

// BFF.Domain/Enums/PaymentStatus.cs
namespace BFF.Domain.Enums;

public enum PaymentStatus
{
    Pending = 0,
    Processing = 1,
    Completed = 2,
    Failed = 3,
    Refunded = 4
}

// BFF.Domain/Enums/UserRole.cs
namespace BFF.Domain.Enums;

public enum UserRole
{
    Customer = 0,
    Admin = 1,
    Manager = 2
}
```

### 2.3 Constantes

```csharp
// BFF.Domain/Constants/GraphQLConstants.cs
namespace BFF.Domain.Constants;

public static class GraphQLConstants
{
    public const string QueryOperationType = "query";
    public const string MutationOperationType = "mutation";
    
    public static class Services
    {
        public const string Users = "UsersService";
        public const string Orders = "OrdersService";
        public const string Payments = "PaymentsService";
    }
    
    public static class Endpoints
    {
        public const string GraphQL = "/graphql";
    }
}

// BFF.Domain/Constants/ApiRoutes.cs
namespace BFF.Domain.Constants;

public static class ApiRoutes
{
    public const string BaseRoute = "api";
    
    public static class Users
    {
        public const string Base = $"{BaseRoute}/users";
        public const string GetById = "{id}";
        public const string Create = "";
        public const string Update = "{id}";
        public const string Delete = "{id}";
    }
    
    public static class Orders
    {
        public const string Base = $"{BaseRoute}/orders";
        public const string GetById = "{id}";
        public const string GetByUserId = "user/{userId}";
        public const string Create = "";
        public const string Update = "{id}";
        public const string Cancel = "{id}/cancel";
    }
    
    public static class Payments
    {
        public const string Base = $"{BaseRoute}/payments";
        public const string GetById = "{id}";
        public const string Process = "";
        public const string Refund = "{id}/refund";
    }
}
```

---

## 🎯 Fase 3: Implementación de la Capa de Aplicación

### 3.1 DTOs (Data Transfer Objects)

```csharp
// BFF.Application/DTOs/Requests/CreateUserRequest.cs
namespace BFF.Application.DTOs.Requests;

public class CreateUserRequest
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
}

// BFF.Application/DTOs/Responses/UserResponse.cs
namespace BFF.Application.DTOs.Responses;

public class UserResponse
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

// BFF.Application/DTOs/Responses/OrderResponse.cs
namespace BFF.Application.DTOs.Responses;

public class OrderResponse
{
    public string Id { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public string Status { get; set; } = string.Empty;
    public List<OrderItemResponse> Items { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}

public class OrderItemResponse
{
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Subtotal { get; set; }
}

// BFF.Application/DTOs/GraphQL/UserGraphQLResponse.cs
namespace BFF.Application.DTOs.GraphQL;

public class UserGraphQLResponse
{
    public UserData? Data { get; set; }
    public List<GraphQLError>? Errors { get; set; }
}

public class UserData
{
    public UserGraphQL? User { get; set; }
}

public class UserGraphQL
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class GraphQLError
{
    public string Message { get; set; } = string.Empty;
    public List<ErrorLocation>? Locations { get; set; }
    public List<string>? Path { get; set; }
}

public class ErrorLocation
{
    public int Line { get; set; }
    public int Column { get; set; }
}
```

### 3.2 Validadores (FluentValidation)

```csharp
// BFF.Application/Validators/CreateUserRequestValidator.cs
using FluentValidation;

namespace BFF.Application.Validators;

public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters");
        
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(255).WithMessage("Email must not exceed 255 characters");
        
        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Phone is required")
            .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone format");
    }
}

// BFF.Application/Validators/CreateOrderRequestValidator.cs
using FluentValidation;

namespace BFF.Application.Validators;

public class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderRequestValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required");
        
        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("At least one item is required")
            .Must(items => items.Count > 0).WithMessage("Order must contain at least one item");
        
        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(x => x.ProductId).NotEmpty().WithMessage("ProductId is required");
            item.RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than 0");
            item.RuleFor(x => x.UnitPrice).GreaterThan(0).WithMessage("UnitPrice must be greater than 0");
        });
    }
}
```

### 3.3 Mappings (AutoMapper)

```csharp
// BFF.Application/Mappings/UserMappingProfile.cs
using AutoMapper;

namespace BFF.Application.Mappings;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        // Domain → Response
        CreateMap<User, UserResponse>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));
        
        // GraphQL → Domain
        CreateMap<UserGraphQL, User>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => Enum.Parse<UserRole>(src.Role)));
        
        // Request → Domain
        CreateMap<CreateUserRequest, User>();
    }
}

// BFF.Application/Mappings/OrderMappingProfile.cs
using AutoMapper;

namespace BFF.Application.Mappings;

public class OrderMappingProfile : Profile
{
    public OrderMappingProfile()
    {
        CreateMap<Order, OrderResponse>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
        
        CreateMap<OrderItem, OrderItemResponse>();
        
        CreateMap<OrderGraphQL, Order>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Enum.Parse<OrderStatus>(src.Status)));
    }
}
```

### 3.4 Application Services

```csharp
// BFF.Application/Services/Interfaces/IUserService.cs
namespace BFF.Application.Services.Interfaces;

public interface IUserService
{
    Task<UserResponse?> GetUserByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<List<UserResponse>> GetUsersAsync(CancellationToken cancellationToken = default);
    Task<UserResponse> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default);
    Task<UserResponse> UpdateUserAsync(string id, UpdateUserRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteUserAsync(string id, CancellationToken cancellationToken = default);
}

// BFF.Application/Services/UserService.cs
using AutoMapper;
using BFF.Infrastructure.GraphQL.Clients;

namespace BFF.Application.Services;

public class UserService : IUserService
{
    private readonly IUsersGraphQLClient _graphQLClient;
    private readonly IMapper _mapper;
    private readonly ILogger<UserService> _logger;
    private readonly IDynatraceService _dynatraceService;

    public UserService(
        IUsersGraphQLClient graphQLClient,
        IMapper mapper,
        ILogger<UserService> logger,
        IDynatraceService dynatraceService)
    {
        _graphQLClient = graphQLClient;
        _mapper = mapper;
        _logger = logger;
        _dynatraceService = dynatraceService;
    }

    public async Task<UserResponse?> GetUserByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        using var activity = _dynatraceService.StartActivity("UserService.GetUserById", id);
        
        try
        {
            _logger.LogInformation("Fetching user with ID: {UserId}", id);
            
            var graphQLResponse = await _graphQLClient.GetUserByIdAsync(id, cancellationToken);
            
            if (graphQLResponse?.Data?.User == null)
            {
                _logger.LogWarning("User with ID {UserId} not found", id);
                return null;
            }
            
            var user = _mapper.Map<User>(graphQLResponse.Data.User);
            var response = _mapper.Map<UserResponse>(user);
            
            _dynatraceService.RecordMetric("user.get.success", 1);
            
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching user with ID: {UserId}", id);
            _dynatraceService.RecordMetric("user.get.error", 1);
            _dynatraceService.RecordException(ex);
            throw;
        }
    }

    public async Task<UserResponse> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        using var activity = _dynatraceService.StartActivity("UserService.CreateUser", request.Email);
        
        try
        {
            _logger.LogInformation("Creating user with email: {Email}", request.Email);
            
            var graphQLResponse = await _graphQLClient.CreateUserAsync(request, cancellationToken);
            
            if (graphQLResponse?.Data?.CreateUser == null)
            {
                throw new GraphQLClientException("Failed to create user");
            }
            
            var user = _mapper.Map<User>(graphQLResponse.Data.CreateUser);
            var response = _mapper.Map<UserResponse>(user);
            
            _dynatraceService.RecordMetric("user.create.success", 1);
            
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user with email: {Email}", request.Email);
            _dynatraceService.RecordMetric("user.create.error", 1);
            _dynatraceService.RecordException(ex);
            throw;
        }
    }
}
```

### 3.5 Excepciones Personalizadas

```csharp
// BFF.Application/Exceptions/GraphQLClientException.cs
namespace BFF.Application.Exceptions;

public class GraphQLClientException : Exception
{
    public List<GraphQLError>? GraphQLErrors { get; }

    public GraphQLClientException(string message) : base(message)
    {
    }

    public GraphQLClientException(string message, List<GraphQLError> errors) : base(message)
    {
        GraphQLErrors = errors;
    }

    public GraphQLClientException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

// BFF.Application/Exceptions/ValidationException.cs
namespace BFF.Application.Exceptions;

public class ValidationException : Exception
{
    public Dictionary<string, string[]> Errors { get; }

    public ValidationException(Dictionary<string, string[]> errors) 
        : base("One or more validation errors occurred.")
    {
        Errors = errors;
    }
}

// BFF.Application/Exceptions/ServiceUnavailableException.cs
namespace BFF.Application.Exceptions;

public class ServiceUnavailableException : Exception
{
    public string ServiceName { get; }

    public ServiceUnavailableException(string serviceName, string message) : base(message)
    {
        ServiceName = serviceName;
    }

    public ServiceUnavailableException(string serviceName, string message, Exception innerException) 
        : base(message, innerException)
    {
        ServiceName = serviceName;
    }
}
```

---

## 🎯 Fase 4: Implementación de la Capa de Infraestructura

### 4.1 GraphQL Clients

```csharp
// BFF.Infrastructure/GraphQL/Models/GraphQLRequest.cs
namespace BFF.Infrastructure.GraphQL.Models;

public class GraphQLRequest
{
    public string Query { get; set; } = string.Empty;
    public object? Variables { get; set; }
    public string? OperationName { get; set; }
}

// BFF.Infrastructure/GraphQL/Queries/UserQueries.cs
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

    public const string GetUsers = @"
        query GetUsers {
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

// BFF.Infrastructure/GraphQL/Mutations/UserMutations.cs
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
                updatedAt
            }
        }";

    public const string DeleteUser = @"
        mutation DeleteUser($id: ID!) {
            deleteUser(id: $id)
        }";
}

// BFF.Infrastructure/GraphQL/Clients/IUsersGraphQLClient.cs
namespace BFF.Infrastructure.GraphQL.Clients;

public interface IUsersGraphQLClient
{
    Task<UserGraphQLResponse?> GetUserByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<UsersGraphQLResponse?> GetUsersAsync(CancellationToken cancellationToken = default);
    Task<CreateUserGraphQLResponse?> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default);
    Task<UpdateUserGraphQLResponse?> UpdateUserAsync(string id, UpdateUserRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteUserAsync(string id, CancellationToken cancellationToken = default);
}

// BFF.Infrastructure/GraphQL/Clients/UsersGraphQLClient.cs
using System.Net.Http.Json;
using System.Text.Json;

namespace BFF.Infrastructure.GraphQL.Clients;

public class UsersGraphQLClient : IUsersGraphQLClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<UsersGraphQLClient> _logger;
    private readonly IDynatraceService _dynatraceService;
    private readonly JsonSerializerOptions _jsonOptions;

    public UsersGraphQLClient(
        HttpClient httpClient,
        ILogger<UsersGraphQLClient> logger,
        IDynatraceService dynatraceService)
    {
        _httpClient = httpClient;
        _logger = logger;
        _dynatraceService = dynatraceService;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<UserGraphQLResponse?> GetUserByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        using var activity = _dynatraceService.StartActivity("UsersGraphQLClient.GetUserById", id);
        
        var request = new GraphQLRequest
        {
            Query = UserQueries.GetUserById,
            Variables = new { id }
        };

        try
        {
            _logger.LogDebug("Sending GraphQL query to Users service: {Query}", UserQueries.GetUserById);
            
            var response = await _httpClient.PostAsJsonAsync("/graphql", request, cancellationToken);
            
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<UserGraphQLResponse>(_jsonOptions, cancellationToken);
            
            if (result?.Errors != null && result.Errors.Any())
            {
                _logger.LogWarning("GraphQL query returned errors: {Errors}", 
                    JsonSerializer.Serialize(result.Errors));
                throw new GraphQLClientException("GraphQL query failed", result.Errors);
            }
            
            _dynatraceService.RecordMetric("graphql.users.get.success", 1);
            
            return result;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error calling Users service");
            _dynatraceService.RecordMetric("graphql.users.get.error", 1);
            _dynatraceService.RecordException(ex);
            throw new ServiceUnavailableException("UsersService", "Failed to connect to Users service", ex);
        }
    }

    public async Task<CreateUserGraphQLResponse?> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        using var activity = _dynatraceService.StartActivity("UsersGraphQLClient.CreateUser", request.Email);
        
        var graphqlRequest = new GraphQLRequest
        {
            Query = UserMutations.CreateUser,
            Variables = new
            {
                input = new
                {
                    name = request.Name,
                    email = request.Email,
                    phone = request.Phone
                }
            }
        };

        try
        {
            _logger.LogDebug("Sending GraphQL mutation to Users service: {Mutation}", UserMutations.CreateUser);
            
            var response = await _httpClient.PostAsJsonAsync("/graphql", graphqlRequest, cancellationToken);
            
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<CreateUserGraphQLResponse>(_jsonOptions, cancellationToken);
            
            if (result?.Errors != null && result.Errors.Any())
            {
                _logger.LogWarning("GraphQL mutation returned errors: {Errors}", 
                    JsonSerializer.Serialize(result.Errors));
                throw new GraphQLClientException("GraphQL mutation failed", result.Errors);
            }
            
            _dynatraceService.RecordMetric("graphql.users.create.success", 1);
            
            return result;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error calling Users service");
            _dynatraceService.RecordMetric("graphql.users.create.error", 1);
            _dynatraceService.RecordException(ex);
            throw new ServiceUnavailableException("UsersService", "Failed to connect to Users service", ex);
        }
    }
}
```

### 4.2 Resilience Policies (Polly)

```csharp
// BFF.Infrastructure/Resilience/RetryPolicies.cs
using Polly;
using Polly.Extensions.Http;

namespace BFF.Infrastructure.Resilience;

public static class RetryPolicies
{
    public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (outcome, timespan, retryCount, context) =>
                {
                    var logger = context.GetLogger();
                    logger?.LogWarning(
                        "Retry {RetryCount} after {Delay}s due to {StatusCode}",
                        retryCount,
                        timespan.TotalSeconds,
                        outcome.Result?.StatusCode);
                });
    }
}

// BFF.Infrastructure/Resilience/CircuitBreakerPolicies.cs
using Polly;
using Polly.Extensions.Http;

namespace BFF.Infrastructure.Resilience;

public static class CircuitBreakerPolicies
{
    public static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: 5,
                durationOfBreak: TimeSpan.FromSeconds(30),
                onBreak: (outcome, breakDelay) =>
                {
                    // Log circuit breaker opened
                },
                onReset: () =>
                {
                    // Log circuit breaker reset
                },
                onHalfOpen: () =>
                {
                    // Log circuit breaker half-open
                });
    }
}

// BFF.Infrastructure/Resilience/TimeoutPolicies.cs
using Polly;
using Polly.Timeout;

namespace BFF.Infrastructure.Resilience;

public static class TimeoutPolicies
{
    public static IAsyncPolicy<HttpResponseMessage> GetTimeoutPolicy()
    {
        return Policy.TimeoutAsync<HttpResponseMessage>(
            timeout: TimeSpan.FromSeconds(30),
            timeoutStrategy: TimeoutStrategy.Optimistic,
            onTimeoutAsync: (context, timespan, task) =>
            {
                var logger = context.GetLogger();
                logger?.LogWarning("Request timed out after {Timeout}s", timespan.TotalSeconds);
                return Task.CompletedTask;
            });
    }
}
```

### 4.3 Telemetría Dynatrace

```csharp
// BFF.Infrastructure/Telemetry/IDynatraceService.cs
namespace BFF.Infrastructure.Telemetry;

public interface IDynatraceService
{
    IDisposable StartActivity(string name, string? identifier = null);
    void RecordMetric(string metricName, double value);
    void RecordException(Exception exception);
    void AddCustomProperty(string key, string value);
}

// BFF.Infrastructure/Telemetry/DynatraceService.cs
using Dynatrace.OneAgent.Sdk.Api;
using Dynatrace.OneAgent.Sdk.Api.Enums;
using System.Diagnostics;

namespace BFF.Infrastructure.Telemetry;

public class DynatraceService : IDynatraceService
{
    private readonly IOneAgentSdk _oneAgent;
    private readonly ILogger<DynatraceService> _logger;
    private readonly ActivitySource _activitySource;

    public DynatraceService(
        IOneAgentSdk oneAgent,
        ILogger<DynatraceService> logger)
    {
        _oneAgent = oneAgent;
        _logger = logger;
        _activitySource = new ActivitySource("BFF.Service");
    }

    public IDisposable StartActivity(string name, string? identifier = null)
    {
        var activity = _activitySource.StartActivity(name, ActivityKind.Internal);
        
        if (activity != null && !string.IsNullOrEmpty(identifier))
        {
            activity.SetTag("identifier", identifier);
        }

        // Dynatrace custom service
        var tracer = _oneAgent.TraceIncomingRemoteCall(
            name,
            "BFF",
            identifier ?? string.Empty);
        
        tracer.Start();

        return new ActivityScope(activity, tracer);
    }

    public void RecordMetric(string metricName, double value)
    {
        try
        {
            // OpenTelemetry metric
            Activity.Current?.SetTag($"metric.{metricName}", value);

            _logger.LogDebug("Recorded metric: {MetricName} = {Value}", metricName, value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording metric: {MetricName}", metricName);
        }
    }

    public void RecordException(Exception exception)
    {
        try
        {
            Activity.Current?.SetStatus(ActivityStatusCode.Error, exception.Message);
            Activity.Current?.RecordException(exception);

            _logger.LogError(exception, "Exception recorded in telemetry");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording exception");
        }
    }

    public void AddCustomProperty(string key, string value)
    {
        Activity.Current?.SetTag(key, value);
    }

    private class ActivityScope : IDisposable
    {
        private readonly Activity? _activity;
        private readonly IIncomingRemoteCallTracer _tracer;

        public ActivityScope(Activity? activity, IIncomingRemoteCallTracer tracer)
        {
            _activity = activity;
            _tracer = tracer;
        }

        public void Dispose()
        {
            _tracer.End();
            _activity?.Dispose();
        }
    }
}

// BFF.Infrastructure/Telemetry/TraceContextPropagator.cs
using System.Diagnostics;

namespace BFF.Infrastructure.Telemetry;

public class TraceContextPropagator : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var activity = Activity.Current;
        
        if (activity != null)
        {
            // Propagate trace context to downstream services
            request.Headers.Add("traceparent", activity.Id);
            request.Headers.Add("tracestate", activity.TraceStateString);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
```

### 4.4 Configuration

```csharp
// BFF.Infrastructure/Configuration/GraphQLServiceSettings.cs
namespace BFF.Infrastructure.Configuration;

public class GraphQLServiceSettings
{
    public string UsersServiceUrl { get; set; } = string.Empty;
    public string OrdersServiceUrl { get; set; } = string.Empty;
    public string PaymentsServiceUrl { get; set; } = string.Empty;
    public int TimeoutSeconds { get; set; } = 30;
    public int RetryCount { get; set; } = 3;
}

// BFF.Infrastructure/Configuration/DynatraceSettings.cs
namespace BFF.Infrastructure.Configuration;

public class DynatraceSettings
{
    public string TenantId { get; set; } = string.Empty;
    public string ApiToken { get; set; } = string.Empty;
    public string EnvironmentUrl { get; set; } = string.Empty;
    public bool Enabled { get; set; } = true;
}
```

---

## 🎯 Fase 5: Implementación de la Capa de API

### 5.1 Controllers

```csharp
// BFF.API/Controllers/UsersController.cs
using Microsoft.AspNetCore.Mvc;

namespace BFF.API.Controllers;

[ApiController]
[Route(ApiRoutes.Users.Base)]
[Produces("application/json")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;
    private readonly IDynatraceService _dynatraceService;

    public UsersController(
        IUserService userService,
        ILogger<UsersController> logger,
        IDynatraceService dynatraceService)
    {
        _userService = userService;
        _logger = logger;
        _dynatraceService = dynatraceService;
    }

    /// <summary>
    /// Get user by ID
    /// </summary>
    [HttpGet(ApiRoutes.Users.GetById)]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResult), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UserResponse>> GetUser(
        [FromRoute] string id,
        CancellationToken cancellationToken)
    {
        using var activity = _dynatraceService.StartActivity("UsersController.GetUser", id);
        
        _logger.LogInformation("Getting user with ID: {UserId}", id);
        
        var user = await _userService.GetUserByIdAsync(id, cancellationToken);
        
        if (user == null)
        {
            _logger.LogWarning("User with ID {UserId} not found", id);
            return NotFound(new ErrorResult
            {
                Code = "USER_NOT_FOUND",
                Message = $"User with ID {id} not found"
            });
        }
        
        return Ok(user);
    }

    /// <summary>
    /// Get all users
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<UserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResult), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<UserResponse>>> GetUsers(CancellationToken cancellationToken)
    {
        using var activity = _dynatraceService.StartActivity("UsersController.GetUsers");
        
        _logger.LogInformation("Getting all users");
        
        var users = await _userService.GetUsersAsync(cancellationToken);
        
        return Ok(users);
    }

    /// <summary>
    /// Create new user
    /// </summary>
    [HttpPost(ApiRoutes.Users.Create)]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResult), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UserResponse>> CreateUser(
        [FromBody] CreateUserRequest request,
        CancellationToken cancellationToken)
    {
        using var activity = _dynatraceService.StartActivity("UsersController.CreateUser", request.Email);
        
        _logger.LogInformation("Creating user with email: {Email}", request.Email);
        
        var user = await _userService.CreateUserAsync(request, cancellationToken);
        
        return CreatedAtAction(
            nameof(GetUser),
            new { id = user.Id },
            user);
    }

    /// <summary>
    /// Update user
    /// </summary>
    [HttpPut(ApiRoutes.Users.Update)]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResult), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UserResponse>> UpdateUser(
        [FromRoute] string id,
        [FromBody] UpdateUserRequest request,
        CancellationToken cancellationToken)
    {
        using var activity = _dynatraceService.StartActivity("UsersController.UpdateUser", id);
        
        _logger.LogInformation("Updating user with ID: {UserId}", id);
        
        var user = await _userService.UpdateUserAsync(id, request, cancellationToken);
        
        return Ok(user);
    }

    /// <summary>
    /// Delete user
    /// </summary>
    [HttpDelete(ApiRoutes.Users.Delete)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResult), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteUser(
        [FromRoute] string id,
        CancellationToken cancellationToken)
    {
        using var activity = _dynatraceService.StartActivity("UsersController.DeleteUser", id);
        
        _logger.LogInformation("Deleting user with ID: {UserId}", id);
        
        var deleted = await _userService.DeleteUserAsync(id, cancellationToken);
        
        if (!deleted)
        {
            return NotFound(new ErrorResult
            {
                Code = "USER_NOT_FOUND",
                Message = $"User with ID {id} not found"
            });
        }
        
        return NoContent();
    }
}
```

### 5.2 Middleware

```csharp
// BFF.API/Middleware/ExceptionHandlingMiddleware.cs
using System.Net;
using System.Text.Json;

namespace BFF.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IDynatraceService _dynatraceService;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IDynatraceService dynatraceService)
    {
        _next = next;
        _logger = logger;
        _dynatraceService = dynatraceService;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation error occurred");
            await HandleExceptionAsync(context, ex, HttpStatusCode.BadRequest);
        }
        catch (GraphQLClientException ex)
        {
            _logger.LogError(ex, "GraphQL client error occurred");
            _dynatraceService.RecordException(ex);
            await HandleExceptionAsync(context, ex, HttpStatusCode.BadGateway);
        }
        catch (ServiceUnavailableException ex)
        {
            _logger.LogError(ex, "Service unavailable: {ServiceName}", ex.ServiceName);
            _dynatraceService.RecordException(ex);
            await HandleExceptionAsync(context, ex, HttpStatusCode.ServiceUnavailable);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred");
            _dynatraceService.RecordException(ex);
            await HandleExceptionAsync(context, ex, HttpStatusCode.InternalServerError);
        }
    }

    private static async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception,
        HttpStatusCode statusCode)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var errorResult = new ErrorResult
        {
            Code = statusCode.ToString(),
            Message = exception.Message,
            Timestamp = DateTime.UtcNow
        };

        if (exception is ValidationException validationException)
        {
            errorResult.Errors = validationException.Errors;
        }

        var json = JsonSerializer.Serialize(errorResult, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}

// BFF.API/Middleware/RequestLoggingMiddleware.cs
using System.Diagnostics;

namespace BFF.API.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(
        RequestDelegate next,
        ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var requestId = Activity.Current?.Id ?? context.TraceIdentifier;

        _logger.LogInformation(
            "HTTP {Method} {Path} started - RequestId: {RequestId}",
            context.Request.Method,
            context.Request.Path,
            requestId);

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();

            _logger.LogInformation(
                "HTTP {Method} {Path} completed with {StatusCode} in {ElapsedMs}ms - RequestId: {RequestId}",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds,
                requestId);
        }
    }
}
```

### 5.3 Filters

```csharp
// BFF.API/Filters/ValidationFilter.cs
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BFF.API.Filters;

public class ValidationFilter : IAsyncActionFilter
{
    private readonly IServiceProvider _serviceProvider;

    public ValidationFilter(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        foreach (var argument in context.ActionArguments.Values)
        {
            if (argument == null)
                continue;

            var validatorType = typeof(IValidator<>).MakeGenericType(argument.GetType());
            var validator = _serviceProvider.GetService(validatorType) as IValidator;

            if (validator != null)
            {
                var validationContext = new ValidationContext<object>(argument);
                var validationResult = await validator.ValidateAsync(validationContext);

                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors
                        .GroupBy(x => x.PropertyName)
                        .ToDictionary(
                            g => g.Key,
                            g => g.Select(x => x.ErrorMessage).ToArray());

                    context.Result = new BadRequestObjectResult(new ErrorResult
                    {
                        Code = "VALIDATION_ERROR",
                        Message = "One or more validation errors occurred",
                        Errors = errors
                    });
                    return;
                }
            }
        }

        await next();
    }
}
```

### 5.4 Program.cs

```csharp
// BFF.API/Program.cs
using BFF.API.Extensions;
using BFF.API.Filters;
using BFF.API.Middleware;
using BFF.Application.Mappings;
using BFF.Application.Services;
using BFF.Application.Services.Interfaces;
using BFF.Application.Validators;
using BFF.Infrastructure.Configuration;
using BFF.Infrastructure.GraphQL.Clients;
using BFF.Infrastructure.Resilience;
using BFF.Infrastructure.Telemetry;
using Dynatrace.OneAgent.Sdk.Api;
using FluentValidation;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// ========================================
// Serilog Configuration
// ========================================
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithEnvironmentName()
    .Enrich.WithMachineName()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// ========================================
// Configuration
// ========================================
var graphQLSettings = builder.Configuration
    .GetSection("GraphQLServices")
    .Get<GraphQLServiceSettings>() ?? new GraphQLServiceSettings();

var dynatraceSettings = builder.Configuration
    .GetSection("Dynatrace")
    .Get<DynatraceSettings>() ?? new DynatraceSettings();

builder.Services.Configure<GraphQLServiceSettings>(
    builder.Configuration.GetSection("GraphQLServices"));
builder.Services.Configure<DynatraceSettings>(
    builder.Configuration.GetSection("Dynatrace"));

// ========================================
// Controllers & API
// ========================================
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() 
    { 
        Title = "BFF API", 
        Version = "v1",
        Description = "Backend for Frontend - REST to GraphQL Gateway"
    });
});

// ========================================
// CORS
// ========================================
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// ========================================
// AutoMapper
// ========================================
builder.Services.AddAutoMapper(typeof(UserMappingProfile).Assembly);

// ========================================
// FluentValidation
// ========================================
builder.Services.AddValidatorsFromAssemblyContaining<CreateUserRequestValidator>();

// ========================================
// Application Services
// ========================================
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();

// ========================================
// GraphQL HTTP Clients
// ========================================
builder.Services.AddHttpClient<IUsersGraphQLClient, UsersGraphQLClient>(client =>
{
    client.BaseAddress = new Uri(graphQLSettings.UsersServiceUrl);
    client.Timeout = TimeSpan.FromSeconds(graphQLSettings.TimeoutSeconds);
})
.AddHttpMessageHandler<TraceContextPropagator>()
.AddPolicyHandler(RetryPolicies.GetRetryPolicy())
.AddPolicyHandler(CircuitBreakerPolicies.GetCircuitBreakerPolicy())
.AddPolicyHandler(TimeoutPolicies.GetTimeoutPolicy());

builder.Services.AddHttpClient<IOrdersGraphQLClient, OrdersGraphQLClient>(client =>
{
    client.BaseAddress = new Uri(graphQLSettings.OrdersServiceUrl);
    client.Timeout = TimeSpan.FromSeconds(graphQLSettings.TimeoutSeconds);
})
.AddHttpMessageHandler<TraceContextPropagator>()
.AddPolicyHandler(RetryPolicies.GetRetryPolicy())
.AddPolicyHandler(CircuitBreakerPolicies.GetCircuitBreakerPolicy())
.AddPolicyHandler(TimeoutPolicies.GetTimeoutPolicy());

builder.Services.AddHttpClient<IPaymentsGraphQLClient, PaymentsGraphQLClient>(client =>
{
    client.BaseAddress = new Uri(graphQLSettings.PaymentsServiceUrl);
    client.Timeout = TimeSpan.FromSeconds(graphQLSettings.TimeoutSeconds);
})
.AddHttpMessageHandler<TraceContextPropagator>()
.AddPolicyHandler(RetryPolicies.GetRetryPolicy())
.AddPolicyHandler(CircuitBreakerPolicies.GetCircuitBreakerPolicy())
.AddPolicyHandler(TimeoutPolicies.GetTimeoutPolicy());

builder.Services.AddTransient<TraceContextPropagator>();

// ========================================
// Caching
// ========================================
builder.Services.AddMemoryCache();

// ========================================
// Dynatrace & OpenTelemetry
// ========================================
if (dynatraceSettings.Enabled)
{
    builder.Services.AddSingleton(OneAgentSdkFactory.CreateInstance());
    builder.Services.AddSingleton<IDynatraceService, DynatraceService>();

    builder.Services.AddOpenTelemetry()
        .WithTracing(tracerProviderBuilder =>
        {
            tracerProviderBuilder
                .SetResourceBuilder(ResourceBuilder.CreateDefault()
                    .AddService("BFF.API")
                    .AddAttributes(new Dictionary<string, object>
                    {
                        ["deployment.environment"] = builder.Environment.EnvironmentName
                    }))
                .AddAspNetCoreInstrumentation(options =>
                {
                    options.RecordException = true;
                })
                .AddHttpClientInstrumentation()
                .AddSource("BFF.Service")
                .AddOtlpExporter(options =>
                {
                    options.Endpoint = new Uri($"{dynatraceSettings.EnvironmentUrl}/api/v2/otlp/v1/traces");
                    options.Headers = $"Authorization=Api-Token {dynatraceSettings.ApiToken}";
                });
        });
}
else
{
    builder.Services.AddSingleton<IDynatraceService, NoOpDynatraceService>();
}

// ========================================
// Health Checks
// ========================================
builder.Services.AddHealthChecks()
    .AddUrlGroup(new Uri($"{graphQLSettings.UsersServiceUrl}/health"), "Users Service")
    .AddUrlGroup(new Uri($"{graphQLSettings.OrdersServiceUrl}/health"), "Orders Service")
    .AddUrlGroup(new Uri($"{graphQLSettings.PaymentsServiceUrl}/health"), "Payments Service");

var app = builder.Build();

// ========================================
// Middleware Pipeline
// ========================================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health");

// ========================================
// Startup
// ========================================
try
{
    Log.Information("Starting BFF API");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
```

### 5.5 appsettings.json

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    }
  },
  "GraphQLServices": {
    "UsersServiceUrl": "http://users-service:5001",
    "OrdersServiceUrl": "http://orders-service:5002",
    "PaymentsServiceUrl": "http://payments-service:5003",
    "TimeoutSeconds": 30,
    "RetryCount": 3
  },
  "Dynatrace": {
    "TenantId": "your-tenant-id",
    "ApiToken": "your-api-token",
    "EnvironmentUrl": "https://your-tenant.live.dynatrace.com",
    "Enabled": true
  },
  "AllowedHosts": "*"
}
```

---

## 🎯 Fase 6: Containerización (Docker)

### 6.1 Dockerfile

```dockerfile
# docker/Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["BFF.Solution.sln", "./"]
COPY ["src/BFF.API/BFF.API.csproj", "src/BFF.API/"]
COPY ["src/BFF.Application/BFF.Application.csproj", "src/BFF.Application/"]
COPY ["src/BFF.Infrastructure/BFF.Infrastructure.csproj", "src/BFF.Infrastructure/"]
COPY ["src/BFF.Domain/BFF.Domain.csproj", "src/BFF.Domain/"]

# Restore dependencies
RUN dotnet restore "src/BFF.API/BFF.API.csproj"

# Copy source code
COPY src/ ./src/

# Build
WORKDIR "/src/src/BFF.API"
RUN dotnet build "BFF.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BFF.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Dynatrace OneAgent (opcional)
# ENV DT_TENANT=your-tenant-id
# ENV DT_TENANTTOKEN=your-tenant-token
# ENV DT_CONNECTION_POINT=https://your-tenant.live.dynatrace.com

ENTRYPOINT ["dotnet", "BFF.API.dll"]
```

### 6.2 docker-compose.yml

```yaml
# docker/docker-compose.yml
version: '3.8'

services:
  bff-api:
    build:
      context: ..
      dockerfile: docker/Dockerfile
    container_name: bff-api
    ports:
      - "5000:8080"
      - "5001:8081"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ASPNETCORE_URLS=http://+:8080;https://+:8081
      - GraphQLServices__UsersServiceUrl=http://users-service:5001
      - GraphQLServices__OrdersServiceUrl=http://orders-service:5002
      - GraphQLServices__PaymentsServiceUrl=http://payments-service:5003
      - Dynatrace__TenantId=${DYNATRACE_TENANT_ID}
      - Dynatrace__ApiToken=${DYNATRACE_API_TOKEN}
      - Dynatrace__EnvironmentUrl=${DYNATRACE_ENV_URL}
    depends_on:
      - users-service
      - orders-service
      - payments-service
    networks:
      - bff-network

  users-service:
    image: users-service:latest
    container_name: users-service
    ports:
      - "5001:5001"
    networks:
      - bff-network

  orders-service:
    image: orders-service:latest
    container_name: orders-service
    ports:
      - "5002:5002"
    networks:
      - bff-network

  payments-service:
    image: payments-service:latest
    container_name: payments-service
    ports:
      - "5003:5003"
    networks:
      - bff-network

networks:
  bff-network:
    driver: bridge
```

---

## 🎯 Fase 7: Kubernetes Deployment

### 7.1 Deployment

```yaml
# k8s/deployment.yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: bff-api
  namespace: default
  labels:
    app: bff-api
    version: v1
spec:
  replicas: 3
  selector:
    matchLabels:
      app: bff-api
  template:
    metadata:
      labels:
        app: bff-api
        version: v1
    spec:
      containers:
      - name: bff-api
        image: your-registry/bff-api:latest
        imagePullPolicy: Always
        ports:
        - containerPort: 8080
          name: http
          protocol: TCP
        env:
        - name: ASPNETCORE_ENVIRONMENT
          value: "Production"
        - name: ASPNETCORE_URLS
          value: "http://+:8080"
        - name: GraphQLServices__UsersServiceUrl
          valueFrom:
            configMapKeyRef:
              name: bff-config
              key: users-service-url
        - name: GraphQLServices__OrdersServiceUrl
          valueFrom:
            configMapKeyRef:
              name: bff-config
              key: orders-service-url
        - name: GraphQLServices__PaymentsServiceUrl
          valueFrom:
            configMapKeyRef:
              name: bff-config
              key: payments-service-url
        - name: Dynatrace__TenantId
          valueFrom:
            secretKeyRef:
              name: dynatrace-secret
              key: tenant-id
        - name: Dynatrace__ApiToken
          valueFrom:
            secretKeyRef:
              name: dynatrace-secret
              key: api-token
        - name: Dynatrace__EnvironmentUrl
          valueFrom:
            secretKeyRef:
              name: dynatrace-secret
              key: environment-url
        resources:
          requests:
            memory: "256Mi"
            cpu: "250m"
          limits:
            memory: "512Mi"
            cpu: "500m"
        livenessProbe:
          httpGet:
            path: /health
            port: 8080
          initialDelaySeconds: 30
          periodSeconds: 10
        readinessProbe:
          httpGet:
            path: /health
            port: 8080
          initialDelaySeconds: 10
          periodSeconds: 5
```

### 7.2 Service

```yaml
# k8s/service.yaml
apiVersion: v1
kind: Service
metadata:
  name: bff-api-service
  namespace: default
  labels:
    app: bff-api
spec:
  type: ClusterIP
  ports:
  - port: 80
    targetPort: 8080
    protocol: TCP
    name: http
  selector:
    app: bff-api
```

### 7.3 ConfigMap

```yaml
# k8s/configmap.yaml
apiVersion: v1
kind: ConfigMap
metadata:
  name: bff-config
  namespace: default
data:
  users-service-url: "http://users-service:5001"
  orders-service-url: "http://orders-service:5002"
  payments-service-url: "http://payments-service:5003"
```

### 7.4 Secret

```yaml
# k8s/secret.yaml
apiVersion: v1
kind: Secret
metadata:
  name: dynatrace-secret
  namespace: default
type: Opaque
stringData:
  tenant-id: "your-tenant-id"
  api-token: "your-api-token"
  environment-url: "https://your-tenant.live.dynatrace.com"
```

### 7.5 Ingress

```yaml
# k8s/ingress.yaml
apiVersion: networking.k8s.io/v1
kind: Ingress
metadata:
  name: bff-api-ingress
  namespace: default
  annotations:
    kubernetes.io/ingress.class: nginx
    cert-manager.io/cluster-issuer: letsencrypt-prod
spec:
  tls:
  - hosts:
    - api.yourdomain.com
    secretName: bff-api-tls
  rules:
  - host: api.yourdomain.com
    http:
      paths:
      - path: /
        pathType: Prefix
        backend:
          service:
            name: bff-api-service
            port:
              number: 80
```

### 7.6 HorizontalPodAutoscaler

```yaml
# k8s/hpa.yaml
apiVersion: autoscaling/v2
kind: HorizontalPodAutoscaler
metadata:
  name: bff-api-hpa
  namespace: default
spec:
  scaleTargetRef:
    apiVersion: apps/v1
    kind: Deployment
    name: bff-api
  minReplicas: 3
  maxReplicas: 10
  metrics:
  - type: Resource
    resource:
      name: cpu
      target:
        type: Utilization
        averageUtilization: 70
  - type: Resource
    resource:
      name: memory
      target:
        type: Utilization
        averageUtilization: 80
```

---

## 🎯 Fase 8: CI/CD Pipeline

### 8.1 GitHub Actions

```yaml
# .github/workflows/ci-cd.yml
name: CI/CD Pipeline

on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main ]

env:
  DOTNET_VERSION: '10.0.x'
  REGISTRY: ghcr.io
  IMAGE_NAME: ${{ github.repository }}/bff-api

jobs:
  build-and-test:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v3
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: ${{ env.DOTNET_VERSION }}
    
    - name: Restore dependencies
      run: dotnet restore
    
    - name: Build
      run: dotnet build --configuration Release --no-restore
    
    - name: Test
      run: dotnet test --no-restore --verbosity normal --collect:"XPlat Code Coverage"
    
    - name: Upload coverage reports
      uses: codecov/codecov-action@v3
      with:
        files: ./tests/**/coverage.cobertura.xml

  docker-build-push:
    needs: build-and-test
    runs-on: ubuntu-latest
    if: github.event_name == 'push'
    
    permissions:
      contents: read
      packages: write
    
    steps:
    - uses: actions/checkout@v3
    
    - name: Log in to Container Registry
      uses: docker/login-action@v2
      with:
        registry: ${{ env.REGISTRY }}
        username: ${{ github.actor }}
        password: ${{ secrets.GITHUB_TOKEN }}
    
    - name: Extract metadata
      id: meta
      uses: docker/metadata-action@v4
      with:
        images: ${{ env.REGISTRY }}/${{ env.IMAGE_NAME }}
        tags: |
          type=ref,event=branch
          type=sha
          type=semver,pattern={{version}}
    
    - name: Build and push Docker image
      uses: docker/build-push-action@v4
      with:
        context: .
        file: ./docker/Dockerfile
        push: true
        tags: ${{ steps.meta.outputs.tags }}
        labels: ${{ steps.meta.outputs.labels }}

  deploy-to-k8s:
    needs: docker-build-push
    runs-on: ubuntu-latest
    if: github.ref == 'refs/heads/main'
    
    steps:
    - uses: actions/checkout@v3
    
    - name: Set up kubectl
      uses: azure/setup-kubectl@v3
    
    - name: Configure kubectl
      run: |
        echo "${{ secrets.KUBE_CONFIG }}" | base64 -d > kubeconfig.yaml
        export KUBECONFIG=kubeconfig.yaml
    
    - name: Deploy to Kubernetes
      run: |
        kubectl apply -f k8s/configmap.yaml
        kubectl apply -f k8s/secret.yaml
        kubectl apply -f k8s/deployment.yaml
        kubectl apply -f k8s/service.yaml
        kubectl apply -f k8s/ingress.yaml
        kubectl apply -f k8s/hpa.yaml
        kubectl rollout status deployment/bff-api
```

---

## 📋 Checklist de Implementación

### ✅ Fase 1: Setup
- [ ] Crear estructura de proyectos
- [ ] Instalar NuGet packages
- [ ] Configurar referencias entre proyectos

### ✅ Fase 2: Domain Layer
- [ ] Implementar modelos de dominio
- [ ] Crear enumeraciones
- [ ] Definir constantes

### ✅ Fase 3: Application Layer
- [ ] Crear DTOs (Request/Response/GraphQL)
- [ ] Implementar validadores FluentValidation
- [ ] Configurar AutoMapper profiles
- [ ] Implementar Application Services
- [ ] Definir excepciones personalizadas

### ✅ Fase 4: Infrastructure Layer
- [ ] Implementar GraphQL clients
- [ ] Crear queries y mutations
- [ ] Configurar Polly policies (retry, circuit breaker, timeout)
- [ ] Implementar telemetría Dynatrace
- [ ] Configurar trace propagation
- [ ] Implementar caching

### ✅ Fase 5: API Layer
- [ ] Implementar REST controllers
- [ ] Crear middleware (Exception, Logging)
- [ ] Implementar validation filters
- [ ] Configurar Program.cs
- [ ] Configurar Swagger/OpenAPI
- [ ] Configurar health checks
- [ ] Configurar appsettings.json

### ✅ Fase 6: Testing
- [ ] Unit tests para Application Services
- [ ] Unit tests para Controllers
- [ ] Integration tests para GraphQL clients
- [ ] Load testing

### ✅ Fase 7: Containerización
- [ ] Crear Dockerfile
- [ ] Crear docker-compose.yml
- [ ] Probar contenedores localmente

### ✅ Fase 8: Kubernetes
- [ ] Crear manifests (Deployment, Service, ConfigMap, Secret)
- [ ] Configurar Ingress
- [ ] Configurar HPA
- [ ] Configurar health checks

### ✅ Fase 9: CI/CD
- [ ] Configurar GitHub Actions
- [ ] Configurar automated tests
- [ ] Configurar Docker build/push
- [ ] Configurar Kubernetes deployment

### ✅ Fase 10: Observabilidad
- [ ] Configurar Dynatrace OneAgent
- [ ] Configurar OpenTelemetry
- [ ] Configurar logging con Serilog
- [ ] Configurar métricas personalizadas
- [ ] Configurar alertas

---

## 🎯 Comandos Útiles

### Desarrollo Local

```bash
# Restaurar dependencias
dotnet restore

# Compilar solución
dotnet build

# Ejecutar tests
dotnet test

# Ejecutar API
cd src/BFF.API
dotnet run

# Watch mode (hot reload)
dotnet watch run
```

### Docker

```bash
# Build imagen
docker build -t bff-api:latest -f docker/Dockerfile .

# Run contenedor
docker run -d -p 5000:8080 --name bff-api bff-api:latest

# Ver logs
docker logs -f bff-api

# Docker Compose
cd docker
docker-compose up -d
docker-compose logs -f
docker-compose down
```

### Kubernetes

```bash
# Aplicar manifests
kubectl apply -f k8s/

# Ver pods
kubectl get pods -l app=bff-api

# Ver logs
kubectl logs -f deployment/bff-api

# Port forward
kubectl port-forward service/bff-api-service 5000:80

# Escalar
kubectl scale deployment bff-api --replicas=5

# Ver HPA
kubectl get hpa

# Describir pod
kubectl describe pod <pod-name>
```

---

## 📊 Monitoreo con Dynatrace

### Métricas Clave

1. **Request Rate**: Requests por segundo
2. **Response Time**: P50, P95, P99
3. **Error Rate**: Errores por minuto
4. **GraphQL Client Performance**:
   - Latencia por servicio (Users, Orders, Payments)
   - Tasa de éxito/fallo
   - Circuit breaker status
5. **Resource Usage**:
   - CPU utilization
   - Memory usage
   - HTTP connection pool

### Dashboards

1. **BFF Overview**
   - Request rate
   - Response time distribution
   - Error rate
   - Active connections

2. **GraphQL Services**
   - Latency por servicio
   - Success/failure rate
   - Circuit breaker events
   - Retry attempts

3. **Infrastructure**
   - Pod count
   - CPU/Memory usage
   - Network I/O
   - HPA activity

---

## 🔒 Seguridad

### Consideraciones

1. **API Security**:
   - Implementar autenticación JWT (próxima fase)
   - Rate limiting
   - CORS configuration
   - Input validation

2. **Secrets Management**:
   - Usar Kubernetes Secrets
   - Azure Key Vault (opcional)
   - No hardcodear credentials

3. **Network Security**:
   - TLS/HTTPS
   - Network policies
   - Service mesh (Istio/Linkerd)

---

## 📝 Próximos Pasos

### Fase 11: Autenticación y Autorización
- [ ] Implementar JWT authentication
- [ ] Configurar OAuth2/OpenID Connect
- [ ] Implementar authorization policies

### Fase 12: Caché Distribuido
- [ ] Implementar Redis cache
- [ ] Configurar cache invalidation
- [ ] Implementar cache-aside pattern

### Fase 13: Rate Limiting
- [ ] Implementar rate limiting por IP
- [ ] Implementar rate limiting por usuario
- [ ] Configurar throttling policies

### Fase 14: Event-Driven (Opcional)
- [ ] Agregar AMQP client (RabbitMQ)
- [ ] Implementar event publishing
- [ ] Configurar event consumers

---

**Versión del Plan**: 1.0  
**Fecha**: Enero 2026  
**Arquitectura**: REST Frontend + GraphQL/HTTP Backend (Arquitectura 3)
