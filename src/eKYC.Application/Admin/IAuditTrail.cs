namespace eKYC.Application.Admin;

/// <summary>Writes admin changes to Operation_Log, attributed to the current user (user 0 when no A_Usr row matches).</summary>
public interface IAuditTrail
{
    Task WriteAsync(string objectType, int objectId, string objectCode, string operationType, string remark);
}
