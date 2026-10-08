namespace eKYC.Domain.Revisions;

/// <summary>Revision type lookup (CL_Rev_Typ) — e.g. "Interna revizija - HBOR".</summary>
public sealed class RevisionType
{
    public int RevTypeId { get; set; }
    public string RevTypeCd { get; set; } = string.Empty;
    public string RevTypeName { get; set; } = string.Empty;
}
