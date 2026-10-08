using eKYC.Domain.Dashboard;

namespace eKYC.Web.Blazor.Services;

public interface IClientAnalysisApiClient
{
    Task<IReadOnlyList<DashboardClientRow>> GetByTypeAsync(string clntTypCd, DashboardFilter filter);
}
