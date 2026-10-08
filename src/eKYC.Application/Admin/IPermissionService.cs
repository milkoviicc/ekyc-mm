namespace eKYC.Application.Admin;

public interface IPermissionService
{
    /// <summary>True if the caller's roles grant at least one of the objects with these A_Objct.Asmbly_Cd codes (e.g. "tabAdmin").</summary>
    Task<bool> HasAnyObjectAsync(params string[] assemblyCodes);
}
