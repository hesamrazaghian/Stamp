# Stamp – Multi‑Tenant Loyalty Management System

Stamp is a **multi‑tenant loyalty system** built with **.NET 8**, following the **Clean Architecture** approach.  
It is designed to be modular, scalable, and secure, providing a robust base for building loyalty and reward platforms.

---

## 📚 Features
- **Multi‑Tenant architecture** with separate data scope per tenant  
- **Clean Architecture layers**: Domain, Application, Infrastructure, Web  
- **Entity Framework Core 8** with SQL Server  
- **JWT authentication & authorization**  
- **User registration and login**  
- **FluentValidation** for input validation  
- **AutoMapper** for object mapping  
- **BCrypt** for secure password hashing  
- **Swagger/OpenAPI** for API documentation  

---

## 🏗️ Architecture
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
    API Controllers, Program.cs, DI configuration


⚙️ Tech Stack
- **.NET 9 / C# 13**
- **Entity Framework Core 9**
- **MediatR 13**
- **FluentValidation 12**
- **AutoMapper 12**
- **BCrypt.Net-Next 4**
- **JWT** (System.IdentityModel.Tokens.Jwt)
- **Swagger / Swashbuckle.AspNetCore 9**

---

## 📦 Projects
- **Stamp.Domain** – Core business entities and domain logic
- **Stamp.Application** – Application services, CQRS handlers, DTOs, validators
- **Stamp.Infrastructure** – EF Core, repositories, external services, migrations
- **Stamp.Web** – API controllers, dependency injection, startup configuration

