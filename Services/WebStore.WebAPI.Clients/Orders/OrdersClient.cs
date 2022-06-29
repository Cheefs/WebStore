using System.Net.Http.Json;
using WebStore.Domain.DTO;
using WebStore.Domain.Entities.Orders;
using WebStore.Domain.ViewModels;
using WebStore.Interfaces;
using WebStore.Interfaces.Services;
using WebStore.WebAPI.Clients.Base;

namespace WebStore.WebAPI.Clients.Orders;

public class OrdersClient : BaseClient, IOrderService
{
    public OrdersClient(HttpClient client) : base(client, WebApiAdresses.V1.Orders) { }

    public async Task<Order> CreateOrderAsync(string username, CartViewModel cart, OrderViewModel orderModel, CancellationToken cancel = default)
    {
        var model = new CreateOrderDTO
        {
            Items = cart.ToDTO(),
            Order = orderModel
        };

        var response = await PostAsync($"{Address}/user/{username}", model, cancel).ConfigureAwait(false);

        var result = await response.EnsureSuccessStatusCode()
            .Content
            .ReadFromJsonAsync<OrderDTO>(cancellationToken: cancel);

        return result?.FromDTO()!;
    }

    public async Task<Order?> GetOrderByIdAsync(int id, CancellationToken cancel = default)
    {
        var order = await GetAsync<OrderDTO>($"{Address}/{id}", cancel).ConfigureAwait(false);
        return order?.FromDTO();
    }

    public async Task<IEnumerable<Order>> GetUserOrdersAsync(string username, CancellationToken cancel = default)
    {
        var orders = await GetAsync<IEnumerable<OrderDTO>>($"{Address}/user/{username}", cancel).ConfigureAwait(false);
        return (orders?.FromDTO() ?? Enumerable.Empty<Order>())!;
    }
}
