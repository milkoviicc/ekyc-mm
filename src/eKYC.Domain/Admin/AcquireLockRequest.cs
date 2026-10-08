namespace eKYC.Domain.Admin;

/// <summary>Asks to start editing one record: (Object_Class, Object_Id) identifies it, Object_Name is a human label.</summary>
public sealed class AcquireLockRequest
{
    public int Object_Id { get; set; }
    public string Object_Class { get; set; } = string.Empty;
    public string? Object_Name { get; set; }
}

/// <summary>Result of an acquire attempt. When not acquired, <see cref="Lock"/> is the lock that is in the way.</summary>
public sealed class AcquireLockResult
{
    public bool Acquired { get; set; }
    public A_Object_Locks? Lock { get; set; }
}

/// <summary>Timings the client needs: when an unrefreshed lock counts as abandoned and how often to refresh.</summary>
public sealed class ObjectLockSettings
{
    public int TtlMinutes { get; set; } = 30;
    public int HeartbeatSeconds { get; set; } = 120;
}
