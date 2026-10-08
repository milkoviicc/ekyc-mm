using eKYC.Domain.Clients;

namespace eKYC.Application.Clients;

public interface ICL_ClntService
{
    Task<CL_Clnt?> GetByIdAsync(int clntId, int tenantId);

    Task<IReadOnlyList<CL_Clnt>> GetAllAsync(int tenantId);

    Task<int> CreateAsync(CL_Clnt client);

    /// <summary>
    /// Optimistic-locking update. Throws <see cref="eKYC.Domain.Exceptions.ConcurrencyException{T}"/>
    /// (Pattern 1) if the supplied row_version no longer matches the database.
    /// </summary>
    Task UpdateAsync(CL_Clnt client);
}
