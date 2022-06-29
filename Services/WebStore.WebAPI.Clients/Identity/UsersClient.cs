using Microsoft.AspNetCore.Identity;
using System.Net.Http.Json;
using System.Security.Claims;
using WebStore.Domain.DTO.Identity;
using WebStore.Domain.Entities.Identity;
using WebStore.Interfaces;
using WebStore.Interfaces.Services.Identity;
using WebStore.WebAPI.Clients.Base;

namespace WebStore.WebAPI.Clients.Identity;

public class UsersClient : BaseClient, IUsersClient
{
    public UsersClient(HttpClient client) : base(client, WebApiAdresses.V1.Identity.Users) {}

    #region Implementation of IUserStore<User>

    public async Task<string> GetUserIdAsync(User user, CancellationToken cancel)
    {
        var response = await PostAsync($"{Address}/userId", user, cancel);
        return await response
           .EnsureSuccessStatusCode()
           .Content
           .ReadAsStringAsync(cancel)
           .ConfigureAwait(false);
    }

    public async Task<string> GetUserNameAsync(User user, CancellationToken cancel)
    {
        var response = await PostAsync($"{Address}/userName", user, cancel);
        return await response
           .EnsureSuccessStatusCode()
           .Content
           .ReadAsStringAsync(cancel)
           .ConfigureAwait(false);
    }

    public async Task SetUserNameAsync(User user, string name, CancellationToken cancel)
    {
        var response = await PostAsync($"{Address}/userName/{name}", user, cancel);
        user.UserName = await response
           .EnsureSuccessStatusCode()
           .Content
           .ReadAsStringAsync(cancel)
           .ConfigureAwait(false);
    }

