using eKYC.DataAccess.Dashboard;
using eKYC.Domain.Dashboard;

namespace eKYC.Application.Dashboard;

public sealed class DashboardService : IDashboardService
{
    private readonly IDashboardRepository _repository;

    public DashboardService(IDashboardRepository repository)
    {
        ArgumentNullException.ThrowIfNull(repository);
        _repository = repository;
    }

    public Task<IReadOnlyList<DashboardClientRow>> GetWorkQueueAsync(DashboardFilter filter, int tenantId)
    {
        ArgumentNullException.ThrowIfNull(filter);
        return _repository.GetWorkQueueAsync(filter, tenantId);
    }
}
