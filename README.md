# MySQLCore

MySQLCore is a backend demo project showing how to build a layered ASP.NET Core REST API backed by MySQL, with message-based processing, structured logging, containerized local infrastructure, and test coverage across the main application layers.

## What This Demo Shows

- A REST API for CRUD-style transaction data.
- A one-to-many image transaction workflow using image records and gallery records.
- Clean separation between API, Core, and Infrastructure projects.
- Repository and service layers for data access and business logic.
- MySQL persistence through Entity Framework Core.
- RabbitMQ publishing and background message processing.
- Message processing state tracking for image-created events.
- API key middleware and Swagger security configuration.
- Centralized error logging with Elmah.
- Structured application logging with Serilog and Seq.
- Docker Compose setup for the API, MySQL, RabbitMQ, and Seq.
- Kubernetes manifests for backend and database deployment.
- SQL scripts for database and table creation/removal.
- Unit test projects for API, Core, and Infrastructure behavior.

## Technology

| Area | Technology |
|---|---|
| Runtime | .NET 8.0 |
| Language | C# |
| API | ASP.NET Core Web API |
| Database | MySQL |
| Data access | Entity Framework Core, Pomelo MySQL provider, MySqlConnector |
| Messaging | RabbitMQ |
| Background processing | ASP.NET Core hosted services |
| Validation | FluentValidation |
| API documentation | Swagger / OpenAPI |
| Logging | Serilog, Seq, Elmah |
| Containers | Docker, Docker Compose |
| Orchestration | Kubernetes |
| Testing | xUnit, Moq, AutoFixture, EF Core InMemory |
| CI | GitHub Actions |

## Solution Structure

```text
src/
  MySQLCore.API             ASP.NET Core API, controllers, middleware, configuration, hosted workers
  MySQLCore.Core            DTOs, interfaces, services, validators, messages, constants, shared models
  MySQLCore.Infrastructure  EF Core DbContext, entities, repositories, factories, RabbitMQ publisher

middleware/
  MySQLCore.Receiver        Console receiver project for RabbitMQ messaging experiments

test/
  MySQLCore.API.Test
  MySQLCore.Core.Test
  MySQLCore.Infrastructure.Test

data/                       SQL database and table scripts
env/                        Docker and Kubernetes environment configuration
kubernetes/                 Kubernetes manifests
scripts/                    Docker and Kubernetes command notes
```

## Main API Areas

### CRUDTransactionController

Demonstrates standard CRUD operations for a simple transaction table:

- Get all records.
- Get paged records.
- Get a record by ID.
- Create a record.
- Update a record.
- Delete a record.

### ImageTransactionController

Demonstrates a richer transaction workflow with related image gallery records:

- Manage image transaction records.
- Return related gallery data.
- Publish image-created messages for background processing.
- Track processing through message status models.

## Messaging Flow

The API uses RabbitMQ for image-created events. When image transaction work creates a message, the infrastructure layer publishes it to RabbitMQ. The API also registers an `ImageProcessingWorker` hosted service that consumes from the image queue, processes the message through the core service layer, acknowledges successful messages, and retries or dead-letters failed messages.

## Running With Docker Compose

Docker Compose is the preferred local demo path.

1. Create the external Docker network:

```bash
docker network create mysqlcore_network
```

2. Start the stack from the repository root:

```bash
docker compose up --build
```

The Compose stack includes:

- MySQL database
- RabbitMQ management broker
- Seq logging server
- MySQLCore API

## Running Tests

Run all tests from the repository root:

```bash
dotnet test
```

## Notes

- Environment-specific values are kept under `env/`.
- SQL setup and teardown scripts are kept under `data/`.
- The API is configured for Swagger/OpenAPI and API key authentication.
- Docker image build automation is included through GitHub Actions.
