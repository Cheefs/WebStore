using Microsoft.AspNetCore.Identity;
using System.Net.Http.Json;
using WebStore.Domain.Entities.Identity;
using WebStore.Interfaces;
using WebStore.WebAPI.Clients.Base;

namespace WebStore.WebAPI.Clients.Identity;

public class RolesClient : BaseClient
{
    public RolesClient(HttpClient client) : base(client, WebApiAdresses.V1.Identity.Roles) {}

    #region IRoleStore<Role>

    public async Task<IdentityResult> CreateAsync(Role role, CancellationToken cancel)
    {
        var response = await PostAsync(Address, role, cancel).ConfigureAwait(false);
        var result = await response
           .EnsureSuccessStatusCode()
           .Content
           .ReadFromJsonAsync<bool>(cancellationToken: cancel).ConfigureAwait(false);

        return result
            ? IdentityResult.Success
            : IdentityResult.Failed();
    }

    public async Task<IdentityResult> UpdateAsync(Role role, CancellationToken cancel)
    {
        var response = await PutAsync(Address, role, cancel).ConfigureAwait(false);
        var result = await response
           .EnsureSuccessStatusCode()
           .Content
           .ReadFromJsonAsync<bool>(cancellationToken: cancel).ConfigureAwait(false);

        return result
            ? IdentityResult.Success
            : IdentityResult.Failed();
    }

    public async Task<IdentityResult> DeleteAsync(Role role, CancellationToken cancel)
    {
        var response = await PostAsync($"{Address}/delete", role, cancel).ConfigureAwait(false);
        var result = await response
           .EnsureSuccessStatusCode()
           .Content
           .ReadFromJsonAsync<bool>(cancellationToken: cancel).ConfigureAwait(false);

        return result
            ? IdentityResult.Success
            : IdentityResult.Failed();
    }

    public async Task<string> GetRoleIdAsync(Role role, CancellationToken cancel)
    {
        var response = await PostAsync($"{Address}/getRoleId", role, cancel).ConfigureAwait(false);
        return await response
           .EnsureSuccessStatusCode()
           .Content
           .ReadAsStringAsync(cancel)
           .ConfigureAwait(false);
    }

    public async Task<string> GetRoleNameAsync(Role role, CancellationToken cancel)
    {
        var response = await PostAsync($"{Address}/getRoleName", role, cancel).ConfigureAwait(false);
        return await response
           .EnsureSuccessStatusCode()
           .Content
           .ReadAsStringAsync(cancel)
           .ConfigureAwait(false);
    }

    public async Task SetRoleNameAsync(Role role, string name, CancellationToken cancel)
    {
        var response = await PostAsync($"{Address}/setRoleName/{name}", role, cancel).ConfigureAwait(false);
        role.Name = await response
           .EnsureSuccessStatusCode()
           .Content
           .ReadAsStringAsync(cancel)
           .ConfigureAwait(false);
    }

    public async Task<string> GetNormalizedRoleNameAsync(Role role, CancellationToken cancel)
    {
        var response = await PostAsync($"{Address}/getNormalizedRoleName", role, cancel).ConfigureAwait(false);
        return await response
           .EnsureSuccessStatusCode()
           .Content
           .ReadAsStringAsync(cancel)
           .ConfigureAwait(false);
    }

    public async Task SetNormalizedRoleNameAsync(Role role, string name, CancellationToken cancel)
    {
        var response = await PostAsync($"{Address}/setNormalizedRoleName/{name}", role, cancel).ConfigureAwait(false);
        role.NormalizedName = await response
           .EnsureSuccessStatusCode()
           .Content
           .ReadAsStringAsync(cancel)
           .ConfigureAwait(false);
    }

    public async Task<Role> FindByIdAsync(string id, CancellationToken cancel)
    {
        var role = await GetAsync<Role>($"{Address}/findById/{id}", cancel).ConfigureAwait(false);
        return role!;
    }

    public async Task<Role> FindByNameAsync(string name, CancellationToken cancel)
    {
        var role = await GetAsync<Role>($"{Address}/findByName/{name}", cancel).ConfigureAwait(false);
        return role!;
    }

    #endregion
}
