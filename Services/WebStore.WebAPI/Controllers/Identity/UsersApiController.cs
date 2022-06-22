using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebStore.DAL.Context;
using WebStore.Domain.Entities.Identity;

namespace WebStore.WebAPI.Controllers.Identity;

[ApiController]
[Route("api/users")]
public class UsersApiController : ControllerBase
{
    private readonly ILogger<UsersApiController> _logger;
    private readonly UserStore<User,Role, WebStoreDB> _userStore;

    public UsersApiController(WebStoreDB webStoreDB, ILogger<UsersApiController> logger)
    {
        _logger = logger;
        _userStore = new (webStoreDB);
    }

    [HttpGet]
    public async Task<IEnumerable<User>> GetAll() => await _userStore.Users.ToArrayAsync();
}

