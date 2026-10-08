namespace eKYC.Domain.Reports;

/// <summary>
/// One row of a risk report (TkLayoutReports01/02/03.java, all three backed by the same
/// CL_ReportRisk01_View — see ReportRiskRepository). PEP_Ind here is computed across contacts and
/// owners, not just the client's own flag, unlike DashboardClientRow.PepInd.
/// </summary>
public sealed class ReportRiskRow
{
    public int ClntId { get; set; }
    public int HborId { get; set; }
    public string? Oib { get; set; }
    public string ClntPrcsngSt { get; set; } = string.Empty;
    public int? RskEstId { get; set; }
    public string? RiskLevel { get; set; }
    public int? RskPnts { get; set; }
    public string PepInd { get; set; } = "NE";
    public string WtchLstInd { get; set; } = "N";
    public string ClntTypCd { get; set; } = string.Empty;
    public string VrstaKlijenta { get; set; } = string.Empty;
    public string ClntNm { get; set; } = string.Empty;
    public DateTime AddDt { get; set; }
    public DateTime MdfDt { get; set; }
}
