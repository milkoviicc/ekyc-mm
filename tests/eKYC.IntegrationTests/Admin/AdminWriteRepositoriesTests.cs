using Dapper;
using eKYC.DataAccess.Admin;
using eKYC.Domain.Admin;
using FluentAssertions;
using Xunit;

namespace eKYC.IntegrationTests.Admin;

/// <summary>
/// Write paths of the admin repositories against the real dev DB. Every test inserts its own marker rows and removes
/// them in a finally block (there is no delete in the API for users/parameters, so the cleanup goes through SQL directly).
/// </summary>
public sealed class AdminWriteRepositoriesTests
{
    private const string Marker = "zz_itest";

    private static readonly eKYC.DataAccess.IDbConnectionFactory Factory = TestConnectionFactory.Create();

    private static async Task ExecuteAsync(string sql, object? args = null)
    {
        using var connection = Factory.CreateConnection();
        await connection.ExecuteAsync(sql, args);
    }

    private static A_Usr NewUser(string login) => new()
    {
        Lgn_Nm = login,
        Usr_Nm_Fst = "Test",
        Usr_St = "V",
        Add_By = 0,
        Add_Dt = DateTime.Now,
        Mdf_By = 0,
        Mdf_Dt = DateTime.Now,
        Current_Host = string.Empty,
    };

    [Fact]
    public async Task A_Usr_Insert_Update_ChangesTheRow()
    {
        var repo = new A_UsrRepository(Factory);
        var id = await repo.InsertAsync(NewUser(Marker));
        try
        {
            var loaded = (await repo.GetByIdAsync(id))!;

            loaded.Usr_Nm_Fst = "Changed";
            (await repo.UpdateAsync(loaded)).Should().Be(1);
            (await repo.GetByIdAsync(id))!.Usr_Nm_Fst.Should().Be("Changed");

            loaded.Usr_Id = -1;
            (await repo.UpdateAsync(loaded)).Should().Be(0, "no such user");

            (await repo.GetByLoginNameAsync(Marker))!.Usr_Id.Should().Be(id);
        }
        finally
        {
            await ExecuteAsync("DELETE FROM A_Usr WHERE Usr_Id = @id", new { id });
        }
    }

    [Fact]
    public async Task A_Usr_ISRole_Assign_IsActive_ThenRevoke()
    {
        var users = new A_UsrRepository(Factory);
        var repo = new A_Usr_ISRoleRepository(Factory);
        var userId = await users.InsertAsync(NewUser(Marker));
        try
        {
            var id = await repo.InsertAsync(new A_Usr_ISRole { Usr_Id = userId, ISRol_Id = 1, Vld_From_Dt = DateTime.Now.AddMinutes(-1) });

            (await repo.HasActiveAssignmentAsync(userId, 1)).Should().BeTrue();
            (await repo.GetActiveRolesForUserAsync(userId)).Should().ContainSingle(r => r.ISRol_Id == 1);

            (await repo.RevokeAsync(id)).Should().Be(1);
            (await repo.HasActiveAssignmentAsync(userId, 1)).Should().BeFalse();
            (await repo.RevokeAsync(id)).Should().Be(0, "an ended assignment cannot be revoked twice");
        }
        finally
        {
            await ExecuteAsync("DELETE FROM A_Usr_ISRole WHERE Usr_Id = @userId; DELETE FROM A_Usr WHERE Usr_Id = @userId", new { userId });
        }
    }

    [Fact]
    public async Task A_ISRole_Objct_Grant_AppearsInPermissions_ThenDelete()
    {
        var repo = new A_ISRole_ObjctRepository(Factory);

        // Role 6 (REVIZIJA) is not granted object 7 (tabReports) in the shipped data.
        (await repo.ExistsAsync(6, 7)).Should().BeFalse("the test assumes a role/object pair that is not granted");

        var id = await repo.InsertAsync(6, 7);
        try
        {
            (await repo.ExistsAsync(6, 7)).Should().BeTrue();
            (await repo.GetObjectsForRoleNamesAsync(["REVIZIJA"])).Should().Contain(o => o.Asmbly_Cd == "tabReports");
        }
        finally
        {
            (await repo.DeleteAsync(id)).Should().Be(1);
        }

        (await repo.GetObjectsForRoleNamesAsync(["REVIZIJA"])).Should().NotContain(o => o.Asmbly_Cd == "tabReports");
        (await repo.GetObjectsForRoleNamesAsync([])).Should().BeEmpty();
    }

