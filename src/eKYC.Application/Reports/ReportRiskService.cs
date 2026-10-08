using eKYC.DataAccess.Reports;
using eKYC.Domain.Reports;

namespace eKYC.Application.Reports;

public sealed class ReportRiskService : IReportRiskService
{
    private readonly IReportRiskRepository _repository;

    public ReportRiskService(IReportRiskRepository repository)
    {
        ArgumentNullException.ThrowIfNull(repository);
        _repository = repository;
    }

    public Task<IReadOnlyList<ReportRiskRow>> GetAsync(string reportKey, ReportRiskFilter filter, int tenantId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(reportKey);
        ArgumentNullException.ThrowIfNull(filter);
        return _repository.GetAsync(reportKey, filter, tenantId);
    }
}
