using eKYC.Domain.Reports;

namespace eKYC.Web.Blazor.Services;

public interface IReportsApiClient
{
    Task<IReadOnlyList<ReportRiskRow>> GetRiskReportAsync(string reportKey, ReportRiskFilter filter);
}
