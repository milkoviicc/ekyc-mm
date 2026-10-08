using System.Net.Http.Json;
using eKYC.Domain.Dashboard;

namespace eKYC.Web.Blazor.Services;

public sealed class ClientAnalysisApiClient : IClientAnalysisApiClient
{
    private readonly HttpClient _httpClient;

    public ClientAnalysisApiClient(HttpClient httpClient)
    {
        ArgumentNullException.ThrowIfNull(httpClient);
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyList<DashboardClientRow>> GetByTypeAsync(string clntTypCd, DashboardFilter filter)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(clntTypCd);
        ArgumentNullException.ThrowIfNull(filter);

        var query = new List<string> { $"clntTypCd={Uri.EscapeDataString(clntTypCd)}" };
        if (!string.IsNullOrWhiteSpace(filter.ClntNm)) query.Add($"clntNm={Uri.EscapeDataString(filter.ClntNm)}");
        if (!string.IsNullOrWhiteSpace(filter.ClntPrcsngSt)) query.Add($"clntPrcsngSt={Uri.EscapeDataString(filter.ClntPrcsngSt)}");
        if (!string.IsNullOrWhiteSpace(filter.Oib)) query.Add($"oib={Uri.EscapeDataString(filter.Oib)}");
        if (filter.RskEstId.HasValue) query.Add($"rskEstId={filter.RskEstId.Value}");

        var url = "api/client-analysis?" + string.Join("&", query);
        return await _httpClient.GetFromJsonAsync<List<DashboardClientRow>>(url) ?? [];
    }
}
