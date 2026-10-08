using eKYC.Application.Admin;
using eKYC.DataAccess.Admin;
using eKYC.Domain.Admin;
using eKYC.Domain.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;

namespace eKYC.UnitTests.Admin;

public sealed class AdminServicesTests
{
    private static (A_UsrService Service, Mock<IA_UsrRepository> Repo) UserService()
    {
        var repo = new Mock<IA_UsrRepository>();
        var current = new Mock<ICurrentUserService>();
        current.Setup(c => c.GetUserIdAsync()).ReturnsAsync(12);
        return (new A_UsrService(repo.Object, current.Object, Mock.Of<IAuditTrail>()), repo);
    }

    [Fact]
    public async Task CreateUser_WithDuplicateLogin_IsRejected()
    {
        var (service, repo) = UserService();
        repo.Setup(r => r.GetByLoginNameAsync("Admin")).ReturnsAsync(new A_Usr { Usr_Id = 12, Lgn_Nm = "Admin" });

        var act = () => service.CreateAsync(new A_Usr { Lgn_Nm = " Admin " });

        await act.Should().ThrowAsync<ValidationException>().WithMessage("*već postoji*");
        repo.Verify(r => r.InsertAsync(It.IsAny<A_Usr>()), Times.Never);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("a-login-name-that-is-longer-than-twenty")]
    public async Task CreateUser_WithMissingOrTooLongLogin_IsRejected(string login)
    {
        var (service, _) = UserService();

        var act = () => service.CreateAsync(new A_Usr { Lgn_Nm = login });

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task UpdateUser_UnknownId_ReturnsNull()
    {
        var (service, repo) = UserService();
        repo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((A_Usr?)null);

        (await service.UpdateAsync(99, new A_Usr { Lgn_Nm = "x" })).Should().BeNull();
    }

    [Fact]
    public async Task AssignRole_UserAlreadyHoldsIt_IsRejected()
    {
        var assignments = new Mock<IA_Usr_ISRoleRepository>();
        var users = new Mock<IA_UsrRepository>();
        var roles = new Mock<IA_ISRoleRepository>();
        users.Setup(u => u.GetByIdAsync(2)).ReturnsAsync(new A_Usr { Usr_Id = 2, Lgn_Nm = "Unos" });
        roles.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new A_ISRole { ISRol_Id = 1, ISRol_Nm = "UNOS" });
        assignments.Setup(a => a.HasActiveAssignmentAsync(2, 1)).ReturnsAsync(true);
        var service = new A_Usr_ISRoleService(assignments.Object, users.Object, roles.Object, Mock.Of<IAuditTrail>());

        var act = () => service.AssignAsync(new A_Usr_ISRole { Usr_Id = 2, ISRol_Id = 1 });

        await act.Should().ThrowAsync<ValidationException>().WithMessage("*već ima ulogu*");
    }

    [Fact]
    public async Task AssignRole_EndBeforeStart_IsRejected()
    {
        var users = new Mock<IA_UsrRepository>();
        var roles = new Mock<IA_ISRoleRepository>();
        users.Setup(u => u.GetByIdAsync(2)).ReturnsAsync(new A_Usr { Usr_Id = 2 });
        roles.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new A_ISRole { ISRol_Id = 1 });
        var service = new A_Usr_ISRoleService(Mock.Of<IA_Usr_ISRoleRepository>(), users.Object, roles.Object, Mock.Of<IAuditTrail>());

        var act = () => service.AssignAsync(new A_Usr_ISRole
        {
            Usr_Id = 2,
            ISRol_Id = 1,
            Vld_From_Dt = new DateTime(2026, 5, 1),
            Vld_To_Dt = new DateTime(2026, 4, 1),
        });

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task PermissionService_MatchesAssemblyCodesIgnoringCase()
    {
        var current = new Mock<ICurrentUserService>();
        current.Setup(c => c.GetCurrentAsync()).ReturnsAsync(new CurrentUser
        {
            Permissions = [new A_Objct { Objct_Id = 5, Asmbly_Cd = "tabAdmin" }],
        });
        var service = new PermissionService(current.Object);

        (await service.HasAnyObjectAsync("TABADMIN")).Should().BeTrue();
        (await service.HasAnyObjectAsync("tabReports", "tabRevision")).Should().BeFalse();
    }

    [Fact]
    public async Task AcquireLock_WithoutMatchingUser_IsForbidden()
    {
        var current = new Mock<ICurrentUserService>();
        current.Setup(c => c.GetUserIdAsync()).ReturnsAsync((int?)null);
        var context = new Mock<ICurrentUserContext>();
        context.Setup(c => c.ClientHost).Returns("10.0.0.1");
        var service = new A_Object_LocksService(
            Mock.Of<IA_Object_LocksRepository>(), current.Object, context.Object, new ObjectLockSettings(), Mock.Of<IAuditTrail>());

        var act = () => service.AcquireAsync(new AcquireLockRequest { Object_Id = 1, Object_Class = "CL_Doc_Revisions" });

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }
}
