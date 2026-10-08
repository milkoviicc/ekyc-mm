using eKYC.DataAccess.ReferenceData;
using FluentAssertions;
using Xunit;

namespace eKYC.IntegrationTests.ReferenceData;

/// <summary>
/// Confirms the Dapper column mappings actually match the live schema — these are the kind of
/// mistakes (wrong column name, wrong alias) that only show up at runtime against a real database.
/// </summary>
public sealed class ReferenceDataRepositoriesTests
{
    [Fact]
    public async Task ClStateRepository_ReturnsRows_WithPopulatedNames()
    {
        var repo = new ClStateRepository(TestConnectionFactory.Create());

        var states = await repo.GetAllAsync();

        states.Should().NotBeEmpty();
        states.Should().OnlyContain(s => !string.IsNullOrWhiteSpace(s.StateNm));
    }

    [Fact]
    public async Task ClientTypeRepository_ReturnsTheFourKnownClientTypes()
    {
        var repo = new ClientTypeRepository(TestConnectionFactory.Create());

        var types = await repo.GetAllAsync();

        types.Count.Should().BeGreaterThanOrEqualTo(4);
        types.Select(t => t.ClntTypCd).Should().Contain(new[] { "P", "L" });
    }

    [Fact]
    public async Task RiskClassRepository_ReturnsRows()
    {
        var repo = new RiskClassRepository(TestConnectionFactory.Create());

        var classes = await repo.GetAllAsync();

        classes.Should().NotBeEmpty();
    }

    [Fact]
    public async Task RiskEstimateRepository_ReturnsRows_WithOrderedPointBands()
    {
        var repo = new RiskEstimateRepository(TestConnectionFactory.Create());

        var estimates = await repo.GetAllAsync();

        estimates.Should().NotBeEmpty();
        estimates.Should().BeInAscendingOrder(e => e.LowerPoints);
    }

    [Fact]
    public async Task OwnershipTypeRepository_ReturnsRows()
    {
        var repo = new OwnershipTypeRepository(TestConnectionFactory.Create());

        var types = await repo.GetAllAsync();

        types.Should().NotBeEmpty();
    }

    [Fact]
    public async Task DocumentTypeRepository_ReturnsRows()
    {
        var repo = new DocumentTypeRepository(TestConnectionFactory.Create());

        var types = await repo.GetAllAsync();

        types.Should().NotBeEmpty();
    }

    [Fact]
    public async Task ClientProcessingStatusRepository_ReturnsStatusesAndTransitions()
    {
        var repo = new ClientProcessingStatusRepository(TestConnectionFactory.Create());

        var statuses = await repo.GetAllAsync();
        var transitions = await repo.GetAllTransitionsAsync();

        statuses.Should().NotBeEmpty();
        transitions.Should().NotBeNull();
    }
}
