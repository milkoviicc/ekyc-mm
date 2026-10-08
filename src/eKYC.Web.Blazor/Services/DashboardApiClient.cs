using System.Net.Http.Json;
using eKYC.Domain.Dashboard;

namespace eKYC.Web.Blazor.Services;

public sealed class DashboardApiClient : IDashboardApiClient
{
    private readonly HttpClient _httpClient;

    public DashboardApiClient(HttpClient httpClient)
    {
        ArgumentNullException.ThrowIfNull(httpClient);
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyList<DashboardClientRow>> GetWorkQueueAsync(DashboardFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);

        var query = new List<string>();
        if (!string.IsNullOrWhiteSpace(filter.ClntTypCd)) query.Add($"clntTypCd={Uri.EscapeDataString(filter.ClntTypCd)}");
        if (!string.IsNullOrWhiteSpace(filter.ClntNm)) query.Add($"clntNm={Uri.EscapeDataString(filter.ClntNm)}");
        if (!string.IsNullOrWhiteSpace(filter.ClntPrcsngSt)) query.Add($"clntPrcsngSt={Uri.EscapeDataString(filter.ClntPrcsngSt)}");
        if (!string.IsNullOrWhiteSpace(filter.Oib)) query.Add($"oib={Uri.EscapeDataString(filter.Oib)}");
        if (filter.RskEstId.HasValue) query.Add($"rskEstId={filter.RskEstId.Value}");

        var url = "api/dashboard/work-queue" + (query.Count > 0 ? "?" + string.Join("&", query) : string.Empty);
        return await _httpClient.GetFromJsonAsync<List<DashboardClientRow>>(url) ?? [];
    }
}
