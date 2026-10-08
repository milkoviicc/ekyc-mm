using eKYC.Domain.Clients;

namespace eKYC.Web.Blazor.Services;

public interface ICL_ClntApiClient
{
    Task<IReadOnlyList<CL_Clnt>> GetAllAsync();
    Task<CL_Clnt?> GetByIdAsync(int clntId);

    /// <summary>
    /// Throws <see cref="ClientConcurrencyConflictException"/> on an HTTP 409 (stale row_version) —
    /// the caller should reload and let the user decide how to proceed.
    /// </summary>
    Task UpdateAsync(CL_Clnt client);
}

/// <summary>Thrown by <see cref="ICL_ClntApiClient.UpdateAsync"/> on an HTTP 409 concurrency conflict.</summary>
public sealed class ClientConcurrencyConflictException : Exception
{
    public CL_Clnt? CurrentRecord { get; }

    public ClientConcurrencyConflictException(CL_Clnt? currentRecord)
        : base("The client was modified by another user since it was loaded.")
    {
        CurrentRecord = currentRecord;
    }
}
