# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build & Run Commands

```bash
# Build
dotnet build

# Run with hot reload (recommended for development)
dotnet watch --project Dotnet10AISamples.Api

# Run normally
dotnet run --project Dotnet10AISamples.Api

# Docker build
docker build -t dotnet10aisamples -f Dotnet10AISamples.Api/Dockerfile .

# Tests (when test project exists)
dotnet test
```

**Local URLs:** http://localhost:5155 (HTTP) or https://localhost:7082 (HTTPS)

## Architecture

This is a .NET 10 Minimal API project demonstrating modern ASP.NET Core patterns.

### Key Patterns

**Response Envelope:** All API responses use `OperationResult<T>` (in `Common/`) which provides:

- `IsSuccess`, `Data`, `ErrorMessage`, `Code` properties
- Factory methods: `OperationResult<T>.Success(data)` and `OperationResult<T>.Failure(message, code)`

**Exception Handling Pipeline:** Two-tier exception handling registered via `builder.AddErrorHandling()`:

1. `ValidationExceptionHandler` - catches FluentValidation exceptions, returns 400 with structured errors
2. `GlobalExceptionHandler` - catches all unhandled exceptions, returns 500 with ProblemDetails

**Error Response Pattern:** Controllers check `result.IsSuccess` and return `Problem(detail: result.ErrorMessage, statusCode: result.Code)` for failures.

### Project Structure

- `Program.cs` - Application startup, middleware pipeline, OpenAPI configuration
- `Controllers/` - API endpoints inheriting from `ControllerBase`
- `Middlewares/` - Exception handlers implementing `IExceptionHandler`
- `Common/` - Shared types like `OperationResult<T>`
- `Extensions/` - Service registration helpers (e.g., `AddErrorHandling()`)

## Coding Conventions

- Controllers return `OperationResult<T>` for consistent API envelopes
- Use `ValidationException` from FluentValidation for validation failures
- PascalCase for classes/methods, camelCase for locals/parameters
- Nullable reference types disabled project-wide
- C# 10+ with implicit usings enabled