    public async Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancel)
    {
        var response = await PostAsync($"{Address}/normalUserName/", user, cancel);
        return await response
           .EnsureSuccessStatusCode()
           .Content
           .ReadAsStringAsync(cancel)
           .ConfigureAwait(false);
    }

    public async Task SetNormalizedUserNameAsync(User user, string name, CancellationToken cancel)
    {
        var response = await PostAsync($"{Address}/normalUserName/{name}", user, cancel);
        user.NormalizedUserName = await response
           .EnsureSuccessStatusCode()
           .Content
           .ReadAsStringAsync(cancel)
           .ConfigureAwait(false);
    }

    public async Task<IdentityResult> CreateAsync(User user, CancellationToken cancel)
    {
        var response = await PostAsync(Address, user, cancel);
        var result = await response
           .EnsureSuccessStatusCode()
           .Content
           .ReadFromJsonAsync<bool>(cancellationToken: cancel)
           .ConfigureAwait(false);

        return result
            ? IdentityResult.Success
            : IdentityResult.Failed();
    }

    public async Task<IdentityResult> UpdateAsync(User user, CancellationToken cancel)
    {
        var response = await PutAsync(Address, user, cancel);
        var result = await response
           .EnsureSuccessStatusCode()
           .Content
           .ReadFromJsonAsync<bool>(cancellationToken: cancel)
           .ConfigureAwait(false);

        return result
            ? IdentityResult.Success
            : IdentityResult.Failed();
    }

    public async Task<IdentityResult> DeleteAsync(User user, CancellationToken cancel)
    {
        var response = await PostAsync($"{Address}/delete", user, cancel);
        var result = await response
           .EnsureSuccessStatusCode()
           .Content
           .ReadFromJsonAsync<bool>(cancellationToken: cancel)
           .ConfigureAwait(false);

        return result
            ? IdentityResult.Success
            : IdentityResult.Failed();
    }

    public async Task<User> FindByIdAsync(string id, CancellationToken cancel)
    {
        var result = await GetAsync<User>($"{Address}/find/{id}", cancel).ConfigureAwait(false);
        return result!;
    }
        

    public async Task<User> FindByNameAsync(string name, CancellationToken cancel)
    {
        var result = await GetAsync<User>($"{Address}/normal/{name}", cancel).ConfigureAwait(false);
        return result!;
    } 

    #endregion

    #region Implementation of IUserRoleStore<User>

    public async Task AddToRoleAsync(User user, string role, CancellationToken cancel) =>
        await PostAsync($"{Address}/role/{role}", user, cancel).ConfigureAwait(false);
    

    public async Task RemoveFromRoleAsync(User user, string role, CancellationToken cancel) =>
        await PostAsync($"{Address}/role/delete/{role}", user, cancel).ConfigureAwait(false);

    public async Task<IList<string>> GetRolesAsync(User user, CancellationToken cancel)
    {
        var response = await PostAsync($"{Address}/roles", user, cancel).ConfigureAwait(false);
        var roles = await response
           .EnsureSuccessStatusCode()
           .Content
           .ReadFromJsonAsync<IList<string>>(cancellationToken: cancel)
           .ConfigureAwait(false);
        return roles!;
    }

    public async Task<bool> IsInRoleAsync(User user, string role, CancellationToken cancel)
    {
        var response = await PostAsync($"{Address}/inRole/{role}", user, cancel);
        return await response
           .EnsureSuccessStatusCode()
           .Content
           .ReadFromJsonAsync<bool>(cancellationToken: cancel)
           .ConfigureAwait(false);
    }

    public async Task<IList<User>> GetUsersInRoleAsync(string role, CancellationToken cancel)
    {
        var users = await GetAsync<List<User>>($"{Address}/usersInRole/{role}", cancel).ConfigureAwait(false);
        return users!;
    }

    #endregion

    #region Implementation of IUserPasswordStore<User>

    public async Task SetPasswordHashAsync(User user, string hash, CancellationToken cancel)
    {
        var response = await PostAsync($"{Address}/setPasswordHash", new PasswordHashDTO { User = user, Hash = hash }, cancel)
           .ConfigureAwait(false);

        user.PasswordHash = await response
           .EnsureSuccessStatusCode()
           .Content
           .ReadAsStringAsync(cancel);
    }

    public async Task<string> GetPasswordHashAsync(User user, CancellationToken cancel)
    {
        var response = await PostAsync($"{Address}/getPasswordHash", user, cancel).ConfigureAwait(false);
        return await response
           .EnsureSuccessStatusCode()
           .Content
           .ReadAsStringAsync(cancel)
           .ConfigureAwait(false);
    }

    public async Task<bool> HasPasswordAsync(User user, CancellationToken cancel)
    {
        var response = await PostAsync($"{Address}/hasPassword", user, cancel).ConfigureAwait(false);
        return await response
           .EnsureSuccessStatusCode()
           .Content
           .ReadFromJsonAsync<bool>(cancellationToken: cancel)
           .ConfigureAwait(false);
    }

    #endregion

    #region Implementation of IUserEmailStore<User>

    public async Task SetEmailAsync(User user, string email, CancellationToken cancel)
    {
        var response = await PostAsync($"{Address}/setEmail/{email}", user, cancel).ConfigureAwait(false);
        user.Email = await response
           .EnsureSuccessStatusCode()
           .Content
           .ReadAsStringAsync(cancel)
           .ConfigureAwait(false);
    }

    public async Task<string> GetEmailAsync(User user, CancellationToken cancel)
    {
        var response = await PostAsync($"{Address}/getEmail", user, cancel).ConfigureAwait(false);
        return await response
           .EnsureSuccessStatusCode()
           .Content
           .ReadAsStringAsync(cancel)
           .ConfigureAwait(false);
    }

    public async Task<bool> GetEmailConfirmedAsync(User user, CancellationToken cancel)
    {
        var response = await PostAsync($"{Address}/getEmailConfirmed", user, cancel).ConfigureAwait(false);
        return await response
           .EnsureSuccessStatusCode()
           .Content
           .ReadFromJsonAsync<bool>(cancellationToken: cancel)
           .ConfigureAwait(false);
    }

    public async Task SetEmailConfirmedAsync(User user, bool confirmed, CancellationToken cancel)
    {
        var response = await PostAsync($"{Address}/setEmailConfirmed/{confirmed}", user, cancel).ConfigureAwait(false);
        user.EmailConfirmed = await response
           .EnsureSuccessStatusCode()
           .Content
           .ReadFromJsonAsync<bool>(cancellationToken: cancel)
           .ConfigureAwait(false);
    }

    public async Task<User> FindByEmailAsync(string email, CancellationToken cancel)
    {
        var user = await GetAsync<User>($"{Address}/findByEmail/{email}", cancel).ConfigureAwait(false);
        return user!;
    }

    public async Task<string> GetNormalizedEmailAsync(User user, CancellationToken cancel)
    {
        var response = await PostAsync($"{Address}/getNormalizedEmail", user, cancel);
        return await response
           .EnsureSuccessStatusCode()
           .Content
           .ReadAsStringAsync(cancel)
           .ConfigureAwait(false);
    }

    public async Task SetNormalizedEmailAsync(User user, string email, CancellationToken cancel)
    {
        var response = await PostAsync($"{Address}/setNormalizedEmail/{email}", user, cancel).ConfigureAwait(false);
        user.NormalizedEmail = await response
           .EnsureSuccessStatusCode()
           .Content
           .ReadAsStringAsync(cancel)
           .ConfigureAwait(false);
    }

    #endregion

    #region Implementation of IUserPhoneNumberStore<User>

    public async Task SetPhoneNumberAsync(User user, string phone, CancellationToken cancel)
    {
        var response = await PostAsync($"{Address}/setPhoneNumber/{phone}", user, cancel).ConfigureAwait(false);
        user.PhoneNumber = await response
           .EnsureSuccessStatusCode()
           .Content
           .ReadAsStringAsync(cancel)
           .ConfigureAwait(false);
    }

    public async Task<string> GetPhoneNumberAsync(User user, CancellationToken cancel)
    {
        var response = await PostAsync($"{Address}/getPhoneNumber", user, cancel).ConfigureAwait(false);
        return await response
           .EnsureSuccessStatusCode()
           .Content
           .ReadAsStringAsync(cancel)
           .ConfigureAwait(false);
    }

    public async Task<bool> GetPhoneNumberConfirmedAsync(User user, CancellationToken cancel)
    {
        var response = await PostAsync($"{Address}/getPhoneNumberConfirmed", user, cancel).ConfigureAwait(false);
        return await response
           .EnsureSuccessStatusCode()
           .Content
           .ReadFromJsonAsync<bool>(cancellationToken: cancel)
           .ConfigureAwait(false);
    }

    public async Task SetPhoneNumberConfirmedAsync(User user, bool confirmed, CancellationToken cancel)
    {
        var response = await PostAsync($"{Address}/setPhoneNumberConfirmed/{confirmed}", user, cancel).ConfigureAwait(false);
        user.PhoneNumberConfirmed = await response
           .EnsureSuccessStatusCode()
           .Content
           .ReadFromJsonAsync<bool>(cancellationToken: cancel)
           .ConfigureAwait(false);
    }

    #endregion

    #region Implementation of IUserLoginStore<User>

    public async Task AddLoginAsync(User user, UserLoginInfo login, CancellationToken cancel) =>
        await PostAsync($"{Address}/addLogin", new AddLoginDTO { User = user, UserLoginInfo = login }, cancel).ConfigureAwait(false);
    public async Task RemoveLoginAsync(User user, string loginProvider, string providerKey, CancellationToken cancel) =>
        await PostAsync($"{Address}/removeLogin/{loginProvider}/{providerKey}", user, cancel).ConfigureAwait(false);

    public async Task<IList<UserLoginInfo>> GetLoginsAsync(User user, CancellationToken cancel)
    {
        var response = await PostAsync($"{Address}/getLogins", user, cancel).ConfigureAwait(false);
        var logins = await response
           .EnsureSuccessStatusCode()
           .Content
           .ReadFromJsonAsync<List<UserLoginInfo>>(cancellationToken: cancel)
           .ConfigureAwait(false);
        return logins!;
    }

    public async Task<User> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancel)
    {
        var user = await GetAsync<User>($"{Address}/findByLogin/{loginProvider}/{providerKey}", cancel).ConfigureAwait(false);
        return user!;
    }

    #endregion

    #region Implementation of IUserLockoutStore<User>

    public async Task<DateTimeOffset?> GetLockoutEndDateAsync(User user, CancellationToken cancel)
    {
        var response = await PostAsync($"{Address}/getLockoutEndDate", user, cancel).ConfigureAwait(false);
        return await response
           .EnsureSuccessStatusCode()
           .Content
           .ReadFromJsonAsync<DateTimeOffset?>(cancellationToken: cancel)
           .ConfigureAwait(false);
    }

    public async Task SetLockoutEndDateAsync(User user, DateTimeOffset? EndDate, CancellationToken cancel)
    {
        var response = await PostAsync(
                $"{Address}/setLockoutEndDate",
                new SetLockoutDTO { User = user, LockoutEnd = EndDate },
                cancel)
           .ConfigureAwait(false);

        user.LockoutEnd = await response
           .EnsureSuccessStatusCode()
           .Content
           .ReadFromJsonAsync<DateTimeOffset?>(cancellationToken: cancel)
           .ConfigureAwait(false);
    }

    public async Task<int> IncrementAccessFailedCountAsync(User user, CancellationToken cancel)
    {
        var response = await PostAsync($"{Address}/incrementAccessFailedCount", user, cancel).ConfigureAwait(false);
        return await response
           .EnsureSuccessStatusCode()
           .Content
           .ReadFromJsonAsync<int>(cancellationToken: cancel)
           .ConfigureAwait(false);
    }

    public async Task ResetAccessFailedCountAsync(User user, CancellationToken cancel)
    {
        await PostAsync($"{Address}/resetAccessFailedCont", user, cancel).ConfigureAwait(false);
    }

    public async Task<int> GetAccessFailedCountAsync(User user, CancellationToken cancel)
    {
        var response = await PostAsync($"{Address}/getAccessFailedCount", user, cancel).ConfigureAwait(false);
        return await response
           .EnsureSuccessStatusCode()
           .Content
           .ReadFromJsonAsync<int>(cancellationToken: cancel)
           .ConfigureAwait(false);
    }

    public async Task<bool> GetLockoutEnabledAsync(User user, CancellationToken cancel)
    {
        var response = await PostAsync($"{Address}/getLockoutEnabled", user, cancel).ConfigureAwait(false);
        return await response
           .EnsureSuccessStatusCode()
           .Content
           .ReadFromJsonAsync<bool>(cancellationToken: cancel)
           .ConfigureAwait(false);
    }

    public async Task SetLockoutEnabledAsync(User user, bool enabled, CancellationToken cancel)
    {
        var response = await PostAsync($"{Address}/setLockoutEnabled/{enabled}", user, cancel).ConfigureAwait(false);
        user.LockoutEnabled = await response
           .EnsureSuccessStatusCode()
           .Content
           .ReadFromJsonAsync<bool>(cancellationToken: cancel)
           .ConfigureAwait(false);
    }

    #endregion

    #region Implementation of IUserTwoFactorStore<User>

    public async Task SetTwoFactorEnabledAsync(User user, bool enabled, CancellationToken cancel)
    {
        var response = await PostAsync($"{Address}/setTwoFactor/{enabled}", user, cancel).ConfigureAwait(false);
        user.TwoFactorEnabled = await response
           .EnsureSuccessStatusCode()
           .Content
           .ReadFromJsonAsync<bool>(cancellationToken: cancel)
           .ConfigureAwait(false);
    }

    public async Task<bool> GetTwoFactorEnabledAsync(User user, CancellationToken cancel)
    {
        var response = await PostAsync($"{Address}/getTwoFactorEnabled", user, cancel).ConfigureAwait(false);
        return await response
           .EnsureSuccessStatusCode()
           .Content
           .ReadFromJsonAsync<bool>(cancellationToken: cancel)
           .ConfigureAwait(false);
    }

    #endregion

    #region Implementation of IUserClaimStore<User>

    public async Task<IList<Claim>> GetClaimsAsync(User user, CancellationToken cancel)
    {
        var response = await PostAsync($"{Address}/getClaims", user, cancel).ConfigureAwait(false);
        var claims = await response
           .EnsureSuccessStatusCode()
           .Content
           .ReadFromJsonAsync<List<Claim>>(cancellationToken: cancel)
           .ConfigureAwait(false);
        return claims!;
    }

    public async Task AddClaimsAsync(User user, IEnumerable<Claim> claims, CancellationToken cancel)
    {
        var response = await PostAsync(
                $"{Address}/addClaims",
                new ClaimDTO { User = user, Claims = claims },
                cancel)
           .ConfigureAwait(false);

        response.EnsureSuccessStatusCode();
    }

    public async Task ReplaceClaimAsync(User user, Claim OldClaim, Claim NewClaim, CancellationToken cancel)
    {
        var response = await PostAsync(
                $"{Address}/replaceClaim",
                new ReplaceClaimDTO { User = user, Claim = OldClaim, NewClaim = NewClaim },
                cancel)
           .ConfigureAwait(false);

        response.EnsureSuccessStatusCode();
    }

    public async Task RemoveClaimsAsync(User user, IEnumerable<Claim> claims, CancellationToken cancel)
    {
        var response = await PostAsync(
                $"{Address}/removeClaims",
                new ClaimDTO { User = user, Claims = claims },
                cancel)
           .ConfigureAwait(false);

        response.EnsureSuccessStatusCode();
    }

    public async Task<IList<User>> GetUsersForClaimAsync(Claim claim, CancellationToken cancel)
    {
        var response = await PostAsync($"{Address}/getUsersForClaim", claim, cancel).ConfigureAwait(false);
        var users = await response
           .EnsureSuccessStatusCode()
           .Content
           .ReadFromJsonAsync<List<User>>(cancellationToken: cancel)
           .ConfigureAwait(false);
        return users!;
    }

    #endregion
}
