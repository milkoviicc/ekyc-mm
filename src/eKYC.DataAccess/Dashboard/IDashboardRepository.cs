using eKYC.Domain.Dashboard;

namespace eKYC.DataAccess.Dashboard;

public interface IDashboardRepository
{
    Task<IReadOnlyList<DashboardClientRow>> GetWorkQueueAsync(DashboardFilter filter, int tenantId);
}
