# EShopMicroservices

# Port Numbers for Catalog.API Microservice listed as http/https

| Microservices | Local Env   | Docker Env  | Docker Inside |
|---------------|-----------  |------------ |---------------|
| Catalog       | 5000 – 5050 | 6000 – 6060 | 8080 – 8081   |
| Basket        | 5001 – 5051 | 6001 – 6061 | 8080 – 8081   |
| Discount      | 5002 – 5052 | 6002 – 6062 | 8080 – 8081   |
| Ordering      | 5003 – 5053 | 6003 – 6063 | 8080 – 8081   |



# 3. Rest API Endpoints of Catalog Microservices

| Method     | Request URI          | Use Cases                |
|--------    |-------------         |-----------               |
| **GET**    | `/products`          | List all products        |
| **GET**    | `/products/{id}`     | Fetch a specific product |
| **GET**    | `/products/category` | Get products by category |
| **POST**   | `/products`          | Create a new product     |
| **PUT**    | `/products/{id}`     | Update a product         |
| **DELETE** | `/products/{id}`     | Remove a product         |


# 4. Underlying Data Structures of Catalog Microservices

- **Document Database** with Store Catalog **JSON** data.
- **2 options:**
  - MongoDB NoSQL database
  - PostgreSQL DB JSON Columns
  - **Chosen:** PostgreSQL with the **Marten** library.
- **Marten** is a powerful library that transforms **PostgreSQL** into a **.NET Transactional Document DB**.
- **PostgreSQL's JSON column features** allow us to store and query data as JSON documents.
- Combines the **flexibility of a document database** with the **reliability of a relational PostgreSQL database**.


# Big Picture – Project

```text
┌──────────────────┐
│   Client Apps    │
│                  │
│  Shopping.Web    │
│     (.NET 10)    │
└────────┬─────────┘
         │
         ▼
┌───────────────────────┐
│     API Gateway       │
│   YARP API Gateway    │
└────────┬──────────────┘
         │
         ├──────────────────────────────────────────────────────┐
         ▼                                                      ▼
┌────────────────┐   ┌────────────────┐   ┌────────────────┐   ┌────────────────┐
│    Catalog     │   │     Basket     │   │    Discount    │   │    Ordering    │
│                │   │                │   │                │   │                │
│ PostgreSQL     │   │ PostgreSQL     │   │ SQLite         │   │ SQL Server     │
│ Clean Arch.    │   │ Redis Cache    │   │                │   │                │
└────────────────┘   └────────────────┘   └────────────────┘   └────────────────┘
         │                  │                  │                  │
         └──────────────────┴──────────────────┴──────────────────┘
                                │
                                ▼
                        ┌────────────────┐
                        │    RabbitMQ    │
                        │ Message Broker │
                        └────────────────┘
```

## Architecture Overview

### Client Application
- **Shopping.Web**
- Built with **.NET 10**
- Sends all requests through the API Gateway.

### API Gateway
- **YARP API Gateway**
- Single entry point for all client requests.
- Routes requests to the appropriate microservice.

### Microservices

| Microservice | Database / Storage | Notes |
|--------------|--------------------|-------|
| **Catalog** | PostgreSQL | Uses Clean Architecture |
| **Basket** | PostgreSQL + Redis | Shopping cart with distributed cache |
| **Discount** | SQLite | Coupon/discount service |
| **Ordering** | SQL Server | Order management service |

### Messaging
- **RabbitMQ**
  - Asynchronous communication
  - Event-driven integration between services

### Technologies
- .NET 10
- ASP.NET Core
- YARP API Gateway
- PostgreSQL
- Redis
- SQLite
- SQL Server
- RabbitMQ
- gRPC
- MassTransit
- MediatR
- Entity Framework Core
- Docker


## Catalog Microservice Internal Architecture

```text
Client
  │
  │ External IP and Port
  ▼
┌──────────────────────────────────────────────────────────────┐
│                      Docker Environment                      │
│                                                              │
│  Internal IP and Port                                        │
│          │                                                   │
│          ▼                                                   │
│  ┌────────────────────────────────────────────────────────┐  │
│  │        Internal "Catalog" Microservice                 │  │
│  │                                                        │  │
│  │              Vertical Slice Architecture               │  │
│  │                                                        │  │
│  │  ┌───────────────┐                                     │  │
│  │  │ Vertical Slice│                                     │  │
│  │  └───────┬───────┘                                     │  │
│  │          │                                             │  │
│  │  ┌──────────────────────────────┐                      │  │
│  │  │ UI                           │                      │  │
│  │  ├──────────────────────────────┤                      │  │
│  │  │ Application                  │                      │  │
│  │  ├──────────────────────────────┤                      │  │
│  │  │ Domain                       │                      │  │
│  │  ├──────────────────────────────┤                      │  │
│  │  │ Infrastructure               │                      │  │
│  │  └──────────────────────────────┘                      │  │
│  │                                                        │  │
│  │             CQRS Pattern                               │  │
│  └───────────────────────────────┬────────────────────────┘  │
│                                  │                           │
│                                  ▼                           │
│                     Catalog.API Container                    │
│                                  │                           │
│                                  ▼                           │
│                          PostgreSQL Database                 │
└──────────────────────────────────────────────────────────────┘
```

