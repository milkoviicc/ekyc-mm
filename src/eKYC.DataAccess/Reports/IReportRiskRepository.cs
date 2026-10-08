using eKYC.Domain.Reports;

namespace eKYC.DataAccess.Reports;

public interface IReportRiskRepository
{
    Task<IReadOnlyList<ReportRiskRow>> GetAsync(string reportKey, ReportRiskFilter filter, int tenantId);
}
