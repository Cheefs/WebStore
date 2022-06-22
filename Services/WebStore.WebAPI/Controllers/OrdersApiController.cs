using Microsoft.AspNetCore.Mvc;
using WebStore.Domain.DTO;
using WebStore.Interfaces;
using WebStore.Interfaces.Services;

namespace WebStore.WebAPI.Controllers;

[ApiController]
[Route(WebApiAdresses.V1.Orders)]
public class OrdersApiController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly ILogger<OrdersApiController> _logger;

    public OrdersApiController(IOrderService orderService, ILogger<OrdersApiController> logger)
    {
        this._orderService = orderService;
        this._logger = logger;
    }

    [HttpGet("user/{username}")]
    public async Task<IActionResult> GetUserOrders(string username)
    {
        var result = await _orderService.GetUserOrdersAsync(username);
        if (result.Any())
        {
            return Ok(result.ToDTO());
        }
        return NoContent();
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetOrderById(int id)
    {
        var result = await _orderService.GetOrderByIdAsync(id);
        if (result is null)
        {
            return NotFound();
        }
        return Ok(result.ToDTO());
    }
    [HttpPost("user/{username}")]
    public async Task<IActionResult> CreateOrder(string username, [FromBody] CreateOrderDTO model)
    {
        var cart = model.Items.ToCartView();
        var orderModel = model.Order;

        var order = await _orderService.CreateOrderAsync(username, cart, orderModel);

       return CreatedAtAction(nameof(GetOrderById), new { order.Id }, order.ToDTO());
    }
}
