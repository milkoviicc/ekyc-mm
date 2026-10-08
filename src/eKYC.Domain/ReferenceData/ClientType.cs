namespace eKYC.Domain.ReferenceData;

/// <summary>
/// Client type lookup (CL_Clnt_Typ) — Physical, Legal, Bank EU, Bank International.
/// </summary>
public sealed class ClientType
{
    public int ClntTypId { get; set; }
    public string ClntTypCd { get; set; } = string.Empty;
    public string? ClntTypDspn { get; set; }
}
