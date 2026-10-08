using eKYC.Domain.Reports;

namespace eKYC.Application.Reports;

public interface IReportRiskService
{
    Task<IReadOnlyList<ReportRiskRow>> GetAsync(string reportKey, ReportRiskFilter filter, int tenantId);
}
