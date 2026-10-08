using eKYC.Application.Clients;
using eKYC.DataAccess.Clients;
using eKYC.Domain.Clients;
using eKYC.Domain.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;

namespace eKYC.UnitTests.Clients;

public sealed class CL_ClntServiceTests
{
    [Fact]
    public async Task UpdateAsync_WhenRepositoryUpdatesOneRow_DoesNotThrow()
    {
        var repository = new Mock<ICL_ClntRepository>();
        repository.Setup(r => r.UpdateAsync(It.IsAny<CL_Clnt>())).ReturnsAsync(1);
        var service = new CL_ClntService(repository.Object);

        var act = () => service.UpdateAsync(new CL_Clnt { Clnt_Id = 1, tenant_id = 1, row_version = 3 });

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task UpdateAsync_WhenRepositoryAffectsZeroRows_ThrowsConcurrencyExceptionWithCurrentRecord()
    {
        var currentRecord = new CL_Clnt { Clnt_Id = 1, tenant_id = 1, row_version = 4, Rmrk = "someone else's edit" };
        var repository = new Mock<ICL_ClntRepository>();
        repository.Setup(r => r.UpdateAsync(It.IsAny<CL_Clnt>())).ReturnsAsync(0);
        repository.Setup(r => r.GetByIdAsync(1, 1)).ReturnsAsync(currentRecord);
        var service = new CL_ClntService(repository.Object);

        var act = () => service.UpdateAsync(new CL_Clnt { Clnt_Id = 1, tenant_id = 1, row_version = 3 });

        var exception = await act.Should().ThrowAsync<ConcurrencyException<CL_Clnt>>();
        exception.Which.CurrentRecord.Should().BeSameAs(currentRecord);
    }
}
