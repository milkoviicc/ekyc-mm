using eKYC.Domain.Revisions;

namespace eKYC.Web.Blazor.Services;

public interface ICL_Doc_RevisionsApiClient
{
    Task<IReadOnlyList<CL_Doc_Revisions>> GetAllAsync(DocRevisionFilter filter);

    Task<CL_Doc_Revisions> CreateAsync(CL_Doc_Revisions revision);

    /// <summary>
    /// Throws <see cref="RevisionConcurrencyConflictException"/> if the server reports a stale
    /// row_version (HTTP 409) — the caller should reload and let the user decide how to proceed.
    /// </summary>
    Task UpdateAsync(CL_Doc_Revisions revision);

    Task DeleteAsync(int docRevisionId);
}

/// <summary>Thrown by <see cref="ICL_Doc_RevisionsApiClient.UpdateAsync"/> on an HTTP 409 concurrency conflict.</summary>
public sealed class RevisionConcurrencyConflictException : Exception
{
    public CL_Doc_Revisions? CurrentRecord { get; }

    public RevisionConcurrencyConflictException(CL_Doc_Revisions? currentRecord)
        : base("The revision was modified by another user since it was loaded.")
    {
        CurrentRecord = currentRecord;
    }
}
