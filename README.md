# Egov.Integrations.ClientAuthentication

[![NuGet](https://img.shields.io/nuget/v/Egov.Integrations.ClientAuthentication.svg)](https://www.nuget.org/packages/Egov.Integrations.ClientAuthentication)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

A .NET library for retrieving client information and settings based on HTTP headers received from a reverse proxy (Ingress). This library is designed to facilitate client identification and authentication in microservices architectures where a central ingress controller handles TLS termination and passes client certificate information via headers. It leverages `Egov.Extensions.Configuration` for secure certificate-based authentication (mTLS) with the client information service.

---

## Table of Contents

- [Features](#features)
- [Prerequisites](#prerequisites)
- [Installation](#installation)
- [Configuration](#configuration)
- [Usage](#usage)
  - [Dependency Injection (Recommended)](#dependency-injection-recommended)
  - [Retrieving Client Information](#retrieving-client-information)
  - [Health Checks](#health-checks)
- [Error Handling](#error-handling)
- [Testing](#testing)
- [Contributing](#contributing)
- [Code of Conduct](#code-of-conduct)
- [AI Assistance](#ai-assistance)
- [License](#license)

---

## Features

- **Ingress-based Authentication**: Automatically extracts client identification from HTTP headers.
- **Client Information Provider**: Retrieves detailed client metadata and settings from a central API.
- **In-memory Caching**: Efficiently caches client information to reduce API calls with configurable TTL.
- **Health Checks**: Built-in support for monitoring certificate expiration and provider availability.
- **Certificate-based Auth**: Seamless integration with `Egov.Extensions.Configuration` for mutual TLS (mTLS).
- **Easy Integration**: Simple extension methods for `IServiceCollection` and `IAuthenticationBuilder`.
- **Async-first API**: Fully asynchronous methods for all service operations.
- **Built for .NET 10+**: Leverages the latest .NET features and performance improvements.

---

## Prerequisites

- .NET 10.0 or later
- A valid service certificate for the client information API (PFX or PEM format)
- `Egov.Extensions.Configuration` for certificate management
- ASP.NET Core environment (for middleware support)

---

## Installation

Install the package from [NuGet](https://www.nuget.org/packages/Egov.Integrations.ClientAuthentication):

```shell
dotnet add package Egov.Integrations.ClientAuthentication
```

Or via the Package Manager Console:

```shell
Install-Package Egov.Integrations.ClientAuthentication
```

---

## Configuration

Add the following sections to your **appsettings.json**:

```json
{
  "ClientAuthentication": {
    "ApiBaseAddress": "https://api.egov.md/clients",
    "CacheTtl": "00:30:00",
    "CacheReuseOnApiFailure": true
  },
  "Certificate": {
    "Path": "Files/Certificates/your-certificate.pfx",
    "Password": "your-certificate-password"
  }
}
```

The client automatically uses the certificate configured via `Egov.Extensions.Configuration`.

---

## Usage

### Dependency Injection (Recommended)

Register the authentication services in **Program.cs**:

```csharp
using Egov.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Register the system certificate (required for mTLS)
builder.Services.AddSystemCertificate(builder.Configuration.GetSection("Certificate"));

// Add ingress-based client authentication
builder.Services.AddAuthentication()
    .AddIngressClient(builder.Configuration.GetSection("ClientAuthentication"));

// Optionally add health checks
builder.Services.AddHealthChecks()
    .AddClientInformationProviderHealthCheck();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
```

### Retrieving Client Information

In your Controllers or Minimal APIs, use the `GetAuthenticatedClient` extension method:

```csharp
app.MapGet("/secure-data", (HttpContext context) =>
{
    var client = context.GetAuthenticatedClient();
    if (client == null) return Results.Unauthorized();

    return Results.Ok(new { Message = $"Hello, {client.Name}!", ClientID = client.ID });
});
```

Inject `IClientInformationProvider` to retrieve client details programmatically:

```csharp
public class MyService(IClientInformationProvider clientProvider)
{
    public async Task ProcessAsync(Guid clientId)
    {
        var clientInfo = await clientProvider.GetClientInformationAsync(clientId);
        if (clientInfo != null)
        {
            var settings = clientInfo.DeserializeSettings<MySettings>();
            // ...
        }
    }
}
```

### Health Checks

Register the health check to monitor certificate validity and API connectivity:

```csharp
builder.Services.AddHealthChecks()
    .AddClientInformationProviderHealthCheck();
```

---

## Error Handling

The library handles communication errors and provides graceful fallback if caching is enabled:

| Scenario | Behavior |
|----------|-----------|
| API Down (Cache available) | Returns cached data if `CacheReuseOnApiFailure` is true |
| API Down (No cache) | Throws `HttpRequestException` |
| Missing Ingress Headers | Returns unauthenticated result (null client) |
| Certificate Expired | Health check reports `Unhealthy` |

---

## Testing

The solution includes a test project `Egov.Integrations.ClientAuthentication.Tests` built with [xUnit](https://xunit.net/).

### Running the tests

```shell
dotnet test src/Egov.Integrations.ClientAuthentication.sln
```

---

## Contributing

Contributions are welcome! Please read [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines on how to get started.

---

## Code of Conduct

This project adheres to the [Contributor Covenant Code of Conduct](CODE_OF_CONDUCT.md). By participating, you are expected to uphold this code.

---

## AI Assistance

This repository contains an [AGENTS.md](AGENTS.md) file with instructions and context for AI coding agents to assist in development, ensuring consistency in code style and project structure.

---

## License

This project is licensed under the [MIT License](LICENSE).
