namespace eKYC.Domain.ReferenceData;

/// <summary>
/// Beneficial-ownership relationship type lookup (CL_Ownrshp_Typ).
/// </summary>
public sealed class OwnershipType
{
    public int OwnrshpTypId { get; set; }
    public string OwnrshpTypCd { get; set; } = string.Empty;
    public string? OwnrshpTypDspn { get; set; }
}
