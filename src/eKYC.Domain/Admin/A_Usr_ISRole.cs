namespace eKYC.Domain.Admin;

/// <summary>User-to-role assignment with validity window (A_Usr_ISRole).</summary>
public sealed class A_Usr_ISRole
{
    public int Usr_ISRol_Id { get; set; }
    public int? Usr_Id { get; set; }
    public int? ISRol_Id { get; set; }
    public DateTime? Vld_From_Dt { get; set; }
    public DateTime? Vld_To_Dt { get; set; }
}
