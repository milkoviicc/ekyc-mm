using eKYC.DataAccess.Admin;

namespace eKYC.Application.Admin;

public sealed class AuditTrail : IAuditTrail
{
    private readonly IOperation_LogRepository _log;
    private readonly ICurrentUserService _currentUser;

    public AuditTrail(IOperation_LogRepository log, ICurrentUserService currentUser)
    {
        ArgumentNullException.ThrowIfNull(log);
        ArgumentNullException.ThrowIfNull(currentUser);
        _log = log;
        _currentUser = currentUser;
    }

    public async Task WriteAsync(string objectType, int objectId, string objectCode, string operationType, string remark)
    {
        var userId = await _currentUser.GetUserIdAsync() ?? 0;
        await _log.InsertAsync(userId, objectType, objectId, objectCode, operationType, remark);
    }
}
