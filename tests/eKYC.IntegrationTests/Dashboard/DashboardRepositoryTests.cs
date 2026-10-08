using eKYC.DataAccess.Dashboard;
using eKYC.Domain.Dashboard;
using FluentAssertions;
using Xunit;

namespace eKYC.IntegrationTests.Dashboard;

public sealed class DashboardRepositoryTests
{
    private static readonly string[] ExcludedStatuses = ["ODBIJEN", "AKTIVAN", "ZATVOREN", "PREKID"];

    [Fact]
    public async Task GetWorkQueueAsync_NoFilter_ExcludesFinalStatuses_AndPopulatesJoinedFields()
    {
        var repo = new DashboardRepository(TestConnectionFactory.Create());

        var rows = await repo.GetWorkQueueAsync(new DashboardFilter(), tenantId: 1);

        rows.Should().NotBeEmpty();
        rows.Should().OnlyContain(r => !ExcludedStatuses.Contains(r.ClntPrcsngSt));
        rows.Should().OnlyContain(r => !string.IsNullOrWhiteSpace(r.VrstaKlijenta));
    }

    [Fact]
    public async Task GetWorkQueueAsync_FilteredByClientType_ReturnsOnlyThatType()
    {
        var repo = new DashboardRepository(TestConnectionFactory.Create());

        var rows = await repo.GetWorkQueueAsync(new DashboardFilter { ClntTypCd = "L" }, tenantId: 1);

        rows.Should().NotBeEmpty();
        rows.Should().OnlyContain(r => r.ClntTypCd == "L");
    }

    [Fact]
    public async Task GetWorkQueueAsync_WrongTenant_ReturnsEmpty()
    {
        var repo = new DashboardRepository(TestConnectionFactory.Create());

        var rows = await repo.GetWorkQueueAsync(new DashboardFilter(), tenantId: 999);

        rows.Should().BeEmpty();
    }

    // Real dev-data regression cases found by manual comparison against the legacy app: both clients
    // have Rsk_Pnts = 0 but their latest completed ('I') risk questionnaire scored well above 6
    // (383 -> 11 pts, 397 -> 113 pts) — confirmed directly against CL_Clnt_Rsk_Qstn_H/CL_Clnt_Rsk_Qstn.
    [Theory]
    [InlineData(383)]
    [InlineData(397)]
    public async Task GetWorkQueueAsync_StaleZeroScoreWithRealRiskHistory_IsFlaggedMustModify(int clntId)
    {
        var repo = new DashboardRepository(TestConnectionFactory.Create());

        var rows = await repo.GetWorkQueueAsync(new DashboardFilter(), tenantId: 1);

        rows.Should().ContainSingle(r => r.ClntId == clntId)
            .Which.MustModify.Should().BeTrue();
    }
}
