using WebStore.Domain.ViewModels;
using WebStore.Interfaces.Services;
using WebStore.Services.Mapping;

namespace WebStore.Services.Services;

public class CartService : ICartService
{
    private readonly IProductData _productData;
    private readonly ICartStore _cartStore;

    public CartService(IProductData productData, ICartStore cartStore)
    {
        _productData = productData;
        _cartStore = cartStore;
    }

    public void Add(int Id)
    {
        var cart = _cartStore.Cart;
        cart.Add(Id);
        _cartStore.Cart = cart;
    }

    public void Decrement(int Id)
    {
        var cart = _cartStore.Cart;
        cart.Decrement(Id);
        _cartStore.Cart = cart;
    }

    public void Remove(int Id)
    {
        var cart = _cartStore.Cart;
        cart.Remove(Id);
        _cartStore.Cart = cart;
    }

    public void Clear()
    {
        var cart = _cartStore.Cart;
        cart.Clear();
        _cartStore.Cart = cart;
    }

    public CartViewModel GetViewModel()
    {
        var cart = _cartStore.Cart;

        var products = _productData.GetProducts(new()
        {
            Ids = cart.Items.Select(item => item.ProductId).ToArray(),
        });

        var productsViews = products.Items.ToView().ToDictionary(p => p!.Id);

        return new()
        {
            Items = cart.Items
               .Where(item => productsViews.ContainsKey(item.ProductId))
               .Select(item => (productsViews[item.ProductId], item.Quantity))!,
        };
    }
}