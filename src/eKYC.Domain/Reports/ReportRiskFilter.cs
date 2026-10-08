namespace eKYC.Domain.Reports;

/// <summary>
/// Optional narrowing criteria shared by all three risk reports — mirrors TkPanelReportsFilter01's
/// date ranges (creation date, last-activity date), client type checkboxes, and risk level checkboxes.
/// </summary>
public sealed class ReportRiskFilter
{
    public DateTime? AddDtFrom { get; set; }
    public DateTime? AddDtTo { get; set; }
    public DateTime? MdfDtFrom { get; set; }
    public DateTime? MdfDtTo { get; set; }
    public IReadOnlyList<string>? ClntTypCds { get; set; }
    public IReadOnlyList<int>? RiskEstIds { get; set; }
}
