using Dapper;
using eKYC.Domain.ReferenceData;

namespace eKYC.DataAccess.ReferenceData;

public sealed class RiskEstimateRepository : IRiskEstimateRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public RiskEstimateRepository(IDbConnectionFactory connectionFactory)
    {
        ArgumentNullException.ThrowIfNull(connectionFactory);
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<RiskEstimate>> GetAllAsync()
    {
        const string sql = """
            SELECT
                RskEst_Id AS RskEstId,
                L_Pnt     AS LowerPoints,
                U_Pnt     AS UpperPoints,
                Rsk_Lvl   AS RiskLevel,
                Anls_Typ  AS AnalysisType
            FROM CL_Rsk_Est
            ORDER BY L_Pnt;
            """;

        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<RiskEstimate>(sql);
        return rows.AsList();
    }
}
