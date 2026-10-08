namespace eKYC.Application.Tenancy;

/// <summary>
/// Stub used until multi-tenancy is actually built out — every request belongs to tenant 1.
/// Replace with a provider that resolves the tenant from the authenticated user's claims when
/// multi-tenant support is picked up; nothing outside this class needs to change.
/// </summary>
public sealed class SingleTenantProvider : ICurrentTenantProvider
{
    public int GetCurrentTenantId() => 1;
}
