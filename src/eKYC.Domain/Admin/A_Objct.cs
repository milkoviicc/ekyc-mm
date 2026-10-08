namespace eKYC.Domain.Admin;

/// <summary>Securable application object — form, menu item, etc. (A_Objct).</summary>
public sealed class A_Objct
{
    public int Objct_Id { get; set; }
    public int? Objct_Typ_Id { get; set; }
    public string? Objct_Nm { get; set; }
    public string? Objct_Dspn { get; set; }
    /// <summary>Legacy Java class/view the object invokes.</summary>
    public string? Objct_Call { get; set; }
    public string? Asmbly_Cd { get; set; }
    public string? Objct_St { get; set; }
}
