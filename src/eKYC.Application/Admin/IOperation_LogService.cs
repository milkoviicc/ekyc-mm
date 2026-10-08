using eKYC.Domain.Admin;

namespace eKYC.Application.Admin;

public interface IOperation_LogService
{
    Task<IReadOnlyList<Operation_Log>> QueryAsync(OperationLogFilter filter);

    Task<IReadOnlyList<string>> GetObjectTypesAsync();
}
