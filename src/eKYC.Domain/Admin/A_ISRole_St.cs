namespace eKYC.Domain.Admin;

/// <summary>
/// Role status lookup (A_ISRole_St). The table has NO primary key in the live schema — add one
/// (migration) before building write operations against it.
/// </summary>
public sealed class A_ISRole_St
{
    public string? ISRol_St { get; set; }
    public string? Stts_Nm { get; set; }
}
