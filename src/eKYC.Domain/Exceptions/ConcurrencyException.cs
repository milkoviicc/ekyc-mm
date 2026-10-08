namespace eKYC.Domain.Exceptions;

/// <summary>
/// Non-generic base so middleware can catch a stale optimistic-locking update (Pattern 1) once,
/// regardless of entity type, instead of needing a separate catch clause per <see cref="ConcurrencyException{T}"/>.
/// </summary>
public abstract class ConcurrencyException : Exception
{
    public abstract object? CurrentRecordObject { get; }

    protected ConcurrencyException()
        : base("The record was modified by another user since it was loaded.")
    {
    }
}

/// <summary>
/// Thrown when an optimistic-locking update (Pattern 1: WHERE row_version = @RowVersion) affects zero rows,
/// meaning another user saved a change first. Carries the current database record so the caller can show
/// the user a compare/overwrite-or-discard dialog instead of a bare error.
/// </summary>
public sealed class ConcurrencyException<T> : ConcurrencyException
{
    public T? CurrentRecord { get; }

    public override object? CurrentRecordObject => CurrentRecord;

    public ConcurrencyException(T? currentRecord)
    {
        CurrentRecord = currentRecord;
    }
}
