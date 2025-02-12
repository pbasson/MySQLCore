# MySQLCore By Preetpal Basson 

-----------
## Overview

ASP.NET Core REST API integrated with MySQL Database.

-----------
## Features

- CRUD operations with MySQL using Entity Framework, structured within an Onion Architecture for modular design.
- Containerized deployment via Docker (API & database) with Kubernetes orchestration for scalability.
- Environment variables for configuration management and a structured Git workflow.
- Automated database handling, including creation, table management, and teardown processes.
- Security measures implemented, including HTTPS, TLS, and API key authentication.

### Controllers

#### CRUDTransactionController

Handles basic CRUD operations for single-table transactions.

#### ImageTransactionController

Manages a one-to-many table structure for storing image file paths efficiently.

### Technology

| Languages |
|---|
| C# |

| Technology | Version |
|---|---|
| ASP.NET | 8.0 |
| MySQL | 8.3 |
| NUnit | - |
| Elmah  | - |
| Swagger | - |
| Docker | - |
| Kubernetes | - |
| Automapper | - |

-----------
## Usage

Prefered way to execute is Docker Compose. 

### Docker Compose

1. Set up docker network (required as docker compose cannot create this.) 
    1. `$ docker network create mysqlcore_network`
1. From Project Directory, Execute Docker Compose 
    1. Execute Docker Compose with Build 
        1. `$ docker compose up --build `

------------------------
# END
