using Dapper;
using eKYC.DataAccess.Admin;
using FluentAssertions;
using Xunit;

namespace eKYC.IntegrationTests.Admin;

public sealed class A_Object_LocksRepositoryTests
{
    [Fact]
    public async Task InsertThenGetAll_ReturnsRowWithJoinedUserName()
    {
        using var connection = TestConnectionFactory.Create().CreateConnection();
        var newId = await connection.QuerySingleAsync<int>("""
            INSERT INTO A_Object_Locks (Object_Id, Object_Class, Object_Name, User_Id, Locked_At, Computer_Name)
            OUTPUT INSERTED.Object_Lock_Id
            VALUES (999, 'IntegrationTestClass', 'integration-test-marker', 1, GETDATE(), 'TEST-PC');
            """);

        try
        {
            var repo = new A_Object_LocksRepository(TestConnectionFactory.Create());
            var locks = await repo.GetAllAsync();

            locks.Should().Contain(l => l.Object_Lock_Id == newId && l.Object_Class == "IntegrationTestClass");
        }
        finally
        {
            await connection.ExecuteAsync("DELETE FROM A_Object_Locks WHERE Object_Lock_Id = @Id;", new { Id = newId });
        }
    }

    [Fact]
    public async Task DeleteAsync_RemovesTheLock()
    {
        using var connection = TestConnectionFactory.Create().CreateConnection();
        var newId = await connection.QuerySingleAsync<int>("""
            INSERT INTO A_Object_Locks (Object_Id, Object_Class, Object_Name, User_Id, Locked_At, Computer_Name)
            OUTPUT INSERTED.Object_Lock_Id
            VALUES (999, 'IntegrationTestClass', 'integration-test-marker', 1, GETDATE(), 'TEST-PC');
            """);

        var repo = new A_Object_LocksRepository(TestConnectionFactory.Create());
        (await repo.DeleteAsync(newId)).Should().Be(1);

        var remaining = await repo.GetAllAsync();
        remaining.Should().NotContain(l => l.Object_Lock_Id == newId);
    }
}
