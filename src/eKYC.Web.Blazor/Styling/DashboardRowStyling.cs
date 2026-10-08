using eKYC.Domain.Dashboard;

namespace eKYC.Web.Blazor.Styling;

/// <summary>
/// Row/cell colors from the legacy dashboard's EkycUI.getDGRRowStyle() + mytheme.scss. Covers only the
/// "existing CL_Clnt row" case (ekyc_status_id is always 4 there per CL_Dashboard_View — the white/
/// light-red/light-orange cases only apply to not-yet-imported HBOR records, a separate intake-queue
/// feature not built here). All colors use !important — MudBlazor's own table-cell text-color rule
/// otherwise silently wins over an inherited/inline `color` on the row, which is why dark-red rows were
/// rendering with default (near-black) text instead of white.
/// </summary>
public static class DashboardRowStyling
{
    public static string GetRowStyle(DashboardClientRow row)
    {
        if (row.ClntPrcsngSt == "ODBIJEN")
        {
            return "background-color: gold !important; color: black !important;";
        }

        // Checked before the status-based branches, same as legacy — a stale zero score can override
        // even an otherwise-green AKTIVAN client.
        if (row.MustModify)
        {
            return "background-color: darkred !important; color: white !important;";
        }

        if (row.ClntPrcsngSt == "IZMJENA")
        {
            return (row.RskPnts ?? 0) < 7
                ? "background-color: lemonchiffon !important; color: black !important;"
                : "background-color: darkred !important; color: white !important;";
        }

        if (row.ClntPrcsngSt == "AKTIVAN")
        {
            return "background-color: palegreen !important; color: black !important;";
        }

        return "background-color: lemonchiffon !important; color: black !important;";
    }

    /// <summary>
    /// The HBOR_ID cell's own background wins over the row color when a pending master-data change
    /// applies — same visual priority as the legacy per-cell style. Otherwise it just matches the row.
    /// </summary>
    public static string GetHborIdCellStyle(DashboardClientRow row) =>
        row.MpChange && !row.DoNotCheck
            ? "background-color: #D7C4FF !important; color: black !important;"
            : GetRowStyle(row);
}
