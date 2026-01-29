**Stamp – Multi‑Business Loyalty Management System**  

Stamp is a **multi‑business loyalty and rewards platform** built with **.NET 9** following the **Clean Architecture** approach.  
It is designed to be modular, scalable, and secure — providing a solid foundation for building multi‑business loyalty solutions.  
---
## 🚀 **Key Features**
- **Multi‑Business architecture** – isolated data scope for each business  
- **Clean Architecture** design – Domain, Application, Infrastructure, Web  
- **Entity Framework Core 8** with SQL Server  
- **JWT authentication & role‑based authorization**  
- **User registration & login**  
- **FluentValidation** for request validation  
- **AutoMapper** for clean object mapping  
- **BCrypt** for secure password hashing  
- **Swagger/OpenAPI** documentation  
---
## 🏗 **Architecture Overview**

```text
Stamp.Domain
│   Entities, Value Objects, BaseEntity
│
Stamp.Application
│   Commands, Queries, DTOs, Handlers, Interfaces, Validators
│
Stamp.Infrastructure
│   EF Core DbContext, Repositories, Services, Persistence
│
Stamp.Web
    API Controllers, Program.cs, Dependency Injection
---
## 🛠 **Tech Stack**
- **.NET 9 / C# 13**  
- **Entity Framework Core 9**  
- **MediatR 13 – CQRS pattern**  
- **FluentValidation 12**  
- **AutoMapper 12**  
- **BCrypt.Net-Next 4** for password hashing  
- **JWT** (`System.IdentityModel.Tokens.Jwt`)  
- **Swagger / Swashbuckle.AspNetCore 9**  
---
## 📦 **Projects**
- **Stamp.Domain** – Core business entities & domain rules  
- **Stamp.Application** – Application services, CQRS handlers, DTOs, validators  
- **Stamp.Infrastructure** – EF Core, repositories, persistence, integrations  
- **Stamp.Web** – API controllers, dependency injection, startup configuration  

