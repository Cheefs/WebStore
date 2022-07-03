using Moq;
using System.Collections.Generic;
using System.Linq;
using WebStore.Domain;
using WebStore.Domain.Entities;
using WebStore.Domain.ViewModels;
using WebStore.Interfaces.Services;
using WebStore.Services.Services;
using Assert = Xunit.Assert;
namespace WebStore.Servises.Tests.Services;

[TestClass]
public class CartServiceTests
{
    private Cart? _cart;
    private Mock<IProductData>? _productDataMock;
    private Mock<ICartStore>? _cartStoreMock;

    private ICartService _cartService = null!;

    [TestInitialize]
    public void Initialize()
    {
        _cart = new Cart
        {
            Items = new List<CartItem>
            {
                new CartItem() { ProductId = 1, Quantity = 1 },
                new CartItem() { ProductId = 2, Quantity = 3 },
            }
        };

        _productDataMock = new Mock<IProductData>();
        _productDataMock
            .Setup(s => s.GetProducts(It.IsAny<ProductFilter>()))
            .Returns(new[]
            {
                new Product
                {
                    Id = 1,
                    Name = "Product 1",
                    Price = 1.1m,
                    Order = 1,
                    ImageUrl = "img_1.png",
                    Brand = new Brand { Id = 1, Name = "Brand 1", Order = 1},
                    SectionId = 1,
                    Section = new Section{ Id = 1, Name = "Section 1", Order = 1 },
                },
                new Product
                {
                    Id = 2,
                    Name = "Product 2",
                    Price = 2.2m,
                    Order = 2,
                    ImageUrl = "img_2.png",
                    Brand = new Brand { Id = 2, Name = "Brand 2", Order = 2},
                    SectionId = 2,
                    Section = new Section{ Id = 2, Name = "Section 2", Order = 2 },
                },
                new Product
                {
                    Id = 3,
                    Name = "Product 3",
                    Price = 3.3m,
                    Order = 3,
                    ImageUrl = "img_3.png",
                    Brand = new Brand { Id = 3, Name = "Brand 3", Order = 3},
                    SectionId = 3,
                    Section = new Section{ Id = 3, Name = "Section 3", Order = 3 },
                },
            });

        _cartStoreMock = new Mock<ICartStore>();
        _cartStoreMock.Setup(c => c.Cart).Returns(_cart);

        _cartService = new CartService(_productDataMock.Object, _cartStoreMock.Object);
    }

    [TestMethod, Description("Тест модели корзины")]
    public void Cart_Class_ItemsCount_returns_Correct_Quantity()
    {
        var expectedCount = _cart?.Items.Sum(i => i.Quantity);
        var actualCount = _cart?.ItemsCount;

        Assert.Equal(expectedCount, actualCount);
    }

    [TestMethod]
    public void CartViewModel_Returns_Correct_ItemsCount()
    {
        var cartViewModel = new CartViewModel
        {
            Items = new[]
            {
                ( new ProductViewModel { Id = 1, Name = "Product 1", Price = 0.5m }, 1 ),
                ( new ProductViewModel { Id = 2, Name = "Product 2", Price = 1.5m }, 3 ),
            }
        };

        var expectedCount = cartViewModel.Items.Sum(i => i.Quantity);

        var actualCount = cartViewModel.ItemsCount;

        Assert.Equal(expectedCount, actualCount);
    }

    [TestMethod]
    public void CartViewModel_Returns_Correct_TotalPrice()
    {
        var cartViewModel = new CartViewModel
        {
            Items = new[]
            {
                ( new ProductViewModel { Id = 1, Name = "Product 1", Price = 0.5m }, 1 ),
                ( new ProductViewModel { Id = 2, Name = "Product 2", Price = 1.5m }, 3 ),
            }
        };

        var expectedTotalPrice = cartViewModel.Items.Sum(item => item.Quantity * item.Product.Price);

        var actualTotalPrice = cartViewModel.TotalPrice;

        Assert.Equal(expectedTotalPrice, actualTotalPrice);
    }

    [TestMethod]
    public void CartService_Add_WorkCorrect()
    {
        _cart?.Items.Clear();

        const int ID = 5;
        const int COUNT = 1;

        _cartService.Add(ID);

        var actual_items_count = _cart?.ItemsCount;

        Assert.Equal(COUNT, actual_items_count);

        Assert.Single(_cart?.Items);

        Assert.Equal(ID, _cart?.Items.Single().ProductId);
    }

    [TestMethod]
    public void CartService_Remove_Correct_Item()
    {
        const int ID = 1;
        const int PRODUCT_ID = 2;

        _cartService.Remove(ID);

        Assert.Single(_cart?.Items);

        Assert.Equal(PRODUCT_ID, _cart?.Items.Single().ProductId);
    }

    [TestMethod]
    public void CartService_Clear_ClearCart()
    {
        _cartService.Clear();

        Assert.Empty(_cart.Items);
    }

    [TestMethod]
    public void CartService_Decrement_Correct()
    {
        const int ID = 2;

        const int QUANTITY = 2;
        const int ITEMS_COUNT = 3;
        const int PRODUCTS_COUNT = 2;

        _cartService.Decrement(ID);

        Assert.Equal(ITEMS_COUNT, _cart?.ItemsCount);
        Assert.Equal(PRODUCTS_COUNT, _cart?.Items.Count);

        var items = _cart?.Items.ToArray();
        Assert.Equal(ID, items?[1].ProductId);
        Assert.Equal(QUANTITY, items?[1].Quantity);
    }

    [TestMethod]
    public void CartService_Remove_Item_When_Decrement_to_0()
    {
        const int ID = 1;
        const int COUNT = 3;

        _cartService.Decrement(ID);

        Assert.Equal(COUNT, _cart?.ItemsCount);
        Assert.Single(_cart?.Items);
    }

    [TestMethod]
    public void CartService_GetViewModel_WorkCorrect()
    {
        const int COUNT = 4;
        const decimal PRICE = 1.1m;

        var result = _cartService.GetViewModel();

        Assert.Equal(COUNT, result.ItemsCount);

        Assert.Equal(PRICE, result.Items.First().Product.Price);

        _productDataMock?.Verify(s => s.GetProducts(It.IsAny<ProductFilter>()));
        _productDataMock?.VerifyNoOtherCalls();
    }
}
