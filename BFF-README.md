# BFF (Backend for Frontend) - .NET 10 Clean Architecture

![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)
![Architecture](https://img.shields.io/badge/Architecture-Clean-blue)
![License](https://img.shields.io/badge/License-MIT-green)

## 📋 Descripción

Backend for Frontend (BFF) implementado con .NET 10 siguiendo los principios de Clean Architecture. Este proyecto actúa como intermediario entre aplicaciones frontend (Angular, Mobile) y microservicios backend que exponen APIs GraphQL.

### Arquitectura

```
┌─────────────────────────────────────────────────────────────┐
│                    [REST] FRONTEND LAYER                    │
│  ┌──────────────────┐         ┌───────────────────────┐     │
│  │   Angular SPA    │         │  Mobile (Kotlin/Swift)│     │
│  │   (HttpClient)   │         │ (Retrofit/Alamofire)  │     │
│  └────────┬─────────┘         └──────────┬────────────┘     │
└───────────┼──────────────────────────────┼──────────────────┘
            │                              │
            │         REST API             │
            ▼                              ▼
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
                        ▼
┌─────────────────────────────────────────────────────────────┐
│          [GQL] MICROSERVICES (.NET 10 - GraphQL/HTTP)       │
│  ┌──────────────┐  ┌──────────────┐  ┌─────────────────┐    │
│  │    Users     │  │    Orders    │  │    Payments     │    │
│  │   Service    │  │   Service    │  │    Service      │    │
│  │ (GraphQL API)│  │ (GraphQL API)│  │  (GraphQL API)  │    │
│  └──────────────┘  └──────────────┘  └─────────────────┘    │
└─────────────────────────────────────────────────────────────┘
```

## 🏗️ Estructura del Proyecto

```
BFF.Solution/
├── src/
│   ├── BFF.API/                      # 🌐 Capa de Presentación
│   │   ├── Controllers/              # REST API Controllers
│   │   ├── Middleware/               # Exception handling, Logging
│   │   ├── Filters/                  # Validation filters
│   │   └── Program.cs                # Application startup
│   │
│   ├── BFF.Application/              # 📋 Capa de Aplicación
│   │   ├── DTOs/                     # Data Transfer Objects
│   │   ├── Services/                 # Business logic services
│   │   ├── Validators/               # FluentValidation rules
│   │   ├── Mappings/                 # AutoMapper profiles
│   │   └── Exceptions/               # Custom exceptions
│   │
│   ├── BFF.Infrastructure/           # 🔧 Capa de Infraestructura
│   │   ├── GraphQL/                  # GraphQL clients
│   │   ├── Resilience/               # Polly policies
│   │   └── Configuration/            # Settings
│   │
│   └── BFF.Domain/                   # 📦 Capa de Dominio
│       ├── Models/                   # Domain models
│       ├── Enums/                    # Enumerations
│       └── Constants/                # Constants
│
├── tests/                            # Unit & Integration tests
├── docker/                           # Docker configuration
├── k8s/                              # Kubernetes manifests
└── .github/workflows/                # CI/CD pipelines
```

## 🚀 Características

- ✅ Clean Architecture
- ✅ .NET 10
- ✅ REST API con Swagger/OpenAPI
- ✅ GraphQL Client para microservicios
- ✅ FluentValidation para validación de datos
- ✅ AutoMapper para mapeo de objetos
- ✅ Polly para resilencia (Retry, Circuit Breaker, Timeout)
- ✅ Serilog para logging estructurado
- ✅ Health checks
- ✅ Docker & Docker Compose
- ✅ Kubernetes (Deployment, Service, Ingress, HPA)
- ✅ GitHub Actions CI/CD

## 🛠️ Requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Docker](https://www.docker.com/) (opcional)
- [Kubernetes](https://kubernetes.io/) (opcional)

## 📦 Instalación

### 1. Clonar el repositorio

```bash
git clone https://github.com/your-repo/bff-dotnet10.git
cd bff-dotnet10
```

### 2. Restaurar dependencias

```bash
dotnet restore
```

### 3. Configurar servicios backend

Editar `src/BFF.API/appsettings.Development.json`:

```json
{
  "GraphQLServices": {
    "UsersServiceUrl": "http://localhost:5001",
    "OrdersServiceUrl": "http://localhost:5002",
    "PaymentsServiceUrl": "http://localhost:5003",
    "TimeoutSeconds": 30,
    "RetryCount": 3
  }
}
```

### 4. Ejecutar la aplicación

```bash
cd src/BFF.API
dotnet run
```

La aplicación estará disponible en:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`
- Swagger UI: `http://localhost:5000/swagger`

## 🐳 Docker

### Construir imagen

```bash
docker build -t bff-api:latest -f docker/Dockerfile .
```

### Ejecutar contenedor

```bash
docker run -d -p 5000:8080 --name bff-api bff-api:latest
```

### Docker Compose

```bash
cd docker
docker-compose up -d
```

## ☸️ Kubernetes

### Aplicar manifests

```bash
kubectl apply -f k8s/configmap.yaml
kubectl apply -f k8s/deployment.yaml
kubectl apply -f k8s/service.yaml
kubectl apply -f k8s/ingress.yaml
kubectl apply -f k8s/hpa.yaml
```

### Ver estado del deployment

```bash
kubectl get pods -l app=bff-api
kubectl logs -f deployment/bff-api
```

## 📚 API Endpoints

### Users

- `GET /api/users` - Obtener todos los usuarios
- `GET /api/users/{id}` - Obtener usuario por ID
- `POST /api/users` - Crear usuario
- `PUT /api/users/{id}` - Actualizar usuario
- `DELETE /api/users/{id}` - Eliminar usuario

### Orders

- `GET /api/orders/{id}` - Obtener orden por ID
- `GET /api/orders/user/{userId}` - Obtener órdenes por usuario
- `POST /api/orders` - Crear orden
- `POST /api/orders/{id}/cancel` - Cancelar orden

### Payments

- `GET /api/payments/{id}` - Obtener pago por ID
- `GET /api/payments/order/{orderId}` - Obtener pagos por orden
- `POST /api/payments` - Procesar pago
- `POST /api/payments/{id}/refund` - Reembolsar pago

## 🧪 Testing

```bash
# Ejecutar todos los tests
dotnet test

# Ejecutar tests con cobertura
dotnet test --collect:"XPlat Code Coverage"
```

## 📝 Logs

Los logs se almacenan en:
- Console (desarrollo)
- `logs/bff-YYYYMMDD.txt` (archivo)

Configuración en `appsettings.json`:

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
  }
}
```

## 🔄 CI/CD

El proyecto incluye un pipeline de GitHub Actions que:

1. **Build & Test** - Compila y ejecuta tests
2. **Code Analysis** - Análisis estático de código
3. **Docker Build** - Construye y publica imagen Docker
4. **Deploy Dev** - Despliega a ambiente de desarrollo
5. **Deploy Prod** - Despliega a producción

## 🤝 Contribuir

1. Fork el proyecto
2. Crear una rama (`git checkout -b feature/AmazingFeature`)
3. Commit cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abrir un Pull Request

## 📄 Licencia

Este proyecto está bajo la licencia MIT. Ver el archivo [LICENSE](LICENSE) para más detalles.

## 👥 Autores

- Tu Nombre - [@yourhandle](https://github.com/yourhandle)

## 🙏 Agradecimientos

- Clean Architecture por Robert C. Martin
- .NET Team por .NET 10
- Comunidad Open Source
