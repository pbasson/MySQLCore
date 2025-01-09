# MySQLCore By Preetpal Basson 

-----------
## Overview

ASP.NET Core REST API connected to MySQL Database.


-----------
## Features

Key features include CRUD to MySQL. 

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
        - `$ docker compose up --build `

------------------------
# END
