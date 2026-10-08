using eKYC.Domain.Clients;

namespace eKYC.DataAccess.Clients;

public interface ICL_ClntRepository
{
    Task<CL_Clnt?> GetByIdAsync(int clntId, int tenantId);

    Task<IReadOnlyList<CL_Clnt>> GetAllAsync(int tenantId);

    Task<int> InsertAsync(CL_Clnt client);

    /// <summary>
    /// Optimistic-locking update (Pattern 1). Returns rows affected — 0 means the row_version the caller
    /// had is stale; the caller should treat that as a concurrency conflict, not silently ignore it.
    /// </summary>
    Task<int> UpdateAsync(CL_Clnt client);
}
