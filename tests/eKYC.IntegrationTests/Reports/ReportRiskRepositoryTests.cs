using eKYC.DataAccess.Reports;
using eKYC.Domain.Reports;
using FluentAssertions;
using Xunit;

namespace eKYC.IntegrationTests.Reports;

public sealed class ReportRiskRepositoryTests
{
    [Theory]
    [InlineData(ReportKeys.RiskPeriod)]
    [InlineData(ReportKeys.RiskDay)]
    public async Task GetAsync_RiskReports_OnlyReturnActiveOrRejected(string reportKey)
    {
        var repo = new ReportRiskRepository(TestConnectionFactory.Create());

        var rows = await repo.GetAsync(reportKey, new ReportRiskFilter(), tenantId: 1);

        rows.Should().NotBeEmpty();
        rows.Should().OnlyContain(r => r.ClntPrcsngSt == "AKTIVAN" || r.ClntPrcsngSt == "ODBIJEN");
    }

    [Fact]
    public async Task GetAsync_HighRiskReprocess_OnlyReturnsQualifyingStatusesAndPoints()
    {
        var repo = new ReportRiskRepository(TestConnectionFactory.Create());

        var rows = await repo.GetAsync(ReportKeys.HighRiskReprocess, new ReportRiskFilter(), tenantId: 1);

        rows.Should().OnlyContain(r =>
            (r.ClntPrcsngSt == "UPITNIK KREIRAN" || r.ClntPrcsngSt == "IZMJENA") && r.RskPnts >= 7);
    }

    [Fact]
    public async Task GetAsync_FilteredByClientType_ReturnsOnlyThatType()
    {
        var repo = new ReportRiskRepository(TestConnectionFactory.Create());

        var rows = await repo.GetAsync(
            ReportKeys.RiskPeriod,
            new ReportRiskFilter { ClntTypCds = ["L"] },
            tenantId: 1);

        rows.Should().NotBeEmpty();
        rows.Should().OnlyContain(r => r.ClntTypCd == "L");
    }

    [Fact]
    public async Task GetAsync_UnknownReportKey_Throws()
    {
        var repo = new ReportRiskRepository(TestConnectionFactory.Create());

        var act = () => repo.GetAsync("not-a-real-report", new ReportRiskFilter(), tenantId: 1);

        await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
    }
}
