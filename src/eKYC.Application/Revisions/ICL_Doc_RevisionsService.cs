using eKYC.Domain.Revisions;

namespace eKYC.Application.Revisions;

public interface ICL_Doc_RevisionsService
{
    Task<IReadOnlyList<CL_Doc_Revisions>> GetAllAsync(DocRevisionFilter filter, int tenantId);

    Task<CL_Doc_Revisions?> GetByIdAsync(int docRevisionId, int tenantId);

    Task<int> CreateAsync(CL_Doc_Revisions revision);

    /// <summary>
    /// Throws <see cref="eKYC.Domain.Exceptions.ConcurrencyException{T}"/> (Pattern 1) if row_version
    /// no longer matches the database.
    /// </summary>
    Task UpdateAsync(CL_Doc_Revisions revision);

    Task DeleteAsync(int docRevisionId, int tenantId);
}
