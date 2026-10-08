using eKYC.Domain.Dashboard;

namespace eKYC.Application.Dashboard;

public interface IDashboardService
{
    Task<IReadOnlyList<DashboardClientRow>> GetWorkQueueAsync(DashboardFilter filter, int tenantId);
}
