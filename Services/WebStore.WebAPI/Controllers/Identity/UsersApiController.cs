using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebStore.DAL.Context;
using WebStore.Domain.DTO.Identity;
using WebStore.Domain.Entities.Identity;
using WebStore.Interfaces;

namespace WebStore.WebAPI.Controllers.Identity;

[ApiController]
[Route(WebApiAdresses.V1.Identity.Users)]
public class UsersApiController : ControllerBase
{
    private readonly ILogger<UsersApiController> _logger;
    private readonly UserStore<User,Role, WebStoreDB> _userStore;

    public UsersApiController(WebStoreDB webStoreDB, ILogger<UsersApiController> logger)
    {
        _logger = logger;
        _userStore = new (webStoreDB);
    }

    [HttpGet("all")]
    public async Task<IEnumerable<User>> GetAll() => await _userStore.Users.ToArrayAsync();

    #region Users
    [HttpPost("userId")]
    public async Task<string> GetUserIdAsync([FromBody] User user) => await _userStore.GetUserIdAsync(user);

    [HttpPost("userName")]
    public async Task<string> GetUserNameAsync([FromBody] User user) => await _userStore.GetUserNameAsync(user);

    [HttpPost("userName/{name}")]
    public async Task<string> SetUserNameAsync([FromBody] User user, string name)
    {
        await _userStore.SetUserNameAsync(user, name);
        await _userStore.UpdateAsync(user);
        return user.UserName;
    }

    [HttpPost("normalUserName")]
    public async Task<string> GetNormalizedUserNameAsync([FromBody] User user) => await _userStore.GetNormalizedUserNameAsync(user);

    [HttpPost("normalUserName/{name}")]
    public async Task<string> SetNormalizedUserNameAsync([FromBody] User user, string name)
    {
        await _userStore.SetNormalizedUserNameAsync(user, name);
        await _userStore.UpdateAsync(user);
        return user.NormalizedUserName;
    }

    [HttpPost]
    public async Task<bool> CreateAsync([FromBody] User user)
    {
        var createResult = await _userStore.CreateAsync(user);

        if (!createResult.Succeeded)
        {
            var errorString = string.Join(", ", createResult.Errors.Select(e => e.Description));
            _logger.LogWarning($"Ошибка создания пользователя {user}:{errorString}");
        }
        
        return createResult.Succeeded;
    }

    [HttpPut]
    public async Task<bool> UpdateAsync([FromBody] User user)
    {
        var updateResult = await _userStore.UpdateAsync(user);

        if (!updateResult.Succeeded)
        {
            var errorString = string.Join(", ", updateResult.Errors.Select(e => e.Description));
            _logger.LogWarning($"Ошибка редактирования пользователя {user}:{errorString}");
        }

        return updateResult.Succeeded;
    }

    [HttpPost("user/delete")]
    [HttpDelete("user/delete")]
    [HttpDelete]
    public async Task<bool> DeleteAsync([FromBody] User user)
    {
        var deleteResult = await _userStore.DeleteAsync(user);

        if (!deleteResult.Succeeded)
        {
            var errorString = string.Join(", ", deleteResult.Errors.Select(e => e.Description));
            _logger.LogWarning($"Ошибка редактирования пользователя {user}:{errorString}");
        }

        return deleteResult.Succeeded;
    }

    [HttpGet("find/{id}")]
    public async Task<User> FindByIdAsync(string id) => await _userStore.FindByIdAsync(id);

    [HttpGet("normal/{name}")]
    public async Task<User> FindByNameAsync(string name) => await _userStore.FindByNameAsync(name);

    [HttpPost("role/{role}")]
    public async Task AddToRoleAsync([FromBody] User user, string role)
    {
        await _userStore.AddToRoleAsync(user, role);
        await _userStore.Context.SaveChangesAsync();
    }

    [HttpDelete("role/{role}")]
    [HttpPost("role/delete/{role}")]
    public async Task RemoveFromRoleAsync([FromBody] User user, string role)
    {
        await _userStore.RemoveFromRoleAsync(user, role);
        await _userStore.Context.SaveChangesAsync();
    }

    [HttpPost("roles")]
    public async Task<IList<string>> GetRolesAsync([FromBody] User user) => await _userStore.GetRolesAsync(user);

