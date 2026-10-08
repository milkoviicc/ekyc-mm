using eKYC.DataAccess.Admin;
using FluentAssertions;
using Xunit;

namespace eKYC.IntegrationTests.Admin;

/// <summary>Read-only smoke tests: every column of each A_ table must map onto its model against the real dev DB.</summary>
public sealed class AdminRepositoriesTests
{
    private static readonly eKYC.DataAccess.IDbConnectionFactory Factory = TestConnectionFactory.Create();


    [Fact]
    public async Task A_Usr_GetAll_MapsColumns()
    {
        var rows = await new A_UsrRepository(Factory).GetAllAsync();
        rows.Should().NotBeEmpty();
        rows.Should().OnlyContain(u => u.Usr_Id > 0 && u.Lgn_Nm != null);
        (await new A_UsrRepository(Factory).GetByIdAsync(rows[0].Usr_Id))!.Lgn_Nm.Should().Be(rows[0].Lgn_Nm);
    }

    [Fact]
    public async Task A_ISRole_GetAll_MapsColumns()
    {
        var rows = await new A_ISRoleRepository(Factory).GetAllAsync();
        rows.Should().OnlyContain(r => r.ISRol_Id > 0 && r.ISRol_Nm != null);
    }

    [Fact]
    public async Task A_Usr_ISRole_GetAll_MapsColumns()
    {
        var rows = await new A_Usr_ISRoleRepository(Factory).GetAllAsync();
        rows.Should().OnlyContain(r => r.Usr_ISRol_Id > 0 && r.Usr_Id != null && r.ISRol_Id != null);
    }

    [Fact]
    public async Task A_ISRole_Objct_GetAll_MapsColumns()
    {
        var rows = await new A_ISRole_ObjctRepository(Factory).GetAllAsync();
        rows.Should().OnlyContain(r => r.ISRol_Objct_Id > 0 && r.Objct_Id != null);
    }

    [Fact]
    public async Task A_Objct_GetAll_MapsColumns()
    {
        var rows = await new A_ObjctRepository(Factory).GetAllAsync();
        rows.Should().OnlyContain(o => o.Objct_Id > 0 && o.Objct_Nm != null);
    }

    [Fact]
    public async Task A_Objct_Typs_GetAll_MapsColumns()
    {
        var rows = await new A_Objct_TypsRepository(Factory).GetAllAsync();
        rows.Should().OnlyContain(t => t.Objct_Typ_Id > 0 && t.Typ_Nm != null);
    }

    [Fact]
    public async Task A_Apl_Prmtr_GetAll_MapsColumns()
    {
        var rows = await new A_Apl_PrmtrRepository(Factory).GetAllAsync();
        rows.Should().OnlyContain(p => p.Apl_Id > 0 && p.Prmtr_Cd != null);
    }
}
