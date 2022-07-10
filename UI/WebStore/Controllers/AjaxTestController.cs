using Microsoft.AspNetCore.Mvc;
using WebStore.Domain.ViewModels;

namespace WebStore.Controllers;

public class AjaxTestController : Controller
{
    private readonly ILogger<AjaxTestController> _logger;

    public AjaxTestController(ILogger<AjaxTestController> logger) => _logger = logger;

    public IActionResult Index() => View();

    public async Task<IActionResult> GetJSON(int? id, string? msg, int delay = 2000)
    {
        _logger.LogInformation($"Получен запрос к GetJSON - id:{id}, msg:{msg}, Delay:{delay}");

        await Task.Delay(delay);

        _logger.LogInformation($"Ответ на запрос к GetJSON - id:{id}, msg:{msg}, Delay:{delay}");

        return Json(new
        {
            Message = $"Response (id:{id ?? -1}: {msg ?? "--null--"})",
            ServerTime = DateTime.Now,
        });
    }

    public async Task<IActionResult> GetHTML(int? id, string? msg, int delay = 2000)
    {
        _logger.LogInformation($"Получен запрос к GetHTML - id:{id}, msg:{msg}, Delay:{delay}");

        await Task.Delay(delay);

        _logger.LogInformation($"Ответ на запрос к GetHTML - id:{id}, msg:{msg}, Delay:{delay}");

        return PartialView("Partial/_DataView", new AjaxTestDataViewModel
        {
            Id = id ?? -1,
            Message = msg,
        });
    }
}