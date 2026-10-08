using Dapper;
using eKYC.Domain.Dashboard;

namespace eKYC.DataAccess.Dashboard;

public sealed class DashboardRepository : IDashboardRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public DashboardRepository(IDbConnectionFactory connectionFactory)
    {
        ArgumentNullException.ThrowIfNull(connectionFactory);
        _connectionFactory = connectionFactory;
    }

    // The legacy dashboard hides clients that no longer need attention — same rule kept here.
    private static readonly string[] ExcludedStatuses = ["ODBIJEN", "AKTIVAN", "ZATVOREN", "PREKID"];

    public async Task<IReadOnlyList<DashboardClientRow>> GetWorkQueueAsync(DashboardFilter filter, int tenantId)
    {
        ArgumentNullException.ThrowIfNull(filter);

        var sql = new System.Text.StringBuilder("""
            SELECT
                C.Clnt_Id        AS ClntId,
                C.HBOR_ID        AS HborId,
                C.Clnt_Typ_Cd    AS ClntTypCd,
                CASE C.Clnt_Typ_Cd
                    WHEN 'P'    THEN 'Fizička osoba'
                    WHEN 'L'    THEN 'Pravna osoba'
                    WHEN 'BEU'  THEN 'Banka EU'
                    WHEN 'BINT' THEN 'Banka int.'
                    ELSE C.Clnt_Typ_Cd
                END              AS VrstaKlijenta,
                COALESCE(P.Nm_Fst + ' ' + P.Nm_Lst, L.Nm, B.Nm, E.Nm, '') AS ClntNm,
                C.Clnt_St        AS ClntSt,
                C.Clnt_Prcsng_St AS ClntPrcsngSt,
                S.Status         AS Status,
                C.RskEst_Id      AS RskEstId,
                C.Rsk_Pnts       AS RskPnts,
                C.PEP_Ind        AS PepInd,
                C.WtchLst_Ind    AS WtchLstInd,
                COALESCE(P.OIB, L.OIB, '') AS Oib,
                (U.Usr_Nm_Fst + ' ' + U.Usr_Nm_Lst) AS ModifiedByName,
                C.Mdf_Dt         AS MdfDt,
                CASE WHEN EXISTS (
                    SELECT 1 FROM CL_MatPod_Compare CMC
                    WHERE CMC.HBOR_ID = C.HBOR_ID AND CMC.Accepted = 0
                ) THEN 1 ELSE 0 END AS MpChange,
                CASE WHEN EXISTS (
                    SELECT 1 FROM CL_MatPod_Compare CMC
                    WHERE CMC.HBOR_ID = C.HBOR_ID AND CMC.Accepted = 0 AND CMC.DoNotCheck = 1
                ) THEN 1 ELSE 0 END AS DoNotCheck,
                CASE WHEN
                    (SELECT COUNT(*) FROM CL_Clnt_Rsk_Qstn_H H WHERE H.Clnt_Id = C.Clnt_Id) > 1
                    AND C.Rsk_Pnts = 0
                    AND (
                        SELECT SUM(Q.Y_Pnt)
                        FROM CL_Clnt_Rsk_Qstn Q
                        WHERE Q.ClntRskQstn_H_Id = (
                            SELECT MAX(H2.ClntRskQstn_H_Id)
                            FROM CL_Clnt_Rsk_Qstn_H H2
                            WHERE H2.Clnt_Id = C.Clnt_Id AND H2.Status = 'I'
                        )
                    ) > 6
                THEN 1 ELSE 0 END AS MustModify
            FROM CL_Clnt C
            LEFT JOIN CL_Clnt_Prsn_Phs P ON C.Clnt_Id = P.Clnt_Id AND C.Clnt_Typ_Cd = 'P'
            LEFT JOIN CL_Clnt_Prsn_Lgl L ON C.Clnt_Id = L.Clnt_Id AND C.Clnt_Typ_Cd = 'L'
            LEFT JOIN CL_Clnt_Bank_EU  B ON C.Clnt_Id = B.Clnt_Id AND C.Clnt_Typ_Cd = 'BEU'
            LEFT JOIN CL_Clnt_Bank_Int E ON C.Clnt_Id = E.Clnt_Id AND C.Clnt_Typ_Cd = 'BINT'
            LEFT JOIN CL_Clnt_PrcsSt  S ON C.Clnt_Prcsng_St = S.Clnt_PrcsSt_Cd
            LEFT JOIN A_Usr U ON C.Mdf_By = U.Usr_Id
            WHERE C.tenant_id = @TenantId
              AND C.Clnt_Prcsng_St NOT IN @ExcludedStatuses
            """);

        var parameters = new DynamicParameters();
        parameters.Add("TenantId", tenantId);
        parameters.Add("ExcludedStatuses", ExcludedStatuses);

        if (!string.IsNullOrWhiteSpace(filter.ClntTypCd))
        {
            sql.Append(" AND C.Clnt_Typ_Cd = @ClntTypCd");
            parameters.Add("ClntTypCd", filter.ClntTypCd);
        }

        if (!string.IsNullOrWhiteSpace(filter.ClntNm))
        {
            sql.Append(" AND COALESCE(P.Nm_Fst + ' ' + P.Nm_Lst, L.Nm, B.Nm, E.Nm, '') LIKE @ClntNmPattern");
            parameters.Add("ClntNmPattern", $"%{filter.ClntNm}%");
        }

        if (!string.IsNullOrWhiteSpace(filter.ClntPrcsngSt))
        {
            sql.Append(" AND C.Clnt_Prcsng_St = @ClntPrcsngSt");
            parameters.Add("ClntPrcsngSt", filter.ClntPrcsngSt);
        }

        if (!string.IsNullOrWhiteSpace(filter.Oib))
        {
            sql.Append(" AND COALESCE(P.OIB, L.OIB, '') LIKE @OibPattern");
            parameters.Add("OibPattern", $"%{filter.Oib}%");
        }

        if (filter.RskEstId.HasValue)
        {
            sql.Append(" AND C.RskEst_Id = @RskEstId");
            parameters.Add("RskEstId", filter.RskEstId.Value);
        }

        sql.Append(" ORDER BY C.Mdf_Dt DESC;");

        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<DashboardClientRow>(sql.ToString(), parameters);
        return rows.AsList();
    }
}
