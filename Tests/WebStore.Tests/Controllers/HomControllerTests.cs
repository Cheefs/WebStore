using WebStore.Controllers;
using Microsoft.AspNetCore.Mvc;
using WebStore.Domain.Entities;
using Moq;
using WebStore.Interfaces.Services;
using WebStore.Domain;
using WebStore.Domain.ViewModels;
using System.Collections.Generic;
using System.Linq;

using Assert = Xunit.Assert;

namespace WebStore.Tests.Controllers;

[TestClass]
public class HomControllerTests
{
    [TestMethod]
    public void Contacts_returns_with_View()
    {
        var controller = new HomeController(null!);

        var result = controller.Contacts();

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Null(viewResult.ViewName);
    }

    [TestMethod]
    public void Error404_returns_with_View()
    {
        var controller = new HomeController(null!);

        var result = controller.Error404();

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Null(viewResult.ViewName);
    }

    [TestMethod]
    public void Greetings_with_Id_42_returns_with_Content_string_contains_42()
    {
        const string ID = "1";
        var expected = $"Hello from first controller - {ID}";
        var controller = new HomeController(null!);

        var result = controller.Greetings(ID);
        var viewResult = Assert.IsType<ContentResult>(result);

        Assert.Equal(expected, viewResult.Content);
    }

    [TestMethod]
    public void Index_returns_with_ViewBag_with_products()
    {
        const int TOTAL_COUNT = 100;
       
        var products = Enumerable
            .Range(1, TOTAL_COUNT)
            .Select(id => new Product { Id = id, Name = $"Product-{id}", Section = new() { Name = "Section" } });

        var controller = new HomeController(null!);
        var productsMock = new Mock<IProductData>();

        productsMock
            .Setup(s => s.GetProducts(It.IsAny<ProductFilter>()))
            .Returns(new Page<Product>(products, 1, TOTAL_COUNT, TOTAL_COUNT));

        var result = controller.Index(productsMock.Object);
        var viewResult = Assert.IsType<ViewResult>(result);
        var actualProductResult = viewResult.ViewData["products"];

        var actualProducts = Assert.IsAssignableFrom<IEnumerable<ProductViewModel>>(actualProductResult);

        Assert.Equal(TOTAL_COUNT, actualProducts.Count());
        Assert.Equal(
            products
                .Select(p => p.Name)
                .Take(TOTAL_COUNT),
            actualProducts.Select(p => p.Name)
        );
    }
}
