namespace eKYC.Domain.Admin;

/// <summary>
/// Application user (A_Usr). Property names match the column names exactly.
/// The legacy Pwd column (varchar(20), plaintext-era) is deliberately NOT mapped —
/// see CLAUDE.md task workflow #7; credentials need a wider column before auth/user-management work starts.
/// </summary>
public sealed class A_Usr
{
    public int Usr_Id { get; set; }
    public string? Lgn_Nm { get; set; }
    public string? Usr_Nm_Fst { get; set; }
    public string? Usr_Nm_Lst { get; set; }
    public bool? Pwd_Rqd_Ind { get; set; }
    public string? Pwd_St { get; set; }
    public DateTime? Pwd_Dt { get; set; }
    public int? Pwd_Life { get; set; }
    public int? Org_Id { get; set; }
    /// <summary>Status flag (char(1)) — soft-delete convention, never hard-delete.</summary>
    public string? Usr_St { get; set; }
    public int? Prsn_Id { get; set; }
    /// <summary>Legacy single-role column; multiple roles live in <see cref="A_Usr_ISRole"/>.</summary>
    public int? ISRol_Id { get; set; }
    public string? Email { get; set; }
    public DateTime? Lst_Lgn_Dt { get; set; }
    public int? Lgn_Try_Cnt { get; set; }
    public int? Add_By { get; set; }
    public DateTime? Add_Dt { get; set; }
    public int? Mdf_By { get; set; }
    public DateTime? Mdf_Dt { get; set; }
    public bool Logged { get; set; }
    public string Current_Host { get; set; } = string.Empty;
    public string? SessionId { get; set; }

}
