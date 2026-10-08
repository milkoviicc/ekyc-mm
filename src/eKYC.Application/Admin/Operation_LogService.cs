using eKYC.DataAccess.Admin;
using eKYC.Domain.Admin;

namespace eKYC.Application.Admin;

public sealed class Operation_LogService : IOperation_LogService
{
    private readonly IOperation_LogRepository _repository;

    public Operation_LogService(IOperation_LogRepository repository)
    {
        ArgumentNullException.ThrowIfNull(repository);
        _repository = repository;
    }

    public Task<IReadOnlyList<Operation_Log>> QueryAsync(OperationLogFilter filter) => _repository.QueryAsync(filter);

    public Task<IReadOnlyList<string>> GetObjectTypesAsync() => _repository.GetObjectTypesAsync();
}
