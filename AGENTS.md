# Repository Guidelines

## Project Structure & Module Organization
- `Dotnet10AISamples.Api/Program.cs`: API startup, routing, OpenAPI, and exception handler wiring.
- `Dotnet10AISamples.Api/Controllers/`: API endpoints (e.g., `WeatherForecastController.cs`).
- `Dotnet10AISamples.Api/Middlewares/`: `ValidationExceptionHandler` and `GlobalExceptionHandler` for ProblemDetails responses.
- `Dotnet10AISamples.Api/Common/`: shared helpers like `OperationResult<T>`.
- `Dotnet10AISamples.Api/Extensions/`: service wiring helpers.
- `appsettings*.json`: local configuration; avoid committing secrets.

## Build, Test, and Development Commands
- Restore dependencies: `dotnet restore`
- Build API: `dotnet build Dotnet10AISamples.Api/Dotnet10AISamples.Api.csproj`
- Run locally with hot reload: `dotnet watch --project Dotnet10AISamples.Api`
- Run in normal mode: `dotnet run --project Dotnet10AISamples.Api`
- Container build (if needed): `docker build -t dotnet10aisamples -f Dotnet10AISamples.Api/Dockerfile .`

## Coding Style & Naming Conventions
- C# 10+ with implicit usings enabled; nullable currently disabled in the project file.
- Prefer expression-bodied members and concise object initialization when readable.
- Controllers inherit from `ControllerBase` and return `OperationResult<T>` for consistent API envelopes.
- Error handling should integrate with the registered `ProblemDetails` pipeline; use `ValidationException` for validation failures.
- PascalCase for classes/methods, camelCase for locals/parameters, ALL_CAPS for constants.

## Testing Guidelines
- No test project yet; add tests under a new `Dotnet10AISamples.Tests` (or similar) using `xUnit` or `NUnit`.
- Name test classes after the subject (`WeatherForecastControllerTests`) and methods with intent (`Get_ReturnsFiveForecasts`).
- Run tests (once added): `dotnet test`

## Commit & Pull Request Guidelines
- Commit messages: short imperative summary (e.g., `Add validation handler for forecasts`).
- Pull requests should describe scope, risks, and validation done; link to issues if they exist.
- Include screenshots or `curl` samples for API changes when useful.
- Ensure builds pass and new endpoints respect the error-handling conventions before requesting review.
