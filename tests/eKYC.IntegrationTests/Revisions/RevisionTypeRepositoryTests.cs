using eKYC.DataAccess.Revisions;
using FluentAssertions;
using Xunit;

namespace eKYC.IntegrationTests.Revisions;

public sealed class RevisionTypeRepositoryTests
{
    [Fact]
    public async Task GetAllAsync_ReturnsTheThreeKnownRevisionTypes()
    {
        var repo = new RevisionTypeRepository(TestConnectionFactory.Create());

        var types = await repo.GetAllAsync();

        types.Should().HaveCount(3);
        types.Select(t => t.RevTypeCd).Should().Contain(["IRH", "NNFI", "INFI"]);
    }
}
