using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Moq;

using WebStore.Controllers;
using WebStore.Domain.Entities.Orders;
using WebStore.Domain.ViewModels;
using WebStore.Interfaces.Services;
using Assert = Xunit.Assert;

namespace WebStore.Tests.Controllers;

[TestClass]
public class CatrControllerTests
{
    [TestMethod]
    public async Task CheckOut_ModelState_Invalid_Returns_View_with_Model()
    {
        const string DESCRIPTION = "Test description";

        var cartServiceMock = new Mock<ICartService>();
        var orderServiceMock = new Mock<IOrderService>();
        var controller = new CartController(cartServiceMock.Object);

        controller.ModelState.AddModelError("", "Invalid model");

        var orderModel = new OrderViewModel { Description = DESCRIPTION };

        var result = await controller.Checkout(orderModel, orderServiceMock.Object);
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<CartOrderViewModel>(viewResult.Model);

        Assert.Equal(DESCRIPTION, model.Order.Description);

        cartServiceMock.Verify(s => s.GetViewModel(), Times.Once);
        cartServiceMock.VerifyNoOtherCalls();
        orderServiceMock.VerifyNoOtherCalls();
    }

    [TestMethod]
    public async Task CheckOut_ModelState_Valid_Call_Service_and_Returns_Redirect()
    {

        const int ORDER_ID = 1;
        const string USER = "Test user";
        const string DESCRIPTION = "Test description";
        const string ADDERESS = "Test address";
        const string PHONE = "Test phone";

        var cartServiceMock = new Mock<ICartService>();
        cartServiceMock
           .Setup(c => c.GetViewModel())
           .Returns(
                new CartViewModel
                {
                    Items = new[] { (new ProductViewModel { Name = "Test product" }, 1) }
                });

        var orderServiceMock = new Mock<IOrderService>();
        orderServiceMock
           .Setup(c => c.CreateOrderAsync(
               It.IsAny<string>(),
               It.IsAny<CartViewModel>(),
               It.IsAny<OrderViewModel>(),
               It.IsAny<CancellationToken>()
            ))
           .ReturnsAsync(new Order
           {
               Id = ORDER_ID,
               Description = DESCRIPTION,
               Address = ADDERESS,
               Phone = PHONE,
               Date = DateTime.Now,
               Items = Array.Empty<OrderItem>(),
           });

        var controller = new CartController(cartServiceMock.Object)
        {
            ControllerContext = new()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, USER) }))
                }
            }
        };

        var orderModel = new OrderViewModel
        {
            Address = ADDERESS,
            Phone = PHONE,
            Description = DESCRIPTION,
        };

        var result = await controller.Checkout(orderModel, orderServiceMock.Object);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);

        Assert.Null(redirectResult.ControllerName);
        Assert.Equal(nameof(CartController.OrderConfirmed), redirectResult.ActionName);

        Assert.Equal(ORDER_ID, redirectResult.RouteValues!["id"]);

        orderServiceMock.Verify(s => s.CreateOrderAsync(
            It.Is<string>(user => user == USER),
            It.IsAny<CartViewModel>(),
            It.IsAny<OrderViewModel>(),
            It.IsAny<CancellationToken>()    
        ));

        orderServiceMock.VerifyNoOtherCalls();

        cartServiceMock.Verify(s => s.GetViewModel(), Times.Once);
        cartServiceMock.Verify(s => s.Clear());
        cartServiceMock.VerifyNoOtherCalls();
    }


    [TestMethod]
    public async Task Checkout_thrown_ArgumentNullException_when_OrderModel_is_null_1()
    {
        var cartServiceMock = new Mock<ICartService>();
        var controller = new CartController(cartServiceMock.Object);
        var orderServiceMock = new Mock<IOrderService>();

        Exception? error = null;
        try
        {
            await controller.Checkout(null!, orderServiceMock.Object);
        }
        catch (Exception e)
        {
            error = e;
        }

        var argumentNullException = Assert.IsType<ArgumentNullException>(error);
        Assert.Equal("OrderModel", argumentNullException.ParamName);
    }

    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public async Task Checkout_thrown_ArgumentNullException_when_OrderModel_is_null_2()
    {
        var cartServiceMock = new Mock<ICartService>();
        var controller = new CartController(cartServiceMock.Object);
        var orderServiceMock = new Mock<IOrderService>();

        await controller.Checkout(null!, orderServiceMock.Object);
    }

    [TestMethod]
    public async Task Checkout_thrown_ArgumentNullException_when_OrderModel_is_null_3()
    {
        var cartServiceMock = new Mock<ICartService>();
        var controller = new CartController(cartServiceMock.Object);
        var orderServiceMock = new Mock<IOrderService>();

        var argumentNullException = await Assert.ThrowsAsync<ArgumentNullException>(async () => 
            await controller.Checkout(null!, orderServiceMock.Object)
        );
        Assert.Equal("OrderModel", argumentNullException.ParamName);
    }
}
