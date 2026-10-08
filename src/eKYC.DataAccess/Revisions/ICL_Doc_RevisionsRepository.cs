using eKYC.Domain.Revisions;

namespace eKYC.DataAccess.Revisions;

public interface ICL_Doc_RevisionsRepository
{
    Task<IReadOnlyList<CL_Doc_Revisions>> GetAllAsync(DocRevisionFilter filter, int tenantId);

    Task<CL_Doc_Revisions?> GetByIdAsync(int docRevisionId, int tenantId);

    Task<int> InsertAsync(CL_Doc_Revisions revision);

    /// <summary>Optimistic-locking update (Pattern 1). Returns rows affected.</summary>
    Task<int> UpdateAsync(CL_Doc_Revisions revision);

    Task<int> DeleteAsync(int docRevisionId, int tenantId);
}
