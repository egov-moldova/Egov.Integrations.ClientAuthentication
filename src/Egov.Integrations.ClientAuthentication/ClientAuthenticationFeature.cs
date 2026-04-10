namespace Egov.Integrations.ClientAuthentication;

internal class ClientAuthenticationFeature
{
    public ClientAuthenticationFeature(string authenticationScheme, AuthenticatedClient client)
    {
        AuthenticationScheme = authenticationScheme;
        Client = client;
    }

    public string AuthenticationScheme { get; }

    public AuthenticatedClient Client { get; }
}
