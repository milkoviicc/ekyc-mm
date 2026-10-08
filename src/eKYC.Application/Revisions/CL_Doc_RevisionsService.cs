using eKYC.DataAccess.Revisions;
using eKYC.Domain.Exceptions;
using eKYC.Domain.Revisions;

namespace eKYC.Application.Revisions;

public sealed class CL_Doc_RevisionsService : ICL_Doc_RevisionsService
{
    private readonly ICL_Doc_RevisionsRepository _repository;

    public CL_Doc_RevisionsService(ICL_Doc_RevisionsRepository repository)
    {
        ArgumentNullException.ThrowIfNull(repository);
        _repository = repository;
    }

    public Task<IReadOnlyList<CL_Doc_Revisions>> GetAllAsync(DocRevisionFilter filter, int tenantId)
    {
        ArgumentNullException.ThrowIfNull(filter);
        return _repository.GetAllAsync(filter, tenantId);
    }

    public Task<CL_Doc_Revisions?> GetByIdAsync(int docRevisionId, int tenantId) =>
        _repository.GetByIdAsync(docRevisionId, tenantId);

    public Task<int> CreateAsync(CL_Doc_Revisions revision)
    {
        ArgumentNullException.ThrowIfNull(revision);
        return _repository.InsertAsync(revision);
    }

    public async Task UpdateAsync(CL_Doc_Revisions revision)
    {
        ArgumentNullException.ThrowIfNull(revision);

        var rowsAffected = await _repository.UpdateAsync(revision);
        if (rowsAffected == 0)
        {
            var current = await _repository.GetByIdAsync(revision.CL_Doc_Revision_Id, revision.tenant_id);
            throw new ConcurrencyException<CL_Doc_Revisions>(current);
        }
    }

    public Task DeleteAsync(int docRevisionId, int tenantId) =>
        _repository.DeleteAsync(docRevisionId, tenantId);
}
