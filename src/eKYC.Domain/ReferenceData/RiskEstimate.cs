namespace eKYC.Domain.ReferenceData;

/// <summary>
/// A risk-points band (CL_Rsk_Est) — a score between LowerPoints and UpperPoints maps to a risk level.
/// </summary>
public sealed class RiskEstimate
{
    public int RskEstId { get; set; }
    public int? LowerPoints { get; set; }
    public int? UpperPoints { get; set; }
    public string? RiskLevel { get; set; }
    public string? AnalysisType { get; set; }
}
