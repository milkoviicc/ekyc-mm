using eKYC.DataAccess.Clients;
using eKYC.Domain.Clients;
using eKYC.Domain.Exceptions;

namespace eKYC.Application.Clients;

public sealed class CL_ClntService : ICL_ClntService
{
    private readonly ICL_ClntRepository _clients;

    public CL_ClntService(ICL_ClntRepository clients)
    {
        ArgumentNullException.ThrowIfNull(clients);
        _clients = clients;
    }

    public Task<CL_Clnt?> GetByIdAsync(int clntId, int tenantId) => _clients.GetByIdAsync(clntId, tenantId);

    public Task<IReadOnlyList<CL_Clnt>> GetAllAsync(int tenantId) => _clients.GetAllAsync(tenantId);

    public Task<int> CreateAsync(CL_Clnt client)
    {
        ArgumentNullException.ThrowIfNull(client);
        return _clients.InsertAsync(client);
    }

    public async Task UpdateAsync(CL_Clnt client)
    {
        ArgumentNullException.ThrowIfNull(client);

        var rowsAffected = await _clients.UpdateAsync(client);
        if (rowsAffected == 0)
        {
            var current = await _clients.GetByIdAsync(client.Clnt_Id, client.tenant_id);
            throw new ConcurrencyException<CL_Clnt>(current);
        }
    }
}
