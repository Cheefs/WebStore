using Microsoft.AspNetCore.Mvc;

namespace WebStore.Controllers;

[ApiController]
[Route("console")]
public class ConsoleController : ControllerBase
{
    [HttpGet("clear")]
    public void Clear() => Console.Clear();

    [HttpGet("write/{str}")]
    public void Write(string str) => Console.WriteLine(str);
}
