using eKYC.Domain.Dashboard;

namespace eKYC.Application.Dashboard;

public interface IClientAnalysisService
{
    Task<IReadOnlyList<DashboardClientRow>> GetByTypeAsync(string clntTypCd, DashboardFilter filter, int tenantId);
}
