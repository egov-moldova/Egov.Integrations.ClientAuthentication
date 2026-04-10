using Egov.Integrations.ClientAuthentication;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSystemCertificate(builder.Configuration.GetSection("Certificate"));
builder.Services.AddIngressClientAuthenticationCore(builder.Configuration.GetSection("ClientAuthentication"), options =>
{
#if DEBUG
        options.Events = new ClientAuthenticationEvents
        {
            OnClientMissing = context =>
            {
                context.Success(new AuthenticatedClient
                {
                    ID = Guid.NewGuid(),
                    Name = "Debug",
                    Settings = @"{ ""Key"": ""Value"" }"
                });
                return Task.CompletedTask;
            }
        };
#endif
});
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Debug", policy => policy.RequireSettingsAssertion<DebugClientSettings>(settings => settings.Key == "Value"));
});

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", (HttpContext context) =>
{
    var client = context.GetAuthenticatedClient();
    if (client == null) return Results.Unauthorized();

    return Results.Ok(new
    {
        Client = client.Name,
        Settings = client.DeserializeSettings<DebugClientSettings>()
    });
}).RequireAuthorization("Debug");

var clientInformationProvider = app.Services.GetRequiredService<IClientInformationProvider>();
var allClients = await clientInformationProvider.GetAllClientsInformationAsync();
app.Logger.LogInformation("Clients count : {count}", allClients.Count);

var client = await clientInformationProvider.GetClientInformationAsync(Guid.NewGuid());
if (client != null) throw new InvalidCastException("Expected null client information for random ID.");

app.Run();

internal class DebugClientSettings
{
    public string? Key { get; set; }
}