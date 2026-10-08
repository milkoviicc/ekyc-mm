namespace eKYC.Domain.Clients;

/// <summary>
/// A bank client under AML/KYC review (CL_Clnt) — physical person, legal entity, or correspondent bank.
/// The specific client-type detail record (physical/legal/bank) is a separate aggregate keyed by the same <see cref="Clnt_Id"/>.
/// </summary>
public sealed class CL_Clnt
{
    public int Clnt_Id { get; set; }

    /// <summary>Foreign key into the upstream HBOR client-master source, not a local identity.</summary>
    public int HBOR_ID { get; set; }

    public string Clnt_Typ_Cd { get; set; } = string.Empty;
    public string Clnt_St { get; set; } = string.Empty;

    /// <summary>
    /// Current workflow status, stored as the CL_Clnt_PrcsSt.Clnt_PrcsSt_Cd string rather than its numeric id —
    /// the legacy schema has no FK enforcing this, so validate against ClientProcessingStatus/ClientProcessingStatusTransition
    /// in the application layer rather than assuming the database will reject an invalid value.
    /// </summary>
    public string Clnt_Prcsng_St { get; set; } = string.Empty;

    public int? RskEst_Id { get; set; }
    public int? Rsk_Pnts { get; set; }
    public string? PEP_Ind { get; set; }
    public string? Rmrk { get; set; }
    public string WtchLst_Ind { get; set; } = "N";

    public int Add_By { get; set; }
    public DateTime Add_Dt { get; set; }
    public int Mdf_By { get; set; }
    public DateTime Mdf_Dt { get; set; }

    /// <summary>Optimistic-concurrency token (Pattern 1) — additive column, not present in the legacy schema.</summary>
    public int row_version { get; set; }

    /// <summary>Multi-tenant discriminator — additive column, single tenant in use today.</summary>
    public int tenant_id { get; set; }
}
