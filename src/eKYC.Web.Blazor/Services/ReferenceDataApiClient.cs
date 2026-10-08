using System.Net.Http.Json;
using eKYC.Domain.ReferenceData;
using eKYC.Domain.Revisions;

namespace eKYC.Web.Blazor.Services;

public sealed class ReferenceDataApiClient : IReferenceDataApiClient
{
    private readonly HttpClient _httpClient;

    public ReferenceDataApiClient(HttpClient httpClient)
    {
        ArgumentNullException.ThrowIfNull(httpClient);
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyList<ClState>> GetStatesAsync() =>
        await _httpClient.GetFromJsonAsync<List<ClState>>("api/reference-data/states") ?? [];

    public async Task<IReadOnlyList<ClientType>> GetClientTypesAsync() =>
        await _httpClient.GetFromJsonAsync<List<ClientType>>("api/reference-data/client-types") ?? [];

    public async Task<IReadOnlyList<RiskClass>> GetRiskClassesAsync() =>
        await _httpClient.GetFromJsonAsync<List<RiskClass>>("api/reference-data/risk-classes") ?? [];

    public async Task<IReadOnlyList<RiskEstimate>> GetRiskEstimatesAsync() =>
        await _httpClient.GetFromJsonAsync<List<RiskEstimate>>("api/reference-data/risk-estimates") ?? [];

    public async Task<IReadOnlyList<OwnershipType>> GetOwnershipTypesAsync() =>
        await _httpClient.GetFromJsonAsync<List<OwnershipType>>("api/reference-data/ownership-types") ?? [];

    public async Task<IReadOnlyList<DocumentType>> GetDocumentTypesAsync() =>
        await _httpClient.GetFromJsonAsync<List<DocumentType>>("api/reference-data/document-types") ?? [];

    public async Task<IReadOnlyList<ClientProcessingStatus>> GetProcessingStatusesAsync() =>
        await _httpClient.GetFromJsonAsync<List<ClientProcessingStatus>>("api/reference-data/processing-statuses") ?? [];

    public async Task<IReadOnlyList<RevisionType>> GetRevisionTypesAsync() =>
        await _httpClient.GetFromJsonAsync<List<RevisionType>>("api/reference-data/revision-types") ?? [];
}
