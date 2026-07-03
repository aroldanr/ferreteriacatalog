# Repository Guidelines

## Project Structure & Module Organization
`ferreteria_catalog.sln` contains a single ASP.NET Core `net8.0` app in `ferreteria_catalog/`. Keep domain models in `Models/`, EF Core access in `Data/`, repository interfaces and implementations in `Repositories/`, and business logic in `Services/`. HTTP APIs live in `Controllers/`, while UI pages and page models live in `Pages/` with feature folders such as `Pages/Login/` and `Pages/Productos/`. Static web assets belong in `wwwroot/`; product images are served from `images/`; operational logs are written to `Logs/`. Do not commit changes from `bin/` or `obj/`.

## Build, Test, and Development Commands
Run commands from the repository root unless noted.

- `dotnet restore ferreteria_catalog.sln` restores NuGet packages.
- `dotnet build ferreteria_catalog.sln` validates the solution compiles.
- `dotnet run --project ferreteria_catalog` starts the app locally.
- `dotnet watch run --project ferreteria_catalog` enables live reload during Razor/API work.
- `dotnet tool restore` restores local tools such as `dotnet-ef`.
- `dotnet ef database update --project ferreteria_catalog` applies EF migrations when they exist.

## Coding Style & Naming Conventions
Follow the existing C# style: 4-space indentation, braces on new lines, `PascalCase` for types and public members, `camelCase` for parameters and locals, and `_camelCase` for private readonly fields. Keep controller actions and service methods asynchronous when they perform I/O. Name Razor Pages by feature and intent, for example `ProductoDetalles.cshtml` and `ProductoDetalles.cshtml.cs`. Prefer one responsibility per service or repository class.

## Testing Guidelines
No automated test project is currently checked in. Before opening a PR, run `dotnet build` and manually verify the main flows: login, product search, pagination, image upload, and admin product updates. When adding tests, create a separate test project under the solution and use the `*Tests.cs` naming pattern.

## Commit & Pull Request Guidelines
Recent commits use short Spanish summaries such as `ajustes` and `se remueve configuracion`. Keep commit subjects brief, imperative, and specific, for example `corrige paginacion de productos`. PRs should include a clear description, impacted areas, setup or data changes, and screenshots for Razor Page UI updates.

## Security & Configuration Tips
Do not commit real secrets, connection strings, or JWT keys. Prefer environment variables or user secrets for local development, and treat `appsettings*.json`, `Logs/`, and uploaded images as sensitive operational data.
