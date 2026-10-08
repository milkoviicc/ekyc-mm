namespace eKYC.Domain.ReferenceData;

/// <summary>
/// A country/state lookup value (CL_States) used on client and owner address fields.
/// </summary>
public sealed class ClState
{
    public int ClStateId { get; set; }
    public string StateCd { get; set; } = string.Empty;
    public string StateNm { get; set; } = string.Empty;
    public string? PhoneNum { get; set; }
}
