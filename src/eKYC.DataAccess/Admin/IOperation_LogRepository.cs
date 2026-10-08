using eKYC.Domain.Admin;

namespace eKYC.DataAccess.Admin;

public interface IOperation_LogRepository
{
    /// <summary>Newest first, joined to A_Usr for the actor's name, capped at 5000 rows.</summary>
    Task<IReadOnlyList<Operation_Log>> QueryAsync(OperationLogFilter filter);

    /// <summary>Distinct Object_Type values, for the audit filter dropdown.</summary>
    Task<IReadOnlyList<string>> GetObjectTypesAsync();

    Task InsertAsync(int userId, string objectType, int objectId, string objectCode, string operationType, string remark);
}
