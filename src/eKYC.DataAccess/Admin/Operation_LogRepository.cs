using Dapper;
using eKYC.Domain.Admin;

namespace eKYC.DataAccess.Admin;

public sealed class Operation_LogRepository : IOperation_LogRepository
{
    private const int MaxRows = 5000;

    private readonly IDbConnectionFactory _connectionFactory;

    public Operation_LogRepository(IDbConnectionFactory connectionFactory)
    {
        ArgumentNullException.ThrowIfNull(connectionFactory);
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<Operation_Log>> QueryAsync(OperationLogFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);

        var sql = new System.Text.StringBuilder("""
            SELECT TOP (@Take)
                O.Log_Id, O.Log_Timestamp, O.User_Id, O.Object_Type, O.Object_Id, O.Object_Code, O.Operation_Type, O.Log_Remark,
                U.Lgn_Nm, U.Usr_Nm_Fst, U.Usr_Nm_Lst
            FROM Operation_Log O
            LEFT JOIN A_Usr U ON O.User_Id = U.Usr_Id
            WHERE 1 = 1
            """);

        var parameters = new DynamicParameters();
        parameters.Add("Take", Math.Clamp(filter.Take, 1, MaxRows));

        if (filter.From.HasValue)
        {
            sql.Append(" AND O.Log_Timestamp >= @From");
            parameters.Add("From", filter.From.Value.Date);
        }

        if (filter.To.HasValue)
        {
            // "To" is a calendar day and includes that whole day.
            sql.Append(" AND O.Log_Timestamp < @ToExclusive");
            parameters.Add("ToExclusive", filter.To.Value.Date.AddDays(1));
        }

        if (filter.User_Id.HasValue)
        {
            sql.Append(" AND O.User_Id = @UserId");
            parameters.Add("UserId", filter.User_Id.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.Object_Type))
        {
            sql.Append(" AND O.Object_Type = @ObjectType");
            parameters.Add("ObjectType", filter.Object_Type);
        }

        sql.Append(" ORDER BY O.Log_Timestamp DESC, O.Log_Id DESC;");

        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<Operation_Log>(sql.ToString(), parameters);
        return rows.AsList();
    }

    public async Task<IReadOnlyList<string>> GetObjectTypesAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<string>("SELECT DISTINCT Object_Type FROM Operation_Log ORDER BY Object_Type;");
        return rows.AsList();
    }

    public async Task InsertAsync(int userId, string objectType, int objectId, string objectCode, string operationType, string remark)
    {
        const string sql = """
            INSERT INTO Operation_Log (Log_Timestamp, User_Id, Object_Type, Object_Id, Object_Code, Operation_Type, Log_Remark)
            VALUES (GETDATE(), @UserId, @ObjectType, @ObjectId, @ObjectCode, @OperationType, @Remark);
            """;

        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(sql, new { UserId = userId, ObjectType = objectType, ObjectId = objectId, ObjectCode = objectCode, OperationType = operationType, Remark = remark });
    }
}
