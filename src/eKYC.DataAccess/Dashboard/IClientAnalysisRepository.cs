using eKYC.Domain.Dashboard;

namespace eKYC.DataAccess.Dashboard;

/// <summary>
/// Backs the "Klijenti - analiza" tab (TkLayoutAnaliza.java) — every client of a given type, regardless
/// of processing status (unlike the dashboard work queue, which hides finished/rejected clients).
/// </summary>
public interface IClientAnalysisRepository
{
    Task<IReadOnlyList<DashboardClientRow>> GetByTypeAsync(string clntTypCd, DashboardFilter filter, int tenantId);
}
