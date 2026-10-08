using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using eKYC.Domain.Revisions;

namespace eKYC.Web.Blazor.Services;

public sealed class CL_Doc_RevisionsApiClient : ICL_Doc_RevisionsApiClient
{
    private readonly HttpClient _httpClient;

    public CL_Doc_RevisionsApiClient(HttpClient httpClient)
    {
        ArgumentNullException.ThrowIfNull(httpClient);
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyList<CL_Doc_Revisions>> GetAllAsync(DocRevisionFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);

        var query = new List<string>();
        if (filter.Year.HasValue) query.Add($"year={filter.Year.Value}");
        if (filter.RevTypeId.HasValue) query.Add($"revTypeId={filter.RevTypeId.Value}");

        var url = "api/revisions" + (query.Count > 0 ? "?" + string.Join("&", query) : string.Empty);
        return await _httpClient.GetFromJsonAsync<List<CL_Doc_Revisions>>(url) ?? [];
    }

    public async Task<CL_Doc_Revisions> CreateAsync(CL_Doc_Revisions revision)
    {
        ArgumentNullException.ThrowIfNull(revision);

        var response = await _httpClient.PostAsJsonAsync("api/revisions", revision);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<CL_Doc_Revisions>())!;
    }

    public async Task UpdateAsync(CL_Doc_Revisions revision)
    {
        ArgumentNullException.ThrowIfNull(revision);

        var response = await _httpClient.PutAsJsonAsync($"api/revisions/{revision.CL_Doc_Revision_Id}", revision);
        if (response.StatusCode == HttpStatusCode.Conflict)
        {
            var conflict = await response.Content.ReadFromJsonAsync<ConcurrencyConflictBody>();
            throw new RevisionConcurrencyConflictException(conflict?.CurrentRecord);
        }

        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteAsync(int docRevisionId)
    {
        var response = await _httpClient.DeleteAsync($"api/revisions/{docRevisionId}");
        response.EnsureSuccessStatusCode();
    }

    private sealed class ConcurrencyConflictBody
    {
        [JsonPropertyName("currentRecord")]
        public CL_Doc_Revisions? CurrentRecord { get; set; }
    }
}
