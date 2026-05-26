# MySQLCore

MySQLCore is a backend demo project showing how to build a layered ASP.NET Core REST API backed by MySQL, with RabbitMQ messaging, outbox-based background processing, Redis caching, structured logging, distributed tracing, metrics, containerized infrastructure, and test coverage across the main application layers.

## What This Demo Shows

- A REST API for CRUD-style transaction data.
- A one-to-many image transaction workflow using image records and gallery records.
- Clean separation between API, Core, Infrastructure, and Worker projects.
- Repository and service layers for data access and business logic.
- MySQL persistence through Entity Framework Core.
- Redis-backed caching through the ASP.NET Core distributed cache abstraction.
- RabbitMQ publishing and background message processing.
- Outbox message storage and publishing for more reliable asynchronous work.
- Processed message tracking for image-created events.
- API endpoints for transaction, outbox, and processed message data.
- API key middleware and Swagger security configuration.
- Global exception handling middleware.
- Structured application logging with Serilog and Seq.
- OpenTelemetry tracing exported through the OTLP collector.
- Prometheus metrics for the API and worker process.
- Grafana, Tempo, Prometheus, Seq, RabbitMQ, Redis, and MySQL infrastructure support.
- Docker Compose setup for the full local stack.
- Kubernetes manifests for backend, worker, database, middleware, and observability services.
- SQL scripts for database and table creation/removal.
- Unit test projects for API, Core, and Infrastructure behavior.

## Technology

| Area | Technology |
|---|---|
| Runtime | .NET 8.0 |
| Language | C# |
| API | ASP.NET Core Web API |
| Worker | .NET Worker Service |
| Database | MySQL 8 |
| Data access | Entity Framework Core, Pomelo MySQL provider, MySqlConnector |
| Cache | Redis, StackExchangeRedis distributed cache |
| Messaging | RabbitMQ |
| Background processing | Hosted services, dedicated worker process |
| Message reliability | Outbox pattern, processed message tracking |
| Validation | FluentValidation |
| API documentation | Swagger / OpenAPI |
| Security | API key middleware |
| Logging | Serilog, Seq, rolling file logs |
| Tracing | OpenTelemetry, OTLP collector, Tempo |
| Metrics | prometheus-net, Prometheus, Grafana |
| Containers | Docker, Docker Compose |
| Orchestration | Kubernetes |
| Testing | xUnit, Moq, AutoFixture, EF Core InMemory, coverlet |
| CI | GitHub Actions |

## Solution Structure

```text
src/
  MySQLCore.API             ASP.NET Core API, controllers, middleware, security, configuration
  MySQLCore.Core            DTOs, interfaces, services, validators, messages, constants, shared models
  MySQLCore.Infrastructure  EF Core DbContext, entities, repositories, factories, RabbitMQ and Redis services

worker/
  MySQLCore.Worker          Background workers for outbox publishing and image message processing

test/
  MySQLCore.API.Test
  MySQLCore.Core.Test
  MySQLCore.Infrastructure.Test

data/                       SQL database and table scripts
configurations/             Prometheus, Tempo, and OpenTelemetry collector configuration
kubernetes/                 Kubernetes manifests for app, middleware, database, and observability services
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
- Create image transaction data for asynchronous processing.
- Work with the messaging flow through the core service layer.

### OutboxMessagerController

Shows the current state of messages waiting to be published or recently published:

- Get pending outbox messages.
- Get the latest published message.
- Get a single outbox message by ID.

### ProcessedMessageController

Shows the result of messages handled by the worker process:

- Get a processed message by ID.
- Get the latest processed messages.

## Messaging Flow

The project uses RabbitMQ for image-created events and an outbox table to keep the publish workflow more reliable.

When the API creates work that needs to happen asynchronously, the message is stored as an outbox record. The worker process runs an `OutboxPublisherWorker` that reads pending outbox messages and publishes them to RabbitMQ. The `ImageProcessingWorker` then consumes image-created messages, processes them through the service layer, tracks the result as a processed message, and handles retry/dead-letter style failure paths through the messaging layer.

## Observability

The project includes a small observability stack so the backend can be inspected beyond basic console output:

- Serilog writes structured logs to the console, files, and Seq.
- The API exposes Prometheus metrics through `/metrics`.
- The worker exposes Prometheus metrics on its own metric server.
- OpenTelemetry tracing is exported to the OTLP collector.
- Tempo stores traces and Grafana can be used to inspect metrics and traces.

## Notes

- Environment-specific values are kept outside the main project files.
- SQL setup and teardown scripts are kept under `data/`.
- Docker Compose and Kubernetes files are included, but setup steps are intentionally not documented here.
- The API is configured for Swagger/OpenAPI and API key authentication.
- Docker image build automation is included through GitHub Actions.
