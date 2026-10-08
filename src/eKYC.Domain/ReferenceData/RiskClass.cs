namespace eKYC.Domain.ReferenceData;

/// <summary>
/// Risk classification lookup (CL_Rsk_Cls), tied to the risk questionnaire header it was derived from.
/// </summary>
public sealed class RiskClass
{
    public int RskClsId { get; set; }
    public int? RskQstnHId { get; set; }
    public string? RskClsDspn { get; set; }
}
