namespace eKYC.Domain.Admin;

/// <summary>Audit trail row - table Operation_Log - joined to A_Usr for the actor's name.</summary>
public sealed class Operation_Log
{
    public int Log_Id { get; set; }
    public DateTime Log_Timestamp { get; set; }
    public int User_Id { get; set; }
    public string Object_Type { get; set; } = string.Empty;
    public int Object_Id { get; set; }
    public string Object_Code { get; set; } = string.Empty;
    public string Operation_Type { get; set; } = string.Empty;
    public string Log_Remark { get; set; } = string.Empty;

    /// <summary>Joined from A_Usr; null if the user no longer exists.</summary>
    public string? Lgn_Nm { get; set; }
    public string? Usr_Nm_Fst { get; set; }
    public string? Usr_Nm_Lst { get; set; }
}

/// <summary>Query filter for the audit trail; all fields optional.</summary>
public sealed class OperationLogFilter
{
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public int? User_Id { get; set; }
    public string? Object_Type { get; set; }

    /// <summary>Maximum rows returned, newest first (capped by the repository).</summary>
    public int Take { get; set; } = 500;
}
