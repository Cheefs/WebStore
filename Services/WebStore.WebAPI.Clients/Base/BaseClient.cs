using System.Net;
using System.Net.Http.Json;

namespace WebStore.WebAPI.Clients.Base;

public abstract class BaseClient
{
    protected HttpClient Http { get; }
    protected string Address { get; }
    protected BaseClient(HttpClient client, string address)
    {
        Http = client;
        Address = address;
    }

    protected T? Get<T>(string url) => GetAsync<T>(url).Result;

    protected async Task<T?> GetAsync<T>(string url, CancellationToken cancellationToken = default)
    {
        var response = await Http.GetAsync(url, cancellationToken).ConfigureAwait(false);

        switch (response.StatusCode)
        {
            case HttpStatusCode.NoContent:
            case HttpStatusCode.NotFound:
                return default;
            default:
                return await response
                    .EnsureSuccessStatusCode()
                    .Content
                    .ReadFromJsonAsync<T>();
        }
    }

    protected HttpResponseMessage Post<T>(string url, T body) => PostAsync(url, body).Result;

    protected async Task<HttpResponseMessage> PostAsync<T>(string url, T body, CancellationToken cancellationToken = default)
    {
        var response = await Http.PostAsJsonAsync(url, body, cancellationToken).ConfigureAwait(false);
        return response.EnsureSuccessStatusCode();
    }

    protected HttpResponseMessage Put<T>(string url, T body) => PutAsync(url, body).Result;

    protected async Task<HttpResponseMessage> PutAsync<T>(string url, T body, CancellationToken cancellationToken = default)
    {
        var response = await Http.PostAsJsonAsync(url, body, cancellationToken).ConfigureAwait(false);
        return response.EnsureSuccessStatusCode();
    }

    protected HttpResponseMessage Delete(string url) => DeleteAsync(url).Result;

    protected async Task<HttpResponseMessage> DeleteAsync(string url, CancellationToken cancellationToken = default)
    {
        return await Http.DeleteAsync(url, cancellationToken).ConfigureAwait(false);
    }
}
