using eKYC.DataAccess.Dashboard;
using eKYC.Domain.Dashboard;

namespace eKYC.Application.Dashboard;

public sealed class ClientAnalysisService : IClientAnalysisService
{
    private readonly IClientAnalysisRepository _repository;

    public ClientAnalysisService(IClientAnalysisRepository repository)
    {
        ArgumentNullException.ThrowIfNull(repository);
        _repository = repository;
    }

    public Task<IReadOnlyList<DashboardClientRow>> GetByTypeAsync(string clntTypCd, DashboardFilter filter, int tenantId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(clntTypCd);
        ArgumentNullException.ThrowIfNull(filter);
        return _repository.GetByTypeAsync(clntTypCd, filter, tenantId);
    }
}