    [HttpPost("inRole/{role}")]
    public async Task<bool> IsInRoleAsync([FromBody] User user, string role) => await _userStore.IsInRoleAsync(user, role);

    [HttpGet("usersInRole/{role}")]
    public async Task<IList<User>> GetUsersInRoleAsync(string role) => await _userStore.GetUsersInRoleAsync(role);

    [HttpPost("getPasswordHash")]
    public async Task<string> GetPasswordHashAsync([FromBody] User user) => await _userStore.GetPasswordHashAsync(user);

    [HttpPost("setPasswordHash")]
    public async Task<string> SetPasswordHashAsync([FromBody] PasswordHashDTO hash)
    {
        await _userStore.SetPasswordHashAsync(hash.User, hash.Hash);
        await _userStore.UpdateAsync(hash.User);
        return hash.User.PasswordHash;
    }

    [HttpPost("hasPassword")]
    public async Task<bool> HasPasswordAsync([FromBody] User user) => await _userStore.HasPasswordAsync(user);

    #endregion

    #region Claims

    [HttpPost("getClaims")]
    public async Task<IList<Claim>> GetClaimsAsync([FromBody] User user) => await _userStore.GetClaimsAsync(user);

    [HttpPost("addClaims")]
    public async Task AddClaimsAsync([FromBody] ClaimDTO claimInfo)
    {
        await _userStore.AddClaimsAsync(claimInfo.User, claimInfo.Claims);
        await _userStore.Context.SaveChangesAsync();
    }

    [HttpPost("replaceClaim")]
    public async Task ReplaceClaimAsync([FromBody] ReplaceClaimDTO claimInfo)
    {
        await _userStore.ReplaceClaimAsync(claimInfo.User, claimInfo.Claim, claimInfo.NewClaim);
        await _userStore.Context.SaveChangesAsync();
    }

    [HttpPost("removeClaim")]
    public async Task RemoveClaimsAsync([FromBody] ClaimDTO claimInfo)
    {
        await _userStore.RemoveClaimsAsync(claimInfo.User, claimInfo.Claims);
        await _userStore.Context.SaveChangesAsync();
    }

    [HttpPost("getUsersForClaim")]
    public async Task<IList<User>> GetUsersForClaimAsync([FromBody] Claim claim) =>
        await _userStore.GetUsersForClaimAsync(claim);

    #endregion

    #region TwoFactor

    [HttpPost("getTwoFactorEnabled")]
    public async Task<bool> GetTwoFactorEnabledAsync([FromBody] User user) => await _userStore.GetTwoFactorEnabledAsync(user);

    [HttpPost("setTwoFactor/{enable}")]
    public async Task<bool> SetTwoFactorEnabledAsync([FromBody] User user, bool enable)
    {
        await _userStore.SetTwoFactorEnabledAsync(user, enable);
        await _userStore.UpdateAsync(user);
        return user.TwoFactorEnabled;
    }

    #endregion

    #region Email/Phone

    [HttpPost("getEmail")]
    public async Task<string> GetEmailAsync([FromBody] User user) => await _userStore.GetEmailAsync(user);

    [HttpPost("setEmail/{email}")]
    public async Task<string> SetEmailAsync([FromBody] User user, string email)
    {
        await _userStore.SetEmailAsync(user, email);
        await _userStore.UpdateAsync(user);
        return user.Email;
    }

    [HttpPost("getNormalizedEmail")]
    public async Task<string> GetNormalizedEmailAsync([FromBody] User user) => await _userStore.GetNormalizedEmailAsync(user);

    [HttpPost("setNormalizedEmail/{email?}")]
    public async Task<string> SetNormalizedEmailAsync([FromBody] User user, string? email)
    {
        await _userStore.SetNormalizedEmailAsync(user, email);
        await _userStore.UpdateAsync(user);
        return user.NormalizedEmail;
    }

    [HttpPost("getEmailConfirmed")]
    public async Task<bool> GetEmailConfirmedAsync([FromBody] User user) => await _userStore.GetEmailConfirmedAsync(user);

    [HttpPost("getEmailConfirmed/{enable}")]
    public async Task<bool> SetEmailConfirmedAsync([FromBody] User user, bool enable)
    {
        await _userStore.SetEmailConfirmedAsync(user, enable);
        await _userStore.UpdateAsync(user);
        return user.EmailConfirmed;
    }

