# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

**Build the solution:**
```bash
dotnet build DmsSolution.sln
```

**Run the API locally:**
```bash
dotnet run --project API/API.csproj
```

**Run with Docker (SQL Server + API):**
```bash
docker-compose up --build
```

**Add an EF Core migration:**
```bash
dotnet ef migrations add <MigrationName> --project DataAccess --startup-project API
```

**Apply migrations:**
```bash
dotnet ef database update --project DataAccess --startup-project API
```

There are no automated tests in this project.

## Architecture

Five projects in a clean-architecture layered structure:

| Project | Role |
|---|---|
| `Domain` | Entities, enums — no dependencies |
| `DataAccess` | EF Core `DatabaseContext`, entity configurations, migrations |
| `Application` | Interfaces/abstractions for use cases, validation, permissions, seeding |
| `Implementation` | Concrete EF implementations of use cases, permission handling, JWT, listings, seeding |
| `API` | Controllers, middleware, AutoMapper profiles, DI wiring (`Program.cs`) |

### Use Case / CQRS Pattern

All business operations go through `ICommandHandler` or `IQueryHandler`. Controllers inject these and resolve the specific use case from DI via `[FromServices]`.

**Adding a new command — follow this checklist:**

1. **Request**: `Application/UseCases/Commands/Requests/<Domain>/<Name>Request.cs`
2. **Interface**: `Application/UseCases/Commands/I<Name>Command.cs`
   - Extends `ICommand<TRequest>` (+ `IProtectedUseCase` if auth-protected)
3. **Implementation**: `Implementation/UseCases/EntityFramework/Commands/<Domain>/EF<Name>Command.cs`
   - Extends `EFUseCase` (gives access to `_context`)
   - If `IProtectedUseCase`: set `RequiredPermission` (from `PermissionCodes`) and `Scope`
4. **Register** in `Program.cs`: `builder.Services.AddTransient<I<Name>Command, EF<Name>Command>()`
5. **Validator** (optional): FluentValidation class in `Application/Validation/<Domain>/`

Queries follow the same structure under `Application/UseCases/Queries/` and `Implementation/UseCases/EntityFramework/Queries/`. Each query implements `IQuery<TSearch, TResponse>`.

### Permission System

- `PermissionCodes` — string constants (e.g., `"documents.write"`)
- `PermissionRegistry` — maps codes to `PermissionScope` and declares dependency relationships (e.g., `documents.write` requires `documents.read`)
- `PermissionScope` enum: `System`, `Document`, `User`, `Group`, `DocumentType`, `Global`
- `system.admin` bypasses all permission checks

**How authorization flows:**
1. `CommandHandler` / `QueryHandler` checks if the use case implements `IProtectedUseCase`
2. For `Document` scope: uses `IDocumentTypeResolver` to extract `DocumentTypeId` from the request, then calls `IPermissionHandler.EnsureAsync(code, documentTypeId)`
3. For all other scopes: calls `IPermissionHandler.EnsureAsync(code, null)`
4. `PermissionProvider` aggregates permissions from direct `UserPermissionGrant`s and via `GroupPermission` (through `UserGroup`)

**Validation** runs after permission check, via FluentValidation. Validators are discovered from the `Application` assembly via `AddValidatorsFromAssemblyContaining<ApplicationMarker>()`.

### Entity Base Classes

- `GuidEntity` — base for entities with `Guid` PK
- `IAuditable` — `CreatedAt` / `ModifiedAt` (auto-set in `DatabaseContext.SaveChanges`)
- `ISoftDeletable` — `IsDeleted` / `DeletedAt` (EF intercepts `Deleted` state and converts to soft delete)
- `IActivatable` — `IsActive`

### Authentication

JWT stored in an HttpOnly cookie (`dms-at`). The token is extracted in `JwtBearerEvents.OnMessageReceived`. `IApplicationActor` represents the current user; `UnauthorizedActor` is used when no valid token is present.

### Startup Seeding

`DatabaseSeeder` runs on every startup and:
- Upserts all `Permission` rows from `PermissionRegistry` (deactivates removed ones)
- Upserts `PermissionDependency` rows
- Creates admin user (`admin@admin.com` / `@dmin123`) with `system.admin` grant if not present

### Exception Handling

`ExceptionHandlingMiddleware` maps domain exceptions to HTTP status codes:
- `UnauthorizedException` / `UnauthenticatedException` → 401
- `ForbiddenException` → 403
- `RequestDataValidationException` → 400 (with field errors)
- `EntityAlreadyExistsException` → 400
- `EntityNotFoundException` → 404
- `ConflictException` → 409
- `UnsupportedExportFormatException` → 400

### Configuration

- **Database**: SQL Server via `ConnectionStrings:DefaultLocalConnection` (local) or `ConnectionStrings:DefaultConnection` (Docker)
- **JWT**: `Jwt:Issuer`, `Jwt:Audience`, `Jwt:Key`, `Jwt:AccessTokenMinutes`
- **Cookie**: `AuthCookie:Name`, `AuthCookie:SameSite`, `AuthCookie:Secure`
- **MinIO**: `MinIo:ServiceUrl`, `MinIo:AccessKey`, `MinIo:SecretKey`, `MinIo:Bucket`
- **CORS**: hardcoded for `http://localhost:4200`
