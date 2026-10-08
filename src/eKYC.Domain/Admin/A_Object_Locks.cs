namespace eKYC.Domain.Admin;

/// <summary>
/// A held edit-session lock (A_Object_Locks) — "Zaključavanje" sub-tab of Administriranje
/// (TkLayoutAdminLocks.java). This is the existing pessimistic-lock table itself; the actual
/// lock-acquisition mechanism (Pattern 2, replacing this with sp_getapplock) is not yet built —
/// this page is the admin view/override tool for whatever locks exist.
/// </summary>
public sealed class A_Object_Locks
{
    public int Object_Lock_Id { get; set; }
    public int Object_Id { get; set; }
    public string Object_Class { get; set; } = string.Empty;
    public string? Object_Name { get; set; }
    public int User_Id { get; set; }
    public DateTime Locked_At { get; set; }
    public string? Computer_Name { get; set; }
    public string? Lgn_Nm { get; set; }
    public string? Usr_Nm_Fst { get; set; }
    public string? Usr_Nm_Lst { get; set; }
}
