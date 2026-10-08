using Dapper;
using eKYC.Domain.Dashboard;

namespace eKYC.DataAccess.Dashboard;

public sealed class ClientAnalysisRepository : IClientAnalysisRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public ClientAnalysisRepository(IDbConnectionFactory connectionFactory)
    {
        ArgumentNullException.ThrowIfNull(connectionFactory);
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<DashboardClientRow>> GetByTypeAsync(string clntTypCd, DashboardFilter filter, int tenantId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(clntTypCd);
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
                C.Mdf_Dt         AS MdfDt
            FROM CL_Clnt C
            LEFT JOIN CL_Clnt_Prsn_Phs P ON C.Clnt_Id = P.Clnt_Id AND C.Clnt_Typ_Cd = 'P'
            LEFT JOIN CL_Clnt_Prsn_Lgl L ON C.Clnt_Id = L.Clnt_Id AND C.Clnt_Typ_Cd = 'L'
            LEFT JOIN CL_Clnt_Bank_EU  B ON C.Clnt_Id = B.Clnt_Id AND C.Clnt_Typ_Cd = 'BEU'
            LEFT JOIN CL_Clnt_Bank_Int E ON C.Clnt_Id = E.Clnt_Id AND C.Clnt_Typ_Cd = 'BINT'
            LEFT JOIN CL_Clnt_PrcsSt  S ON C.Clnt_Prcsng_St = S.Clnt_PrcsSt_Cd
            LEFT JOIN A_Usr U ON C.Mdf_By = U.Usr_Id
            WHERE C.tenant_id = @TenantId
              AND C.Clnt_Typ_Cd = @ClntTypCd
            """);

        var parameters = new DynamicParameters();
        parameters.Add("TenantId", tenantId);
        parameters.Add("ClntTypCd", clntTypCd);

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
