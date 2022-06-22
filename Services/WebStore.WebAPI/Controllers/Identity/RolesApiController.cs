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
}