## Architecture Overview

### Request Flow

1. A **Client** sends a request to the service using an **external IP and port**.
2. Docker maps the request to the container's **internal IP and port**.
3. The request reaches the **Catalog.API** container.
4. The request is processed using **Vertical Slice Architecture** with **CQRS**.
5. Data is persisted in **PostgreSQL**.

## Internal Layers

| Layer | Responsibility |
|--------|----------------|
| **UI** | HTTP endpoints (Minimal APIs or Controllers), request/response handling. |
| **Application** | Business use cases, commands, queries, validation, orchestration. |
| **Domain** | Core business rules, entities, value objects, domain logic. |
| **Infrastructure** | Database access, repositories, external services, persistence. |

## Architectural Patterns

# Vertical Slice Architecture
- Organizes code by **feature** instead of technical layers.
- Each feature contains everything it needs:
  - Endpoint
  - Command/Query
  - Handler
  - Validation
  - Business logic
  - Persistence

# CQRS (Command Query Responsibility Segregation)

Separates operations into:

- **Commands**
  - Modify application state.
  - Examples:
    - Create Product
    - Update Product
    - Delete Product

- **Queries**
  - Read data only.
  - Examples:
    - Get Product
    - Get Products
    - Get Products by Category

# Database

- **PostgreSQL**
- Used as the persistence layer for the Catalog microservice.
- Stores product information accessed through CQRS handlers.


# Patterns & Principles of Catalog Microservices

- **CQRS Pattern (Command Query Responsibility Segregation):**
  Divides operations into **commands (write)** and **queries (read)**.

- **Mediator Pattern:**
  Facilitates communication between objects through a **mediator**, reducing direct dependencies and simplifying interactions.

- **Dependency Injection (DI) in ASP.NET Core:**
  A built-in feature that manages object creation and dependency resolution, promoting loose coupling and testability.

- **Minimal APIs and Routing in ASP.NET 8:**
  Provides a lightweight approach to defining HTTP endpoints with minimal boilerplate while supporting routing and request handling.

- **ORM (Object-Relational Mapping) Pattern:**
  Abstracts database interactions by mapping objects to database tables, allowing developers to work with domain objects instead of raw SQL.


# Libraries (NuGet Packages) of Catalog Microservices

- **MediatR (CQRS):**
  Simplifies the implementation of the **CQRS** pattern by dispatching commands and queries through a mediator, reducing coupling between components.

- **Carter (API Endpoints):**
  Provides a clean and lightweight way to define **Minimal API** endpoints, improving routing and HTTP request handling with concise, modular code.

- **Marten (PostgreSQL Document Database):**
  Uses **PostgreSQL** as a transactional document database. It leverages PostgreSQL's native **JSON/JSONB** capabilities to store, query, and manage documents efficiently.

- **Mapster (Object Mapping):**
  A fast and configurable object-mapping library that simplifies converting between domain entities, DTOs, request models, and response models.

- **FluentValidation for Input Validation

# 4. Project Folder Structure of Catalog Microservice

- The project is organized into the following main folders:
  - **Models**
  - **Features**
  - **Data**
  - **Abstractions**

- **Features** such as **CreateProduct** and **GetProduct** contain their own handlers, endpoint definitions, validation, and business logic.

- The primary feature module is **Products**, which groups all product-related functionality.

- The **Data** folder contains the database context, configuration, seed data, and other components responsible for data access and persistence.

## Example Folder Structure

```text
Catalog.API/
├── Properties/
├── Data/
│   └── CatalogInitialData.cs
├── Exceptions/
│   └── ProductNotFoundException.cs
├── Models/
│   └── Product.cs
├── Products/
│   ├── CreateProduct/
│   │   ├── CreateProductCommand.cs
│   │   ├── CreateProductHandler.cs
│   │   └── CreateProductEndpoint.cs
│   ├── GetProduct/
│   │   ├── GetProductQuery.cs
│   │   ├── GetProductHandler.cs
│   │   └── GetProductEndpoint.cs
│   ├── GetProducts/
│   ├── UpdateProduct/
│   └── DeleteProduct/
├── Program.cs
└── appsettings.json
```

## Organization Benefits

- **Feature-based organization** instead of layer-based organization.
- Each feature is self-contained, improving maintainability.
- Related endpoints, handlers, requests, and business logic are located together.
- The **Data** folder centralizes persistence concerns.
- The structure aligns well with **Vertical Slice Architecture** and **CQRS** principles.



