using System.Collections.Concurrent;
using System.Net;
using System.Net.Http.Json;
using System.Text.Encodings.Web;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Egov.Integrations.ClientAuthentication;

internal sealed class MPassClientInformationProvider : IClientInformationProvider
{
    private record CachedAuthenticatedClient(DateTime ExpirationTime, AuthenticatedClient Client);
    private record CachedClients(DateTime ExpirationTime, IList<AuthenticatedClient> Clients);

    internal const string HttpClientNamePrefix = "MPassClientAuthentication+";
    private const string ClientIdentifierCachePrefix = "Client+";
    private const string ClientSerialCachePrefix = "ClientSerial+";

    private readonly ILogger<MPassClientInformationProvider> _logger;
    private readonly IOptionsMonitor<ClientInformationProviderOptions> _optionsMonitor;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ConcurrentDictionary<string, CachedAuthenticatedClient> _authenticatedCache = new();
    private CachedClients _allClientsCache = new(DateTime.MinValue, Array.Empty<AuthenticatedClient>());

    public MPassClientInformationProvider(
        ILogger<MPassClientInformationProvider> logger,
        IOptionsMonitor<ClientInformationProviderOptions> optionsMonitor,
        IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _optionsMonitor = optionsMonitor;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<AuthenticatedClient?> GetClientInformationAsync(Guid clientId, CancellationToken cancellationToken = default)
        => await GetClientInformationAsync(ClientAuthenticationDefaults.AuthenticationScheme, clientId, cancellationToken);

    public async Task<AuthenticatedClient?> GetClientInformationAsync(string authenticationScheme, Guid clientId, CancellationToken cancellationToken = default)
    {
        var clientIdString = clientId.ToString();
        return await GetClientFromCacheOrMPassAsync(authenticationScheme, ClientIdentifierCachePrefix + clientIdString,
            "ID", clientIdString, cancellationToken);
    }
    
    public async Task<AuthenticatedClient?> GetClientInformationByCertificateSerialAsync(string clientCertificateSerial, CancellationToken cancellationToken = default)
        => await GetClientInformationByCertificateSerialAsync(ClientAuthenticationDefaults.AuthenticationScheme, clientCertificateSerial, cancellationToken);

    public async Task<AuthenticatedClient?> GetClientInformationByCertificateSerialAsync(string authenticationScheme, string clientCertificateSerial, CancellationToken cancellationToken = default)
        => await GetClientFromCacheOrMPassAsync(authenticationScheme, ClientSerialCachePrefix + clientCertificateSerial, 
            "CertificateSerialNumber", clientCertificateSerial, cancellationToken);

    public async Task<IList<AuthenticatedClient>> GetAllClientsInformationAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        if (_allClientsCache.ExpirationTime >= now)
        {
            return _allClientsCache.Clients;
        }

        var options = _optionsMonitor.Get(ClientAuthenticationDefaults.AuthenticationScheme);
        try
        {
            var clients = await GetAllClientsFromMPass(ClientAuthenticationDefaults.AuthenticationScheme, cancellationToken);
            if (clients == null)
            {
                return Array.Empty<AuthenticatedClient>();
            }
            _allClientsCache = new CachedClients(now.Add(options.CacheTtl), clients);
            return clients;
        }
        catch (HttpRequestException ex) when (options.CacheReuseOnApiFailure)
        {
            _logger.FailedRetrievingClientInformation(ex);
            return _allClientsCache.Clients;
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested && options.CacheReuseOnApiFailure)
        {
            _logger.FailedRetrievingClientInformation(ex);
            return _allClientsCache.Clients;
        }
    }

    private async Task<AuthenticatedClient?> GetClientFromCacheOrMPassAsync(string authenticationScheme, string cacheKey, 
        string searchType, string searchValue, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        if (_authenticatedCache.TryGetValue(cacheKey, out var cachedAuthenticatedClient) &&
            cachedAuthenticatedClient.ExpirationTime >= now)
        {
            return cachedAuthenticatedClient.Client;
        }

        var options = _optionsMonitor.Get(authenticationScheme);

        try
        {
            var client = await GetClientFromMPassAsync(authenticationScheme, searchType, searchValue, cancellationToken);
            
            if (client != null)
            {
                _authenticatedCache[cacheKey] = new CachedAuthenticatedClient(now.Add(options.CacheTtl), client);
            }

            return client;
        }
        catch (HttpRequestException ex) when (options.CacheReuseOnApiFailure)
        {
            _logger.FailedRetrievingClientInformation(ex);
            return cachedAuthenticatedClient?.Client;
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested && options.CacheReuseOnApiFailure)
        {
            _logger.FailedRetrievingClientInformation(ex);
            return cachedAuthenticatedClient?.Client;
        }
    }

    private async Task<AuthenticatedClient?> GetClientFromMPassAsync(string authenticationScheme, string searchType, string searchValue, CancellationToken cancellationToken)
    {
        var httpClient = _httpClientFactory.CreateClient(HttpClientNamePrefix + authenticationScheme);
        var uri = new Uri($"?searchType={searchType}&searchValue={UrlEncoder.Default.Encode(searchValue)}", UriKind.Relative);
        using var response = await httpClient.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<AuthenticatedClient>(cancellationToken: cancellationToken);
    }

    private async Task<IList<AuthenticatedClient>?> GetAllClientsFromMPass(string authenticationScheme, CancellationToken cancellationToken)
    {
        var httpClient = _httpClientFactory.CreateClient(HttpClientNamePrefix + authenticationScheme);
        using var response = await httpClient.GetAsync(string.Empty, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<IList<AuthenticatedClient>>(cancellationToken: cancellationToken);
    }
}
