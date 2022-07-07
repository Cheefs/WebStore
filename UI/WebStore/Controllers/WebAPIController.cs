using Microsoft.AspNetCore.Mvc;
using WebStore.Interfaces.TestApi;

namespace WebStore.Controllers;

public class WebAPIController : Controller
{
    private readonly IValuesService _valuesService;

    public WebAPIController(IValuesService valuesService) => _valuesService = valuesService;

    public IActionResult Index()
    {
        return View(_valuesService.GetValues());
    }
}
