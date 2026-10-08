using eKYC.Domain.Dashboard;

namespace eKYC.Web.Blazor.Services;

public interface IDashboardApiClient
{
    Task<IReadOnlyList<DashboardClientRow>> GetWorkQueueAsync(DashboardFilter filter);
}
