namespace eKYC.Domain.Admin;

/// <summary>Information-system role (A_ISRole), e.g. UNOS, ODOBR1, ODOBR2, ADMIN. Property names match the columns.</summary>
public sealed class A_ISRole
{
    public int ISRol_Id { get; set; }
    public string? ISRol_Nm { get; set; }
    /// <summary>Status code — joins to <see cref="A_ISRole_St.ISRol_St"/>.</summary>
    public string? ISRol_St { get; set; }
    public string? Apl_Cd { get; set; }
}