    [HttpGet("userFindByEmail/{email}")]
    public async Task<User> FindByEmailAsync(string email) => await _userStore.FindByEmailAsync(email);

    [HttpPost("getPhoneNumber")]
    public async Task<string> GetPhoneNumberAsync([FromBody] User user) => await _userStore.GetPhoneNumberAsync(user);

    [HttpPost("setPhoneNumber/{phone}")]
    public async Task<string> SetPhoneNumberAsync([FromBody] User user, string phone)
    {
        await _userStore.SetPhoneNumberAsync(user, phone);
        await _userStore.UpdateAsync(user);
        return user.PhoneNumber;
    }

    [HttpPost("getPhoneNumberConfirmed")]
    public async Task<bool> GetPhoneNumberConfirmedAsync([FromBody] User user) =>
        await _userStore.GetPhoneNumberConfirmedAsync(user);

    [HttpPost("setPhoneNumberConfirmed/{confirmed}")]
    public async Task<bool> SetPhoneNumberConfirmedAsync([FromBody] User user, bool confirmed)
    {
        await _userStore.SetPhoneNumberConfirmedAsync(user, confirmed);
        await _userStore.UpdateAsync(user);
        return user.PhoneNumberConfirmed;
    }

    #endregion

    #region Login/Lockout

    [HttpPost("addLogin")]
    public async Task AddLoginAsync([FromBody] AddLoginDTO login)
    {
        await _userStore.AddLoginAsync(login.User, login.UserLoginInfo);
        await _userStore.Context.SaveChangesAsync();
    }

    [HttpPost("removeLogin/{loginProvider}/{providerKey}")]
    public async Task RemoveLoginAsync([FromBody] User user, string loginProvider, string providerKey)
    {
        await _userStore.RemoveLoginAsync(user, loginProvider, providerKey);
        await _userStore.Context.SaveChangesAsync();
    }

    [HttpPost("getLogins")]
    public async Task<IList<UserLoginInfo>> GetLoginsAsync([FromBody] User user) => 
        await _userStore.GetLoginsAsync(user);

    [HttpGet("findByLogin/{loginProvider}/{providerKey}")]
    public async Task<User> FindByLoginAsync(string loginProvider, string providerKey) => 
        await _userStore.FindByLoginAsync(loginProvider, providerKey);

    [HttpPost("getLockoutEndDate")]
    public async Task<DateTimeOffset?> GetLockoutEndDateAsync([FromBody] User user) =>
        await _userStore.GetLockoutEndDateAsync(user);

    [HttpPost("SetLockoutEndDate")]
    public async Task<DateTimeOffset?> SetLockoutEndDateAsync([FromBody] SetLockoutDTO lockoutInfo)
    {
        await _userStore.SetLockoutEndDateAsync(lockoutInfo.User, lockoutInfo.LockoutEnd);
        await _userStore.UpdateAsync(lockoutInfo.User);
        return lockoutInfo.User.LockoutEnd;
    }

    [HttpPost("incrementAccessFailedCount")]
    public async Task<int> IncrementAccessFailedCountAsync([FromBody] User user)
    {
        var count = await _userStore.IncrementAccessFailedCountAsync(user);
        await _userStore.UpdateAsync(user);
        return count;
    }

    [HttpPost("resetAccessFailedCount")]
    public async Task<int> ResetAccessFailedCountAsync([FromBody] User user)
    {
        await _userStore.ResetAccessFailedCountAsync(user);
        await _userStore.UpdateAsync(user);
        return user.AccessFailedCount;
    }

    [HttpPost("getAccessFailedCount")]
    public async Task<int> GetAccessFailedCountAsync([FromBody] User user) => await _userStore.GetAccessFailedCountAsync(user);

    [HttpPost("getLockoutEnabled")]
    public async Task<bool> GetLockoutEnabledAsync([FromBody] User user) => await _userStore.GetLockoutEnabledAsync(user);

    [HttpPost("setLockoutEnabled/{enable}")]
    public async Task<bool> SetLockoutEnabledAsync([FromBody] User user, bool enable)
    {
        await _userStore.SetLockoutEnabledAsync(user, enable);
        await _userStore.UpdateAsync(user);
        return user.LockoutEnabled;
    }

    #endregion
}

