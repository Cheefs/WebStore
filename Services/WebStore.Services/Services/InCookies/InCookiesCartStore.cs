using Microsoft.AspNetCore.Http;
using System.Text.Json;
using WebStore.Domain.Entities;
using WebStore.Interfaces.Services;

namespace WebStore.Services.Services.InCookies;

public class InCookiesCartStore: ICartStore
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly string _cartName;

    public Cart Cart
    {
        get
        {
            var context = _httpContextAccessor.HttpContext!;
            var cookies = context.Response.Cookies;

            var cartCookie = context.Request.Cookies[_cartName];
            if (cartCookie is null)
            {
                var cart = new Cart();
                cookies.Append(_cartName, JsonSerializer.Serialize(cart));
                return cart;
            }

            ReplaceCart(cookies, cartCookie);
            return JsonSerializer.Deserialize<Cart>(cartCookie)!;
        }
        set => ReplaceCart(_httpContextAccessor.HttpContext!.Response.Cookies, JsonSerializer.Serialize(value));
    }

    private void ReplaceCart(IResponseCookies cookies, string cart)
    {
        cookies.Delete(_cartName);
        cookies.Append(_cartName, cart);
    }

    public InCookiesCartStore(IHttpContextAccessor HttpContextAccessor)
    {
        _httpContextAccessor = HttpContextAccessor;

        var user = HttpContextAccessor.HttpContext!.User;
        var userName = user.Identity!.IsAuthenticated ? $"-{user.Identity.Name}" : null;

        _cartName = $"WebStore.GB.Cart{userName}";
    }
}
