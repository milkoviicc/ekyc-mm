using eKYC.Application.Revisions;
using eKYC.DataAccess.Revisions;
using eKYC.Domain.Exceptions;
using eKYC.Domain.Revisions;
using FluentAssertions;
using Moq;
using Xunit;

namespace eKYC.UnitTests.Revisions;

public sealed class CL_Doc_RevisionsServiceTests
{
    [Fact]
    public async Task UpdateAsync_WhenRepositoryUpdatesOneRow_DoesNotThrow()
    {
        var repository = new Mock<ICL_Doc_RevisionsRepository>();
        repository.Setup(r => r.UpdateAsync(It.IsAny<CL_Doc_Revisions>())).ReturnsAsync(1);
        var service = new CL_Doc_RevisionsService(repository.Object);

        var act = () => service.UpdateAsync(new CL_Doc_Revisions { CL_Doc_Revision_Id = 1, tenant_id = 1, row_version = 3 });

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task UpdateAsync_WhenRepositoryAffectsZeroRows_ThrowsConcurrencyExceptionWithCurrentRecord()
    {
        var currentRecord = new CL_Doc_Revisions { CL_Doc_Revision_Id = 1, tenant_id = 1, row_version = 4, Subject = "someone else's edit" };
        var repository = new Mock<ICL_Doc_RevisionsRepository>();
        repository.Setup(r => r.UpdateAsync(It.IsAny<CL_Doc_Revisions>())).ReturnsAsync(0);
        repository.Setup(r => r.GetByIdAsync(1, 1)).ReturnsAsync(currentRecord);
        var service = new CL_Doc_RevisionsService(repository.Object);

        var act = () => service.UpdateAsync(new CL_Doc_Revisions { CL_Doc_Revision_Id = 1, tenant_id = 1, row_version = 3 });

        // Caught as the non-generic base here on purpose — this is exactly what the shared
        // ExceptionHandlingMiddleware catch clause relies on.
        var exception = await act.Should().ThrowAsync<ConcurrencyException>();
        exception.Which.CurrentRecordObject.Should().BeSameAs(currentRecord);
    }
}
