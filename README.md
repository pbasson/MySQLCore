# MySQLCore By Preetpal Basson 

-----------
## Overview

ASP.NET Core REST API connected to MySQL Database.

-----------
## Features

Key features include CRUD operations with MySQL using Entity Framework, implementation of Onion Architecture, and deployment through multiple Docker containers (API and database).

### Technology

| Languages |
|---|
| C# |

| Technology | Version |
|---|---|
| ASP.NET | 8.0 |
| MySQL | 8.3 |
| Swagger | - |

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
