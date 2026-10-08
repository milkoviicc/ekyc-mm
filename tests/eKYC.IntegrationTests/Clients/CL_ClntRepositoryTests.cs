using Dapper;
using eKYC.DataAccess.Clients;
using eKYC.Domain.Clients;
using FluentAssertions;
using Xunit;

namespace eKYC.IntegrationTests.Clients;

/// <summary>
/// Exercises CL_Clnt CRUD against the live dev database, including the new row_version optimistic-locking
/// columns added by migration 001. Every test inserts its own marker row (HBOR_ID = -999) and deletes it in
/// a finally block, so this can run repeatedly against the shared dev DB without leaving junk behind.
/// </summary>
public sealed class CL_ClntRepositoryTests
{
    private const int MarkerHborId = -999;

    private static CL_Clnt BuildTestClient() => new()
    {
        HBOR_ID = MarkerHborId,
        Clnt_Typ_Cd = "P",
        Clnt_St = "A",
        Clnt_Prcsng_St = "NOVI",
        PEP_Ind = "NE",
        Rmrk = "integration-test-marker",
        WtchLst_Ind = "N",
        Add_By = 1,
        Add_Dt = DateTime.UtcNow,
        Mdf_By = 1,
        Mdf_Dt = DateTime.UtcNow,
        tenant_id = 1,
    };

    private static async Task DeleteAsync(int clntId)
    {
        using var connection = TestConnectionFactory.Create().CreateConnection();
        await connection.ExecuteAsync("DELETE FROM CL_Clnt WHERE Clnt_Id = @Clnt_Id;", new { Clnt_Id = clntId });
    }

    [Fact]
    public async Task InsertThenGetById_RoundTripsTheRecord()
    {
        var repo = new CL_ClntRepository(TestConnectionFactory.Create());
        var newId = await repo.InsertAsync(BuildTestClient());

        try
        {
            var loaded = await repo.GetByIdAsync(newId, tenantId: 1);

            loaded.Should().NotBeNull();
            loaded!.HBOR_ID.Should().Be(MarkerHborId);
            loaded.row_version.Should().Be(1, "InsertAsync seeds row_version at 1");
        }
        finally
        {
            await DeleteAsync(newId);
        }
    }

    [Fact]
    public async Task UpdateAsync_WithCurrentRowVersion_Succeeds_AndIncrementsRowVersion()
    {
        var repo = new CL_ClntRepository(TestConnectionFactory.Create());
        var newId = await repo.InsertAsync(BuildTestClient());

        try
        {
            var loaded = await repo.GetByIdAsync(newId, tenantId: 1);
            loaded!.Rmrk = "integration-test-marker-updated";

            var rowsAffected = await repo.UpdateAsync(loaded);

            rowsAffected.Should().Be(1);
            var reloaded = await repo.GetByIdAsync(newId, tenantId: 1);
            reloaded!.Rmrk.Should().Be("integration-test-marker-updated");
            reloaded.row_version.Should().Be(2, "a successful update increments row_version");
        }
        finally
        {
            await DeleteAsync(newId);
        }
    }

    [Fact]
    public async Task UpdateAsync_WithStaleRowVersion_AffectsZeroRows()
    {
        var repo = new CL_ClntRepository(TestConnectionFactory.Create());
        var newId = await repo.InsertAsync(BuildTestClient());

        try
        {
            var firstEditor = await repo.GetByIdAsync(newId, tenantId: 1);
            var secondEditor = await repo.GetByIdAsync(newId, tenantId: 1);

            firstEditor!.Rmrk = "saved-first";
            (await repo.UpdateAsync(firstEditor)).Should().Be(1);

            // secondEditor still holds the pre-update row_version — this is exactly the conflict
            // Pattern 1 exists to catch.
            secondEditor!.Rmrk = "saved-second-should-not-apply";
            var rowsAffected = await repo.UpdateAsync(secondEditor);

            rowsAffected.Should().Be(0, "the row_version secondEditor holds is now stale");
        }
        finally
        {
            await DeleteAsync(newId);
        }
    }
}
