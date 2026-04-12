# Pokémon Classifier API

A sophisticated Pokémon classification API built with .NET 10.0 ASP.NET Core using hexagonal (ports and adapters) architecture. The API leverages AI models for intelligent Pokemon classification based on image data, featuring intelligent caching with Redis for optimal performance.

## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Architecture](#architecture)
- [Technology Stack](#technology-stack)
- [Project Structure](#project-structure)
- [Setup Instructions](#setup-instructions)
- [Configuration](#configuration)
- [Running the Application](#running-the-application)
- [API Endpoints](#api-endpoints)
- [Caching Strategy](#caching-strategy)
- [Dependency Injection](#dependency-injection)
- [Contributing](#contributing)
- [License](#license)

## Overview

The Pokémon Classifier API is a RESTful service that classifies Pokémon based on image data and descriptions. It integrates with external AI models to provide intelligent classification capabilities while maintaining high performance through intelligent Redis-based caching.

## Features

- **Pokémon Image Classification**: Classify Pokémon based on image data using AI models
- **Intelligent Caching**: Redis-based caching with SHA256 hashing for fast response times
- **Generic Model Integration**: Provider-agnostic model integration supporting any compatible API
- **Health Check Endpoint**: Built-in health monitoring
- **Comprehensive Logging**: Structured logging for debugging and monitoring
- **Configuration Management**: Options pattern for flexible configuration
- **OpenAPI/Swagger Documentation**: Built-in API documentation with OpenAPI support
- **Error Handling**: Robust error handling with appropriate HTTP status codes

## Architecture

The application follows the **Hexagonal Architecture** pattern (Ports and Adapters), which ensures:

- **Independence**: The domain logic is independent of external frameworks and databases
- **Testability**: Each layer can be tested independently
- **Flexibility**: Easy to swap implementations without changing core logic

### Architecture Diagram

```
┌─────────────────────────────────────────────────────────────┐
│                     HTTP Clients                             │
└────────┬────────────────────────────────────────────┬────────┘
         │                                            │
         ▼                                            ▼
    ┌─────────────┐                          ┌──────────────┐
    │ Classifier  │                          │    Model     │
    │  Adapter    │                          │   Controller │
    │  (Inbound)  │                          │  (Inbound)   │
    └──────┬──────┘                          └──────┬───────┘
           │                                        │
           ▼                                        ▼
    ┌──────────────────────────────────────────────────────┐
    │           Ports (Domain Interfaces)                   │
    ├─────────────────────┬────────────────────────────────┤
    │ ClassifierPort      │ ModelPort                      │
    │ HealthCheckPort     │ NoSqlDatabasePort              │
    └─────────────────────┴────────────────────────────────┘
           ▲                      ▲
           │                      │
    ┌──────┴──────┐    ┌─────────┴──────────┬──────────────┐
    │ Interceptor │    │ ModelAdapter       │ DatabaseAdapter
    │             │    │ (Outbound)         │ (Outbound)
    │             │    │                    │
    └──────┬──────┘    └────────┬───────────┴──────────────┘
           │                    │            │
           └────────┬───────────┘            │
                    │                        │
                    ▼                        ▼
            ┌───────────────┐      ┌──────────────────┐
            │  Model        │      │   Redis          │
            │  Service API  │      │   Database       │
            │  (e.g. Ollama)│      │                  │
            └───────────────┘      └──────────────────┘
```

### Layer Responsibilities

1. **Adapters (Inbound)**: Handle incoming HTTP requests
   - `ClassifierAdapter`: Manages Pokémon classification requests
   - `HealthCheckAdapter`: Provides health status

2. **Ports**: Define domain contracts
   - `ClassifierPort`: Interface for classification operations
   - `ModelPort`: Interface for external model communication
   - `NoSqlDatabasePort`: Interface for caching operations
   - `HealthCheckPort`: Interface for health checks

3. **Domain (Core Business Logic)**:
   - `ClassifierInterceptor`: Orchestrates classification workflow
   - `ClassifierEntity`: Domain entity for classification

4. **Adapters (Outbound)**: Implement external integrations
   - `ModelAdapter`: Communicates with external AI models (generic)
   - `NoSqlDatabaseAdapter`: Manages Redis caching

## Technology Stack

| Component | Technology | Version |
|-----------|-----------|---------|
| Runtime | .NET | 10.0 |
| Framework | ASP.NET Core | 10.0.2 |
| API Documentation | Microsoft.AspNetCore.OpenApi | 10.0.2 |
| Caching | StackExchange.Redis | 2.10.1 |
| AI Integration | Microsoft.Extensions.AI | 10.2.0 |
| Language | C# | Latest |

## Project Structure

```
pokedex-classifier/
├── api/                              # ASP.NET Core API
│   ├── Adapters/
│   │   ├── Inbound/
│   │   │   ├── ClassifierAdapter.cs     # HTTP request handler for classification
│   │   │   └── HealthCheckAdapter.cs    # Health status endpoint
│   │   └── Outbound/
│   │       ├── ModelAdapter.cs          # External model communication
│   │       └── NoSqlDatabaseAdapter.cs  # Redis caching implementation
│   ├── Ports/
│   │   ├── Inbound/
│   │   │   ├── ClassifierPort.cs        # Classification interface
│   │   │   └── HealthCheckPort.cs       # Health check interface
│   │   └── Outbound/
│   │       ├── ModelPort.cs             # Model service interface
│   │       └── NoSqlDatabasePort.cs     # Cache service interface
│   ├── Domain/
│   │   ├── Entities/
│   │   │   ├── ClassifierEntity.cs      # Domain entity
│   │   │   └── ClassifierEntityImpl.cs   # Entity implementation
│   │   └── DTOs/
│   │       ├── PostClassifyPokemonImageDto.cs
│   │       ├── PostClassifyPokemonImageResponseDto.cs
│   │       ├── ModelGenerateRequest.cs
│   │       └── ModelGenerateResponse.cs
│   ├── Interceptors/
│   │   ├── ClassifierInterceptor.cs     # Business logic orchestration
│   │   └── Impl/
│   │       └── ClassifierInterceptorImpl.cs
│   ├── Controllers/
│   │   └── ModelController.cs           # Direct model access endpoints
│   ├── Configuration/
│   │   └── ModelOptions.cs              # Configuration options
│   ├── appsettings.json                 # Default configuration
│   ├── appsettings.Development.json    # Development configuration
│   ├── Program.cs                       # Application entry point
│   └── api.csproj                      # Project file
├── docker-compose.yaml                 # Development compose file
├── docker-compose.production.yaml       # Production compose file
├── Makefile                            # Build and run commands
├── pokedex-classifier.sln              # Solution file
└── README.md                           # This file
```

## Setup Instructions

### Prerequisites

- .NET 10.0 SDK
- Docker and Docker Compose
- Redis (or use Docker)
- Git

### Clone the Repository

```bash
git clone https://github.com/matheusdoedev/pokedex-classifier.git
cd pokedex-classifier
```

### Local Development Setup

#### 1. Install Dependencies

```bash
cd api
dotnet restore
```

#### 2. Set Up Redis

Using Docker:
```bash
docker run -d -p 6379:6379 --name pokedex-redis redis:7-alpine redis-server --appendonly yes
```

Or use Docker Compose:
```bash
docker-compose up -d no-sql-db
```

#### 3. Set Up Model Service

You need to set up a compatible model service. The default configuration supports services with an API endpoint compatible with Ollama's API format.

**Option A: Using Ollama (Recommended)**

1. Install Ollama from https://ollama.ai
2. Pull a model:
   ```bash
   ollama pull llama2
   # or for Pokemon classification
   ollama pull JJMack/pokemon_gen1_9_classifier
   ```
3. Run Ollama (it exposes an API on port 11434):
   ```bash
   ollama serve
   ```

**Option B: Using Docker**

```bash
docker run -d -p 11434:11434 --name pokedex-ollama ollama/ollama:latest
docker exec pokedex-ollama ollama pull llama2
```

**Option C: Custom Model Service**

Configure the API to point to your custom model service by updating `appsettings.json` (see Configuration section).

#### 4. Configure the Application

Update `api/appsettings.Development.json` if needed:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "Redis": {
    "ConnectionString": "localhost:6379"
  },
  "Model": {
    "Endpoint": "http://localhost:11434",
    "ModelName": "llama2",
    "TimeoutSeconds": 30
  }
}
```

#### 5. Build and Run

```bash
cd api
dotnet build
dotnet run
```

The API will be available at `https://localhost:5001` (HTTPS) or `http://localhost:5000` (HTTP).

## Configuration

### Model Configuration (ModelOptions)

The application uses the Options pattern to configure model settings. Configuration is loaded from the `Model` section in `appsettings.json`.

#### Configuration Options

| Key | Type | Default | Description |
|-----|------|---------|-------------|
| `Endpoint` | string | `http://localhost:11434` | The URL endpoint of the model service API |
| `ModelName` | string | `llama2` | The name of the model to use for inference |
| `TimeoutSeconds` | int | 30 | Request timeout in seconds (1-300) |

#### Environment Variables

You can override configuration using environment variables:

```bash
export Model__Endpoint=http://your-model-service:11434
export Model__ModelName=pokemon-classifier
export Model__TimeoutSeconds=60

export Redis__ConnectionString=redis-prod:6379
```

#### Configuration Validation

Model configuration is validated at startup:
- Endpoint must be a valid URL
- ModelName cannot be empty
- TimeoutSeconds must be between 1 and 300 seconds

If validation fails, the application will throw an `InvalidOperationException` on startup.

### Redis Configuration

Configure Redis connection in the `Redis` section:

```json
{
  "Redis": {
    "ConnectionString": "localhost:6379"
  }
}
```

For production with authentication:
```json
{
  "Redis": {
    "ConnectionString": "default:password@redis-prod:6379"
  }
}
```

## Running the Application

### Development Mode

```bash
# Using .NET CLI
cd api
dotnet run

# Using Docker Compose (all services)
docker-compose up

# Using Makefile
make up
```

### Production Mode

```bash
# Using Docker Compose with production settings
docker-compose -f docker-compose.production.yaml up -d

# Using Makefile
make up-production
```

### Stopping Services

```bash
# Using Docker Compose
docker-compose down --remove-orphans

# Using Makefile
make down
```

## API Endpoints

### Classifier Endpoints

#### 1. Classify Pokémon Image

**Endpoint**: `POST /api/classifier/classify`

Classifies a Pokémon based on image data with automatic caching.

**Request Body**:
```json
{
  "image": "base64-encoded-image-or-image-data"
}
```

**Response (200 OK)**:
```json
{
  "response": "Classification result from the model"
}
```

**Error Responses**:
- `400 Bad Request`: Invalid or null request body
- `503 Service Unavailable`: Model service is unavailable
- `500 Internal Server Error`: Unexpected error

**Caching**: 
- Responses are cached based on SHA256 hash of the image
- Subsequent requests with the same image return cached results instantly

### Model Controller Endpoints

#### 1. Send Prompt to Model

**Endpoint**: `POST /api/model/prompt`

Send a raw prompt directly to the model service for inference.

**Request Body**:
```json
{
  "prompt": "What are the characteristics of Pikachu?"
}
```

**Response (200 OK)**:
```json
{
  "response": "Model generated response"
}
```

**Error Responses**:
- `400 Bad Request`: Prompt is empty or null
- `500 Internal Server Error`: Error processing prompt

### 2. Classify Pokémon by Description

**Endpoint**: `POST /api/model/classify-pokemon`

Classify a Pokémon based on image description without caching (direct model access).

**Request Body**:
```json
{
  "imageDescription": "Yellow electric mouse with red cheeks and lightning tail"
}
```

**Response (200 OK)**:
```json
{
  "classification": "Model classification result"
}
```

**Error Responses**:
- `400 Bad Request`: Image description is empty or null
- `500 Internal Server Error`: Error classifying Pokémon

### Health Check Endpoint

#### 1. Health Status

**Endpoint**: `GET /`

Returns the current UTC timestamp to verify the API is running.

**Response (200 OK)**:
```
2024-04-12T10:30:45.1234567Z
```

### Swagger/OpenAPI Documentation

**Available at**: `GET /openapi/v1.json` (Development mode)

View the interactive API documentation in development mode at your API's root URL.

## Caching Strategy

The application implements an intelligent caching strategy to optimize performance:

### Cache Workflow

```
Client Request
    ↓
Generate SHA256 hash of image data
    ↓
Check if hash exists in Redis
    ↓
┌─── Hash Found ──→ Return cached response ──→ Response (O(1) lookup)
│
└─── Hash Not Found ──→ Send to Model Service ──→ Receive response
                              ↓
                        Store in Redis with hash as key
                              ↓
                        Return response to client
```

### Implementation Details

**Location**: `ClassifierInterceptorImpl.cs`

**Key Components**:
- **Hashing**: SHA256 hashing of Base64-encoded image data
- **Key Generation**: Hash becomes the cache key in Redis
- **Storage**: Both request hash and model response stored
- **Retrieval**: Check cache before model invocation

**Code Flow**:
```csharp
1. Hash the image: SHA256(imageBase64)
2. Check if hash exists: noSqlDatabase.Exists(hash)
3. If exists: return cached response
4. If not: call model, cache result, return
```

### Benefits

- **Performance**: Eliminates redundant model service calls for identical images
- **Cost**: Reduces API calls to external model services
- **Scalability**: Reduces load on model service
- **Consistency**: Ensures identical inputs produce identical outputs

### Cache Expiration

Currently, Redis stores cache without expiration (TTL). For production, consider implementing:

```bash
# Set expiration policy in Redis configuration
maxmemory-policy allkeys-lru  # Evict least recently used keys
```

## Dependency Injection

The application uses ASP.NET Core's built-in dependency injection container configured in `Program.cs`.

### Registered Services

**Configuration Options**:
```csharp
builder.Services.Configure<ModelOptions>(
    builder.Configuration.GetSection(ModelOptions.SectionName));
```

**Redis Connection**:
```csharp
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    string? config = builder.Configuration.GetSection("Redis")["ConnectionString"]
        ?? throw new ArgumentException("redis connection string is null");
    
    return ConnectionMultiplexer.Connect(config);
});
```

**HTTP Client for Model Service**:
```csharp
builder.Services.AddHttpClient<ModelPort, ModelAdapter>()
    .ConfigureHttpClient(client =>
    {
        var modelConfig = builder.Configuration.GetSection(ModelOptions.SectionName);
        var timeoutSeconds = modelConfig.GetValue<int?>("TimeoutSeconds") ?? 30;
        client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
    });
```

**Ports & Adapters**:
```csharp
builder.Services.AddScoped<NoSqlDatabasePort, NoSqlDatabaseAdapter>();
builder.Services.AddScoped<ClassifierInterceptor, ClassifierInterceptorImpl>();
```

### Service Lifetimes

- **Singleton**: `IConnectionMultiplexer` (Redis connection - reused across requests)
- **Scoped**: `NoSqlDatabasePort`, `ClassifierInterceptor` (one per request)
- **Transient**: Controllers, HTTP clients (created on each request)

## Design Patterns

### 1. Hexagonal Architecture (Ports and Adapters)

Separates the application into three layers:
- **Core Domain**: Contains business logic independent of infrastructure
- **Ports**: Define interfaces for external interactions
- **Adapters**: Implement ports to connect with external systems

**Benefits**:
- Testability: Mock adapters for unit testing
- Flexibility: Swap implementations without changing core logic
- Independence: Core logic doesn't depend on frameworks

### 2. Dependency Injection Pattern

Uses ASP.NET Core's DI container to manage dependencies.

**Benefits**:
- Loose coupling between components
- Easy to test with mocks
- Configuration-driven behavior

### 3. Options Pattern

Configuration is loaded via `IOptions<ModelOptions>`.

**Benefits**:
- Type-safe configuration
- Validation at startup
- Easy to extend with new settings

### 4. Repository Pattern

`NoSqlDatabasePort` acts as a repository for cache operations.

**Benefits**:
- Abstracts data access logic
- Easy to swap storage implementations
- Consistent interface for cache operations

### 5. Adapter Pattern

`ModelAdapter` adapts external model service API to internal `ModelPort` interface.

**Benefits**:
- Isolates external API changes
- Enables easy switching between model providers
- Transforms external format to internal format

## Contributing

We welcome contributions to the Pokémon Classifier API! Please follow these guidelines:

### Getting Started

1. Fork the repository
2. Create a feature branch: `git checkout -b feature/your-feature-name`
3. Make your changes
4. Commit with meaningful messages: `git commit -m "feat: description"`
5. Push to your fork: `git push origin feature/your-feature-name`
6. Create a Pull Request

### Code Standards

- Follow C# naming conventions (PascalCase for classes, camelCase for variables)
- Add XML documentation comments to public classes and methods
- Include unit tests for new functionality
- Ensure all tests pass before submitting PR
- Keep commits atomic and well-described

### Commit Message Format

```
<type>: <subject>

<body>

<footer>
```

**Types**:
- `feat`: A new feature
- `fix`: A bug fix
- `docs`: Documentation only
- `refactor`: Code refactoring without feature changes
- `test`: Adding or updating tests
- `chore`: Build process, dependencies, or tooling

**Example**:
```
feat: add pokemon name extraction from model response

- Extract pokemon name from model response
- Validate extracted name against known pokemon list
- Cache both full response and extracted name

Closes #123
```

### Testing

Before submitting a PR, ensure:

```bash
cd api
dotnet test
dotnet build
```

### Pull Request Guidelines

1. Provide a clear description of your changes
2. Reference any related issues
3. Ensure CI/CD checks pass
4. Request review from maintainers

### Reporting Issues

When reporting issues, please include:
- Clear description of the problem
- Steps to reproduce
- Expected vs actual behavior
- Environment details (OS, .NET version, etc.)
- Relevant logs or screenshots

## License

This project is licensed under the MIT License. See LICENSE file for details.

---

## Getting Help

- **Documentation**: See the Setup and Configuration sections above
- **Issues**: Create an issue on GitHub
- **Discussions**: Use GitHub Discussions for questions
- **API Docs**: Access OpenAPI/Swagger docs in development mode

## Support

For questions or support, please create an issue on the GitHub repository.

---

**Last Updated**: April 12, 2026

**Maintainer**: matheusdoedev
