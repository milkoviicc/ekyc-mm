namespace eKYC.Domain.Dashboard;

/// <summary>
/// One row of the dashboard work-queue grid — a client still awaiting processing, with just enough
/// detail (from CL_Clnt joined to its type-specific sub-table) to triage without opening the full record.
/// Mirrors the legacy CL_Dashboard_View's "existing client" branch; the "brand-new HBOR client, not yet
/// imported into eKYC" branch is intentionally out of scope until IClientMasterDataProvider exists
/// (see CLAUDE.md decision #4/#7) — see DashboardRepository for detail.
/// </summary>
public sealed class DashboardClientRow
{
    public int ClntId { get; set; }
    public int HborId { get; set; }
    public string ClntTypCd { get; set; } = string.Empty;
    public string VrstaKlijenta { get; set; } = string.Empty;
    public string ClntNm { get; set; } = string.Empty;
    public string ClntSt { get; set; } = string.Empty;
    public string ClntPrcsngSt { get; set; } = string.Empty;
    public string? Status { get; set; }
    public int? RskEstId { get; set; }
    public int? RskPnts { get; set; }
    public string? PepInd { get; set; }
    public string WtchLstInd { get; set; } = "N";
    public string? Oib { get; set; }
    public string? ModifiedByName { get; set; }
    public DateTime MdfDt { get; set; }

    /// <summary>
    /// True when CL_MatPod_Compare has an unaccepted row for this client's HBOR_ID — the client's local
    /// data differs from what HBOR currently holds. Drives the HBOR_ID cell highlight (legacy "pd_change").
    /// </summary>
    public bool MpChange { get; set; }

    /// <summary>When true, suppresses the MpChange highlight even if a pending compare row exists.</summary>
    public bool DoNotCheck { get; set; }

    /// <summary>
    /// True when the client has more than one risk-questionnaire history entry, the current stored
    /// RskPnts is 0, but the most recently *completed* (Status='I') questionnaire scored more than 6 —
    /// a stale zero score masking real risk history. Overrides the normal status-based row color
    /// (legacy "mustmodify") even for an otherwise-green AKTIVAN client.
    /// </summary>
    public bool MustModify { get; set; }
}
