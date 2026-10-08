namespace eKYC.Domain.Reports;

/// <summary>
/// The three risk reports from the legacy Izvješća tab (TkLayoutReports01/02/03.java). Each maps to a
/// fixed base WHERE clause chosen server-side in ReportRiskRepository — never accept a raw criteria
/// string from the client, unlike the legacy Vaadin filter panel.
/// </summary>
public static class ReportKeys
{
    public const string RiskPeriod = "risk-period";
    public const string RiskDay = "risk-day";
    public const string HighRiskReprocess = "high-risk-reprocess";

    public static readonly IReadOnlyList<string> All = [RiskPeriod, RiskDay, HighRiskReprocess];
}
