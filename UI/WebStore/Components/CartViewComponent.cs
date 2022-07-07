using Microsoft.AspNetCore.Mvc;
using WebStore.Interfaces.Services;

namespace WebStore.Components;

public class CartViewComponent: ViewComponent
{
    private readonly ICartStore _cartStore;

    public CartViewComponent(ICartStore cartStore) => _cartStore = cartStore;

    public IViewComponentResult Invoke()
    {
        ViewBag.Count = _cartStore.Cart.ItemsCount;
        return View();
    }
}
