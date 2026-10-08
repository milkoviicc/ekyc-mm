using eKYC.Domain.Admin;

namespace eKYC.Application.Admin;

public interface ICurrentUserService
{
    /// <summary>The caller's identity, matching A_Usr row, roles and permitted objects. Cached for the request.</summary>
    Task<CurrentUser> GetCurrentAsync();

    /// <summary>A_Usr.Usr_Id of the caller, or null when no active A_Usr row matches the login name.</summary>
    Task<int?> GetUserIdAsync();
}
