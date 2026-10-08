namespace eKYC.Domain.Dashboard;

/// <summary>
/// Optional narrowing criteria for the dashboard work queue — mirrors the filter sub-form in the legacy
/// TkLayoutDashboard (client type, name, processing status, OIB, risk estimate). All fields optional;
/// an unset field is not applied as a condition.
/// </summary>
public sealed class DashboardFilter
{
    public string? ClntTypCd { get; set; }
    public string? ClntNm { get; set; }
    public string? ClntPrcsngSt { get; set; }
    public string? Oib { get; set; }
    public int? RskEstId { get; set; }
}
