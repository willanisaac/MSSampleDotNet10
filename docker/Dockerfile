# Build stage
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

# Publish
FROM build AS publish
RUN dotnet publish "BFF.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

COPY --from=publish /app/publish .

# Create logs directory
RUN mkdir -p /app/logs

# Environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "BFF.API.dll"]
