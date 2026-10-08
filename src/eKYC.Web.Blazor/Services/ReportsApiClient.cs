using System.Net.Http.Json;
using eKYC.Domain.Reports;

namespace eKYC.Web.Blazor.Services;

public sealed class ReportsApiClient : IReportsApiClient
{
    private readonly HttpClient _httpClient;

    public ReportsApiClient(HttpClient httpClient)
    {
        ArgumentNullException.ThrowIfNull(httpClient);
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyList<ReportRiskRow>> GetRiskReportAsync(string reportKey, ReportRiskFilter filter)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(reportKey);
        ArgumentNullException.ThrowIfNull(filter);

        var query = new List<string> { $"reportKey={Uri.EscapeDataString(reportKey)}" };
        if (filter.AddDtFrom.HasValue) query.Add($"addDtFrom={filter.AddDtFrom.Value:yyyy-MM-dd}");
        if (filter.AddDtTo.HasValue) query.Add($"addDtTo={filter.AddDtTo.Value:yyyy-MM-dd}");
        if (filter.MdfDtFrom.HasValue) query.Add($"mdfDtFrom={filter.MdfDtFrom.Value:yyyy-MM-dd}");
        if (filter.MdfDtTo.HasValue) query.Add($"mdfDtTo={filter.MdfDtTo.Value:yyyy-MM-dd}");
        if (filter.ClntTypCds is { Count: > 0 })
        {
            query.AddRange(filter.ClntTypCds.Select(c => $"clntTypCds={Uri.EscapeDataString(c)}"));
        }
        if (filter.RiskEstIds is { Count: > 0 })
        {
            query.AddRange(filter.RiskEstIds.Select(r => $"riskEstIds={r}"));
        }

        var url = "api/reports/risk?" + string.Join("&", query);
        return await _httpClient.GetFromJsonAsync<List<ReportRiskRow>>(url) ?? [];
    }
}
