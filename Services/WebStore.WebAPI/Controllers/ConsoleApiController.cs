using Microsoft.AspNetCore.Mvc;
using WebStore.Interfaces;

namespace WebStore.WebAPI.Controllers;

[ApiController]
[Route(WebApiAdresses.V1.Console)]
public class ConsoleApiController : ControllerBase
{
    [HttpGet("clear")]
    public void Clear() => Console.Clear();

    [HttpGet("write/{str}")]
    public void Write(string str) => Console.WriteLine(str);
}
