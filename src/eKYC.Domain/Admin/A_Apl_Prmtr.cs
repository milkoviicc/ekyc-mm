namespace eKYC.Domain.Admin;

/// <summary>Application configuration key/value (A_Apl_Prmtr).</summary>
public sealed class A_Apl_Prmtr
{
    public int Apl_Id { get; set; }
    public string? Prmtr_Cd { get; set; }
    public string? Prmtr_Val { get; set; }
    public string? Prmtr_Dspn { get; set; }
}
