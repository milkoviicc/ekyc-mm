namespace eKYC.Domain.ReferenceData;

/// <summary>
/// Client document type lookup (CL_Doc_Typ) — e.g. passport, registration extract.
/// </summary>
public sealed class DocumentType
{
    public int ClDocTypId { get; set; }
    public string DocTypeCd { get; set; } = string.Empty;
    public string DocTypeNm { get; set; } = string.Empty;
}
