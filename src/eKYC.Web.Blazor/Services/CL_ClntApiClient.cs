using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using eKYC.Domain.Clients;

namespace eKYC.Web.Blazor.Services;

public sealed class CL_ClntApiClient : ICL_ClntApiClient
{
    private readonly HttpClient _httpClient;

    public CL_ClntApiClient(HttpClient httpClient)
    {
        ArgumentNullException.ThrowIfNull(httpClient);
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyList<CL_Clnt>> GetAllAsync() =>
        await _httpClient.GetFromJsonAsync<List<CL_Clnt>>("api/clients") ?? [];

    public async Task<CL_Clnt?> GetByIdAsync(int clntId)
    {
        var response = await _httpClient.GetAsync($"api/clients/{clntId}");
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<CL_Clnt>();
    }

    public async Task UpdateAsync(CL_Clnt client)
    {
        ArgumentNullException.ThrowIfNull(client);

        var response = await _httpClient.PutAsJsonAsync($"api/clients/{client.Clnt_Id}", client);
        if (response.StatusCode == HttpStatusCode.Conflict)
        {
            var conflict = await response.Content.ReadFromJsonAsync<ConcurrencyConflictBody>();
            throw new ClientConcurrencyConflictException(conflict?.CurrentRecord);
        }

        response.EnsureSuccessStatusCode();
    }

    private sealed class ConcurrencyConflictBody
    {
        [JsonPropertyName("currentRecord")]
        public CL_Clnt? CurrentRecord { get; set; }
    }
}