    [Fact]
    public async Task A_Apl_Prmtr_Insert_Update_ChangesTheValue()
    {
        var repo = new A_Apl_PrmtrRepository(Factory);
        var id = await repo.InsertAsync(new A_Apl_Prmtr { Prmtr_Cd = Marker, Prmtr_Val = "a", Prmtr_Dspn = "test" });
        try
        {
            var loaded = (await repo.GetByIdAsync(id))!;

            loaded.Prmtr_Val = "b";
            (await repo.UpdateAsync(loaded)).Should().Be(1);

            (await repo.GetByCodeAsync(Marker))!.Prmtr_Val.Should().Be("b");
        }
        finally
        {
            await ExecuteAsync("DELETE FROM A_Apl_Prmtr WHERE Apl_Id = @id", new { id });
        }
    }

    [Fact]
    public async Task A_Object_Locks_Acquire_Conflict_Takeover_Release()
    {
        var users = new A_UsrRepository(Factory);
        var repo = new A_Object_LocksRepository(Factory);
        var alice = await users.InsertAsync(NewUser(Marker + "_a"));
        var bob = await users.InsertAsync(NewUser(Marker + "_b"));
        var request = new AcquireLockRequest { Object_Id = 987654, Object_Class = Marker, Object_Name = "test record" };
        try
        {
            var first = await repo.TryAcquireAsync(request, alice, "PC-A", ttlMinutes: 30);
            first.Acquired.Should().BeTrue();
            first.Lock!.User_Id.Should().Be(alice);

            // Same user again: refreshed, same lock.
            var again = await repo.TryAcquireAsync(request, alice, "PC-A", 30);
            again.Acquired.Should().BeTrue();
            again.Lock!.Object_Lock_Id.Should().Be(first.Lock.Object_Lock_Id);

            // Another user: refused, told who holds it.
            var blocked = await repo.TryAcquireAsync(request, bob, "PC-B", 30);
            blocked.Acquired.Should().BeFalse();
            blocked.Lock!.User_Id.Should().Be(alice);
            blocked.Lock.Lgn_Nm.Should().Be(Marker + "_a");

            // After the TTL the lock counts as abandoned and bob can take it.
            await ExecuteAsync("UPDATE A_Object_Locks SET Locked_At = DATEADD(HOUR, -2, GETDATE()) WHERE Object_Lock_Id = @id", new { id = first.Lock.Object_Lock_Id });
            var takeover = await repo.TryAcquireAsync(request, bob, "PC-B", 30);
            takeover.Acquired.Should().BeTrue();
            takeover.Lock!.User_Id.Should().Be(bob);

            // Only the holder (or an admin) can release or refresh.
            (await repo.HeartbeatAsync(takeover.Lock.Object_Lock_Id, alice)).Should().Be(0);
            (await repo.HeartbeatAsync(takeover.Lock.Object_Lock_Id, bob)).Should().Be(1);
            (await repo.ReleaseAsync(takeover.Lock.Object_Lock_Id, alice, isAdmin: false)).Should().Be(0);
            (await repo.ReleaseAsync(takeover.Lock.Object_Lock_Id, alice, isAdmin: true)).Should().Be(1);
        }
        finally
        {
            await ExecuteAsync("DELETE FROM A_Object_Locks WHERE Object_Class = @Marker; DELETE FROM A_Usr WHERE Usr_Id IN (@alice, @bob)", new { Marker, alice, bob });
        }
    }

    [Fact]
    public async Task Operation_Log_Insert_ThenQueryByFilters()
    {
        var repo = new Operation_LogRepository(Factory);
        await repo.InsertAsync(12, Marker, 1, "code", "INSERT", "integration test");
        try
        {
            var rows = await repo.QueryAsync(new OperationLogFilter { Object_Type = Marker, User_Id = 12, From = DateTime.Today, To = DateTime.Today });
            rows.Should().ContainSingle();
            rows[0].Lgn_Nm.Should().Be("Admin");
            (await repo.GetObjectTypesAsync()).Should().Contain(Marker);
        }
        finally
        {
            await ExecuteAsync("DELETE FROM Operation_Log WHERE Object_Type = @Marker", new { Marker });
        }
    }
}
