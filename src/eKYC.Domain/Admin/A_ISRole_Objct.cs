namespace eKYC.Domain.Admin;

/// <summary>Role-to-object permission / menu entry (A_ISRole_Objct), including its place in the menu hierarchy.</summary>
public sealed class A_ISRole_Objct
{
    public int ISRol_Objct_Id { get; set; }
    public int? ISRol_Id { get; set; }
    public int? Objct_Id { get; set; }
    public string? Objct_Ttl { get; set; }
    public int? Prnt_Objct_id { get; set; }
    public int? Sqnc_No { get; set; }
    /// <summary>Access modifier code (char(2)).</summary>
    public string? Mdfr_Cd { get; set; }
    public int? Hrchy_Lvl { get; set; }
    public string? Hrchy_Path { get; set; }
}
