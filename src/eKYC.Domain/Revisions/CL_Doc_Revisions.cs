namespace eKYC.Domain.Revisions;

/// <summary>
/// A document/audit revision record (CL_Doc_Revisions) — "Revizija" tab (TkLayoutRevision.java).
/// </summary>
public sealed class CL_Doc_Revisions
{
    public int CL_Doc_Revision_Id { get; set; }

    /// <summary>
    /// Despite the column name (CL_Doc_Typ_Rev_Id), this stores a CL_Rev_Typ.CL_Rev_Type_Id value —
    /// the legacy app's own edit dialog binds its "Vrsta revizije" combo (sourced from CL_Rev_Typ) to
    /// this column. Faithfully replicated rather than "fixed", since the schema is being kept as-is.
    /// </summary>
    public int CL_Doc_Typ_Rev_Id { get; set; }

    public DateTime Rev_Date { get; set; }
    public DateTime? Rev_Range_From { get; set; }
    public DateTime? Rev_Range_To { get; set; }
    public string Rev_Done_By { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string? Recommendation { get; set; }

    public int row_version { get; set; }
    public int tenant_id { get; set; }
}