# Vertical Slice Architecture

- Introduced by **Jimmy Bogard** as an alternative to traditional **Layered**, **Onion**, and **Clean Architecture** approaches.

- Organizes the code around **features** or **use cases**, rather than technical layers or concerns.

- Each **feature** is implemented **end-to-end**, spanning all layers of the application—from the API endpoint to the database.

- Commonly used in **complex, feature-rich applications**, where independent feature development improves maintainability.

- Divides the application into **distinct features**, where each feature contains everything it needs:
  - Endpoint
  - Request/Response models
  - Validation
  - Business logic
  - Data access
  - Persistence

- Contrasts with traditional **N-Tier (Layered) Architecture**, where the application is organized horizontally into layers such as:
  - Presentation
  - Business Logic
  - Data Access
  - Infrastructure

## Traditional Layered Architecture

```text
Presentation
──────────────
Business Logic
──────────────
Data Access
──────────────
Database
```

## Vertical Slice Architecture

```text
Feature A
├── Endpoint
├── Command / Query
├── Handler
├── Validation
├── Repository
└── Database

Feature B
├── Endpoint
├── Command / Query
├── Handler
├── Validation
├── Repository
└── Database

Feature C
├── Endpoint
├── Command / Query
├── Handler
├── Validation
├── Repository
└── Database
```

## Benefits

- Feature-oriented organization.
- High cohesion and low coupling.
- Easier maintenance and testing.
- Simplifies adding new features.
- Enables teams to work independently on separate features.
- Works naturally with **CQRS**, **Minimal APIs**, and **MediatR**.


# CQRS and MediatR Request Lifecycle

## Request Flow

```text
Client
   │
   ▼
HTTP Request
   │
   ▼
ASP.NET Core Endpoint (Minimal API / Controller)
   │
   ▼
IMediator.Send(Command | Query)
   │
   ▼
MediatR
   │
   ▼
IRequestHandler<TRequest, TResult>
(CommandHandler / QueryHandler)
   │
   ▼
Repository
   │
   ▼
Database
   │
   ▼
Result
   │
   ▼
Handler
   │
   ▼
MediatR
   │
   ▼
HTTP Response
   │
   ▼
Client
```

## Components

### Client
Initiates an HTTP request to the API.

### ASP.NET Core Endpoint
Receives the request and creates either a **Command** (write operation) or a **Query** (read operation).

### MediatR
Acts as the mediator, routing the request to the appropriate handler without the endpoint knowing the implementation details.

### IRequestHandler<TRequest, TResult>

Each request has exactly one corresponding handler.

- **CommandHandler**
  - Creates, updates, or deletes data.
- **QueryHandler**
  - Retrieves data without modifying application state.

### Repository

Encapsulates data-access logic and communicates with the database.

### Database

Persists or retrieves application data.

### Result

The handler returns a response object (DTO or Result) back through MediatR to the API endpoint.

---

## CQRS Flow

### Command (Write)

```text
Client
    │
    ▼
CreateProductCommand
    │
    ▼
MediatR
    │
    ▼
CreateProductHandler
    │
    ▼
Repository
    │
    ▼
Database
    │
    ▼
Success / Created Product
```

### Query (Read)

```text
Client
    │
    ▼
GetProductQuery
    │
    ▼
MediatR
    │
    ▼
GetProductHandler
    │
    ▼
Repository
    │
    ▼
Database
    │
    ▼
Product DTO
```

## Benefits

- Decouples API endpoints from business logic.
- Each request has a single, dedicated handler.
- Improves maintainability and testability.
- Supports **CQRS** by separating read and write operations.
- Integrates naturally with **Vertical Slice Architecture**, where each feature contains its own commands, queries, handlers, validation, and endpoints.



# Develop Catalog.API Infrastructure, Handler and Endpoint

- Infrastructure - Data Concerns for Catalog API
- Marten .NET Transactional Document DB for PostgreSQL Database
- Setup and Run PostgreSQL DB using Docker Compose file for Multi-container Docker environment
- Develop Products Features in Vertical Slice with CQRS, MediatR in Handler class
- Develop Products Endpoints with Minimal APIs and Carter
- Test Product Endpoints Connecting to Docker Postgres Container


# Underlying Data Structures of Catalog Microservices

- **Marten** is an **ORM (Object-Relational Mapper)** that leverages **PostgreSQL's JSON** capabilities.
- **Marten** is a powerful library that transforms **PostgreSQL** into a **.NET Transactional Document DB**.
- **PostgreSQL's JSON column features** allow us to store and query data as **JSON documents**.
- Combines the **flexibility of a document database** with the **reliability of a relational PostgreSQL database**.
- The **Catalog** service uses **Marten** for **PostgreSQL** interaction as a **Document DB**.
- 
