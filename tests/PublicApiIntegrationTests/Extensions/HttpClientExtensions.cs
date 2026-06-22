using System.Net.Http.Headers;
using PublicApiIntegrationTests.Helpers;

namespace PublicApiIntegrationTests.Extensions;

public static class HttpClientExtensions
{
    public static HttpClient CreateAuthenticatedClient(this CustomWebApplicationFactory factory)
    {
        var client = factory.CreateClient();

        var token = ApiTokenHelper.GetToken(factory);

        client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", token);

        return client;
    }
}
