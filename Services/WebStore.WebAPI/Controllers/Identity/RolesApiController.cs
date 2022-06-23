using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebStore.DAL.Context;
using WebStore.Domain.Entities.Identity;
using WebStore.Interfaces;

namespace WebStore.WebAPI.Controllers.Identity;

[ApiController]
[Route(WebApiAdresses.V1.Identity.Roles)]
public class RolesApiController : ControllerBase
{
    private readonly RoleStore<Role> _rolesStore;
    private readonly ILogger<RolesApiController> _logger;

    public RolesApiController(WebStoreDB webStoreDb, ILogger<RolesApiController> logger)
    {
        _logger = logger;
        _rolesStore = new(webStoreDb);
    }

    [HttpGet("all")]
    public async Task<IEnumerable<Role>> GetAll() => await _rolesStore.Roles.ToArrayAsync();

    /* ------------------------------------------------------------------------------------ */

    [HttpPost]
    public async Task<bool> CreateAsync(Role role)
    {
        var createResult = await _rolesStore.CreateAsync(role);

        if (!createResult.Succeeded)
        {
            var errorMessage = string.Join(", ", createResult.Errors.Select(e => e.Description));
            _logger.LogWarning($"Ошибка создания роли {role}:{errorMessage}");
        }

        return createResult.Succeeded;
    }

    [HttpPut]
    public async Task<bool> UpdateAsync(Role role)
    {
        var updateResult = await _rolesStore.UpdateAsync(role);

        if (!updateResult.Succeeded)
        {
            var errorMessage = string.Join(", ", updateResult.Errors.Select(e => e.Description));
            _logger.LogWarning($"Ошибка изменения роли {role}:{errorMessage}");
        }
        
        return updateResult.Succeeded;
    }

    [HttpDelete]
    [HttpPost("delete")]
    public async Task<bool> DeleteAsync(Role role)
    {
        var deleteResult = await _rolesStore.DeleteAsync(role);

        if (!deleteResult.Succeeded)
        {
            var errorMessage = string.Join(", ", deleteResult.Errors.Select(e => e.Description));
            _logger.LogWarning($"Ошибка удаления роли {role}:{errorMessage}");
        };

        return deleteResult.Succeeded;
    }

    [HttpPost("getRoleId")]
    public async Task<string> GetRoleIdAsync([FromBody] Role role) => await _rolesStore.GetRoleIdAsync(role);

    [HttpPost("getRoleName")]
    public async Task<string> GetRoleNameAsync([FromBody] Role role) => await _rolesStore.GetRoleNameAsync(role);

    [HttpPost("setRoleName/{name}")]
    public async Task<string> SetRoleNameAsync(Role role, string name)
    {
        await _rolesStore.SetRoleNameAsync(role, name);
        await _rolesStore.UpdateAsync(role);
        return role.Name;
    }

    [HttpPost("getNormalizedRoleName")]
    public async Task<string> GetNormalizedRoleNameAsync(Role role) => await _rolesStore.GetNormalizedRoleNameAsync(role);

    [HttpPost("setNormalizedRoleName/{name}")]
    public async Task<string> SetNormalizedRoleNameAsync(Role role, string name)
    {
        await _rolesStore.SetNormalizedRoleNameAsync(role, name);
        await _rolesStore.UpdateAsync(role);
        return role.NormalizedName;
    }

    [HttpGet("findById/{id}")]
    public async Task<Role> FindByIdAsync(string id) => await _rolesStore.FindByIdAsync(id);

    [HttpGet("findByName/{name}")]
    public async Task<Role> FindByNameAsync(string name) => await _rolesStore.FindByNameAsync(name);
}
