using Dapper;
using eKYC.Domain.Reports;

namespace eKYC.DataAccess.Reports;

public sealed class ReportRiskRepository : IReportRiskRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public ReportRiskRepository(IDbConnectionFactory connectionFactory)
    {
        ArgumentNullException.ThrowIfNull(connectionFactory);
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<ReportRiskRow>> GetAsync(string reportKey, ReportRiskFilter filter, int tenantId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(reportKey);
        ArgumentNullException.ThrowIfNull(filter);
        if (!ReportKeys.All.Contains(reportKey))
        {
            throw new ArgumentOutOfRangeException(nameof(reportKey), reportKey, "Unknown report key.");
        }

        var sql = new System.Text.StringBuilder("""
            SELECT
                V.Clnt_Id        AS ClntId,
                V.HBOR_ID        AS HborId,
                V.OIB            AS Oib,
                V.Clnt_Prcsng_St AS ClntPrcsngSt,
                V.RskEst_Id      AS RskEstId,
                V.Rsk_Lvl        AS RiskLevel,
                V.Rsk_Pnts       AS RskPnts,
                V.PEP_Ind        AS PepInd,
                V.WtchLst_Ind    AS WtchLstInd,
                V.Clnt_Typ_Cd    AS ClntTypCd,
                V.VrstaKlijenta  AS VrstaKlijenta,
                V.Clnt_Nm        AS ClntNm,
                V.Add_Dt         AS AddDt,
                V.Mdf_Dt         AS MdfDt
            FROM CL_ReportRisk01_View V
            INNER JOIN CL_Clnt C ON V.Clnt_Id = C.Clnt_Id
            WHERE C.tenant_id = @TenantId
            """);

        var parameters = new DynamicParameters();
        parameters.Add("TenantId", tenantId);

        // Base criteria is chosen server-side from a known key — never built from client-supplied SQL,
        // unlike the legacy Vaadin filter panel which lets the caller pass an arbitrary WHERE fragment.
        switch (reportKey)
        {
            case ReportKeys.RiskPeriod:
            case ReportKeys.RiskDay:
                sql.Append(" AND V.Clnt_Prcsng_St IN ('AKTIVAN', 'ODBIJEN')");
                break;
            case ReportKeys.HighRiskReprocess:
                sql.Append(" AND V.Clnt_Prcsng_St IN ('UPITNIK KREIRAN', 'IZMJENA') AND V.Rsk_Pnts >= 7");
                break;
        }

        if (filter.AddDtFrom.HasValue)
        {
            sql.Append(" AND V.Add_Dt >= @AddDtFrom");
            parameters.Add("AddDtFrom", filter.AddDtFrom.Value.Date);
        }

        if (filter.AddDtTo.HasValue)
        {
            sql.Append(" AND V.Add_Dt < @AddDtTo");
            parameters.Add("AddDtTo", filter.AddDtTo.Value.Date.AddDays(1));
        }

        if (filter.MdfDtFrom.HasValue)
        {
            sql.Append(" AND V.Mdf_Dt >= @MdfDtFrom");
            parameters.Add("MdfDtFrom", filter.MdfDtFrom.Value.Date);
        }

        if (filter.MdfDtTo.HasValue)
        {
            sql.Append(" AND V.Mdf_Dt < @MdfDtTo");
            parameters.Add("MdfDtTo", filter.MdfDtTo.Value.Date.AddDays(1));
        }

        if (filter.ClntTypCds is { Count: > 0 })
        {
            sql.Append(" AND V.Clnt_Typ_Cd IN @ClntTypCds");
            parameters.Add("ClntTypCds", filter.ClntTypCds);
        }

        if (filter.RiskEstIds is { Count: > 0 })
        {
            sql.Append(" AND V.RskEst_Id IN @RiskEstIds");
            parameters.Add("RiskEstIds", filter.RiskEstIds);
        }

        sql.Append(" ORDER BY V.Mdf_Dt DESC;");

        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<ReportRiskRow>(sql.ToString(), parameters);
        return rows.AsList();
    }
}
