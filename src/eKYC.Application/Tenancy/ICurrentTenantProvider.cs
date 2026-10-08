namespace eKYC.Application.Tenancy;

/// <summary>
/// Resolves the tenant the current request belongs to. Every tenant-scoped repository call goes through
/// this rather than trusting a tenant id supplied by the client, so multi-tenancy can be turned on later
/// without touching call sites — see the single-tenant stub implementation for what changes when it does.
/// </summary>
public interface ICurrentTenantProvider
{
    int GetCurrentTenantId();
}
