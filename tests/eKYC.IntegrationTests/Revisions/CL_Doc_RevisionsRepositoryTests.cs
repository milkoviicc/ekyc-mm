using Dapper;
using eKYC.DataAccess.Revisions;
using eKYC.Domain.Revisions;
using FluentAssertions;
using Xunit;

namespace eKYC.IntegrationTests.Revisions;

/// <summary>
/// First repository in this project with real Insert/Update/Delete — exercises the full CRUD +
/// optimistic-locking cycle against the live dev DB, same pattern as CL_ClntRepositoryTests.
/// </summary>
public sealed class CL_Doc_RevisionsRepositoryTests
{
    private static CL_Doc_Revisions BuildTestRevision(int revTypeId) => new()
    {
        CL_Doc_Typ_Rev_Id = revTypeId,
        Rev_Date = new DateTime(2026, 1, 15),
        Rev_Range_From = new DateTime(2026, 1, 1),
        Rev_Range_To = new DateTime(2026, 1, 31),
        Rev_Done_By = "integration-test-marker",
        Subject = "Integration test revision",
        Recommendation = "N/A",
        tenant_id = 1,
    };

    private static async Task DeleteAsync(int docRevisionId)
    {
        using var connection = TestConnectionFactory.Create().CreateConnection();
        await connection.ExecuteAsync(
            "DELETE FROM CL_Doc_Revisions WHERE CL_Doc_Revision_Id = @Id;", new { Id = docRevisionId });
    }

    [Fact]
    public async Task InsertThenGetById_RoundTripsTheRecord()
    {
        var repo = new CL_Doc_RevisionsRepository(TestConnectionFactory.Create());
        var newId = await repo.InsertAsync(BuildTestRevision(revTypeId: 1));

        try
        {
            var loaded = await repo.GetByIdAsync(newId, tenantId: 1);

            loaded.Should().NotBeNull();
            loaded!.Rev_Done_By.Should().Be("integration-test-marker");
            loaded.row_version.Should().Be(1);
        }
        finally
        {
            await DeleteAsync(newId);
        }
    }

    [Fact]
    public async Task UpdateAsync_WithCurrentRowVersion_Succeeds_AndIncrementsRowVersion()
    {
        var repo = new CL_Doc_RevisionsRepository(TestConnectionFactory.Create());
        var newId = await repo.InsertAsync(BuildTestRevision(revTypeId: 1));

        try
        {
            var loaded = await repo.GetByIdAsync(newId, tenantId: 1);
            loaded!.Subject = "Updated subject";

            (await repo.UpdateAsync(loaded)).Should().Be(1);

            var reloaded = await repo.GetByIdAsync(newId, tenantId: 1);
            reloaded!.Subject.Should().Be("Updated subject");
            reloaded.row_version.Should().Be(2);
        }
        finally
        {
            await DeleteAsync(newId);
        }
    }

    [Fact]
    public async Task UpdateAsync_WithStaleRowVersion_AffectsZeroRows()
    {
        var repo = new CL_Doc_RevisionsRepository(TestConnectionFactory.Create());
        var newId = await repo.InsertAsync(BuildTestRevision(revTypeId: 1));

        try
        {
            var firstEditor = await repo.GetByIdAsync(newId, tenantId: 1);
            var secondEditor = await repo.GetByIdAsync(newId, tenantId: 1);

            firstEditor!.Subject = "saved-first";
            (await repo.UpdateAsync(firstEditor)).Should().Be(1);

            secondEditor!.Subject = "saved-second-should-not-apply";
            (await repo.UpdateAsync(secondEditor)).Should().Be(0);
        }
        finally
        {
            await DeleteAsync(newId);
        }
    }

    [Fact]
    public async Task DeleteAsync_RemovesTheRecord()
    {
        var repo = new CL_Doc_RevisionsRepository(TestConnectionFactory.Create());
        var newId = await repo.InsertAsync(BuildTestRevision(revTypeId: 1));

        (await repo.DeleteAsync(newId, tenantId: 1)).Should().Be(1);
        (await repo.GetByIdAsync(newId, tenantId: 1)).Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_FilteredByYear_ReturnsOnlyMatchingYear()
    {
        var repo = new CL_Doc_RevisionsRepository(TestConnectionFactory.Create());
        var newId = await repo.InsertAsync(BuildTestRevision(revTypeId: 1));

        try
        {
            var rows2026 = await repo.GetAllAsync(new DocRevisionFilter { Year = 2026 }, tenantId: 1);
            var rows1999 = await repo.GetAllAsync(new DocRevisionFilter { Year = 1999 }, tenantId: 1);

            rows2026.Should().Contain(r => r.CL_Doc_Revision_Id == newId);
            rows1999.Should().NotContain(r => r.CL_Doc_Revision_Id == newId);
        }
        finally
        {
            await DeleteAsync(newId);
        }
    }
}
