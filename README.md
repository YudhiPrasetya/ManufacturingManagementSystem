# Manufacturing Management System

[![.NET CI](https://github.com/YudhiPrasetya/ManufacturingManagementSystem/actions/workflows/ci.yml/badge.svg)](https://github.com/YudhiPrasetya/ManufacturingManagementSystem/actions/workflows/ci.yml)

An enterprise-oriented Manufacturing Management System built with **.NET 10**, **ASP.NET Core Web API**, **Entity Framework Core**, and **Clean Architecture**.

This project demonstrates backend engineering practices for manufacturing operations, focusing on production order management, production reporting, data persistence, automated testing, and continuous integration.

## Project Overview

The system provides RESTful APIs to manage manufacturing production orders and track production progress.

Current capabilities include:

- Create production orders.
- Retrieve production orders by order number.
- Retrieve production orders with pagination.
- Filter production orders by status.
- Report completed production quantities.
- Automatically manage production order status.
- Enforce production quantity business rules.
- Persist data using SQLite and Entity Framework Core.
- Automated unit and integration testing.
- GitHub Actions CI pipeline.

## Technology Stack

| Category            | Technology               |
| ------------------- | ------------------------ |
| Framework           | .NET 10                  |
| Backend             | ASP.NET Core Web API     |
| Architecture        | Clean Architecture       |
| ORM                 | Entity Framework Core 10 |
| Database            | SQLite                   |
| API Documentation   | Swagger / OpenAPI        |
| Testing             | xUnit                    |
| Mocking             | Moq                      |
| Integration Testing | EF Core SQLite In-Memory |
| Version Control     | Git                      |
| CI/CD               | GitHub Actions           |

## Architecture

The project follows Clean Architecture principles to separate business logic, application use cases, infrastructure concerns, and API presentation.

```text
ManufacturingManagementSystem
│
├── src
│   └── Production
│       ├── Production.Api
│       │   ├── Controllers
│       │   └── Contracts
│       │
│       ├── Production.Application
│       │   ├── Interfaces
│       │   └── UseCases
│       │
│       ├── Production.Domain
│       │   └── Entities
│       │
│       └── Production.Infrastructure
│           ├── Persistence
│           ├── Repositories
│           └── Migrations
│
├── tests
│   └── Production.Tests
│
├── .github
│   └── workflows
│       └── ci.yml
│
└── ManufacturingManagementSystem.slnx
```

### Architecture Responsibilities

- **Domain:** Core business entities, business rules, and domain behavior.
- **Application:** Use cases, commands, queries, and repository contracts.
- **Infrastructure:** Database context, Entity Framework Core configuration, migrations, and repository implementations.
- **API:** HTTP endpoints, request/response contracts, dependency injection, and API documentation.
- **Tests:** Automated verification of domain rules, application use cases, and repository persistence.

## Production Order Business Rules

Production orders follow these business rules:

1. Order number, product code, and product name are required.
2. Planned quantity must be greater than zero.
3. Each production order must have a unique order number.
4. Reported production quantity must be greater than zero.
5. Completed quantity cannot exceed planned quantity.
6. Production status is updated automatically.

### Production Status Lifecycle

| Status     | Description                                 |
| ---------- | ------------------------------------------- |
| Planned    | Production has not started.                 |
| InProgress | Production has started but is not complete. |
| Completed  | Completed quantity equals planned quantity. |

## API Endpoints

Base URL:

```text
http://localhost:5228
```

| Method | Endpoint                                      | Description                     |
| ------ | --------------------------------------------- | ------------------------------- |
| POST   | `/api/production/orders`                      | Create production order         |
| GET    | `/api/production/orders/{orderNumber}`        | Get production order details    |
| GET    | `/api/production/orders`                      | Get paginated production orders |
| POST   | `/api/production/orders/{orderNumber}/report` | Report production quantity      |

### Create Production Order

`POST /api/production/orders`

Request:

```json
{
  "orderNumber": "PO-2026-0001",
  "productCode": "FAB-001",
  "productName": "Cotton Fabric 30s",
  "plannedQuantity": 1000
}
```

### Report Production

`POST /api/production/orders/PO-2026-0001/report`

Request:

```json
{
  "quantity": 250
}
```

### Get Production Orders

`GET /api/production/orders?page=1&pageSize=10`

Filter by status:

```text
GET /api/production/orders?page=1&pageSize=10&status=InProgress
```

The paginated response includes items, current page, page size, total count, total pages, and navigation indicators.

## Database

The application currently uses SQLite with Entity Framework Core.

Database schema changes are managed through EF Core migrations.

The local development database is created when EF Core migrations are applied using the dotnet ef database update command.

## Getting Started

### Prerequisites

- .NET 10 SDK
- Git
- Visual Studio Code or another compatible editor
- EF Core CLI (dotnet-ef 10.0.14)

### Clone Repository

```bash
git clone https://github.com/YudhiPrasetya/ManufacturingManagementSystem.git

cd ManufacturingManagementSystem
```

### Restore Dependencies

```bash
dotnet restore ManufacturingManagementSystem.slnx
dotnet tool install --global dotnet-ef --version 10.0.14
```

### Apply Database Migrations

```bash
dotnet ef database update /
  --project src/Production/Production.Infrastructure /
  --startup-project src/Production/Production.Api
```

### Run Application

```bash
dotnet run --project src/Production/Production.Api
```

Open Swagger UI:

```text
http://localhost:5228/swagger
```

The actual URL may differ depending on the configured application launch profile.

## Testing

The project includes automated tests covering:

- Production order domain validation.
- Production quantity reporting rules.
- Application use cases.
- Pagination and status filtering.
- Repository persistence.
- Unique order number constraints.

Run all tests:

```bash
dotnet test ManufacturingManagementSystem.slnx
```

**Current test results:**

- Total tests: 25
- Passed: 25
- Failed: 0
- Skipped: 0

## Continuous Integration

GitHub Actions automatically executes the following pipeline on every push and pull request:

1. Checkout source code.
2. Set up .NET 10 SDK.
3. Restore dependencies.
4. Build the solution in Release configuration.
5. Run automated tests.

The CI pipeline has successfully completed its initial GitHub-hosted execution.

## Project Roadmap

Planned improvements:

- [ ] Improve API validation and standardized error handling.
- [ ] Add structured logging and observability.
- [ ] Add authentication and authorization.
- [ ] Introduce additional manufacturing modules.
- [ ] Add Inventory Management service.
- [ ] Introduce API Gateway with Ocelot.
- [ ] Implement asynchronous integration using RabbitMQ.
- [ ] Expand integration and architecture testing.
- [ ] Add deployment automation.

## Engineering Focus

This project is developed to demonstrate practical backend engineering capabilities, including:

- Clean Architecture and separation of concerns.
- Domain-driven business rules.
- RESTful API design.
- Database modeling and persistence.
- Automated testing.
- CI pipeline implementation.
- Scalable service-oriented architecture planning.

---

**Author:** Yudhi Prasetyo  
**GitHub:** [YudhiPrasetya](https://github.com/YudhiPrasetya)
