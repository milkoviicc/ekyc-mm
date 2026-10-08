using System.Net.Http.Json;
using eKYC.Domain.Admin;

namespace eKYC.Web.Blazor.Services;

public sealed class A_Object_LocksApiClient : IA_Object_LocksApiClient
{
    private readonly HttpClient _httpClient;

    public A_Object_LocksApiClient(HttpClient httpClient)
    {
        ArgumentNullException.ThrowIfNull(httpClient);
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyList<A_Object_Locks>> GetAllAsync() =>
        await _httpClient.GetFromJsonAsync<List<A_Object_Locks>>("api/admin/locks") ?? [];

    public async Task DeleteAsync(int objectLockId)
    {
        var response = await _httpClient.DeleteAsync($"api/admin/locks/{objectLockId}");
        response.EnsureSuccessStatusCode();
    }
}
