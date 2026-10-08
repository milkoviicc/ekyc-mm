using eKYC.DataAccess.Dashboard;
using eKYC.Domain.Dashboard;
using FluentAssertions;
using Xunit;

namespace eKYC.IntegrationTests.Dashboard;

public sealed class ClientAnalysisRepositoryTests
{
    [Theory]
    [InlineData("P")]
    [InlineData("L")]
    [InlineData("BEU")]
    [InlineData("BINT")]
    public async Task GetByTypeAsync_ReturnsOnlyThatType_IncludingFinalStatuses(string clntTypCd)
    {
        var repo = new ClientAnalysisRepository(TestConnectionFactory.Create());

        var rows = await repo.GetByTypeAsync(clntTypCd, new DashboardFilter(), tenantId: 1);

        rows.Should().NotBeEmpty();
        rows.Should().OnlyContain(r => r.ClntTypCd == clntTypCd);
        // Unlike the dashboard work queue, this view must include clients in a final status too (e.g. AKTIVAN).
        rows.Should().Contain(r => r.ClntPrcsngSt == "AKTIVAN");
    }

    [Fact]
    public async Task GetByTypeAsync_FilteredByOib_NarrowsResults()
    {
        var repo = new ClientAnalysisRepository(TestConnectionFactory.Create());

        var all = await repo.GetByTypeAsync("P", new DashboardFilter(), tenantId: 1);
        var sampleOib = all.Select(r => r.Oib).FirstOrDefault(o => !string.IsNullOrWhiteSpace(o));
        sampleOib.Should().NotBeNullOrWhiteSpace("test data should have at least one physical client with an OIB");

        var filtered = await repo.GetByTypeAsync("P", new DashboardFilter { Oib = sampleOib }, tenantId: 1);

        filtered.Should().NotBeEmpty();
        filtered.Should().OnlyContain(r => r.Oib != null && r.Oib.Contains(sampleOib!));
    }
}
