using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebStore.DAL.Context;
using WebStore.Domain.Entities.Identity;

namespace WebStore.WebAPI.Controllers.Identity;

[ApiController]
[Route("api/roles")]
public class RolesApiController : ControllerBase
{
    private readonly RoleStore<Role> _rolesStore;
    private readonly ILogger<RolesApiController> _logger;

    public RolesApiController(WebStoreDB webStoreDb, ILogger<RolesApiController> logger)
    {
        _logger = logger;
        _rolesStore = new(webStoreDb);
    }

    [HttpGet]
    public async Task<IEnumerable<Role>> GetAll() => await _rolesStore.Roles.ToArrayAsync();
}
