# Agents

This repository is optimized for both human and AI agents.

## Project Structure

- `src/`: Contains the source code for the library and tests.
  - `Egov.Integrations.ClientAuthentication/`: The main library project.
  - `Egov.Integrations.ClientAuthentication.Tests/`: Unit tests for the library.
- `files/`: Contains static assets like the project icon (`MPass.png`).

## Metadata

- **Package ID**: `Egov.Integrations.ClientAuthentication`
- **Target Framework**: `net10.0`
- **Main Namespace**: `Egov.Integrations.ClientAuthentication`

## Development Guidelines

- Use `dotnet build` from the `src/` directory to build the solution.
- Use `dotnet test` from the `src/` directory to run all tests.
- When adding new public APIs, ensure they are documented with XML comments.
- Follow the existing coding style (Implicit Usings, Nullable enabled).

## CI/CD

- GitHub Actions are used for building, testing, and publishing.
- Workflows are located in `.github/workflows/`.
