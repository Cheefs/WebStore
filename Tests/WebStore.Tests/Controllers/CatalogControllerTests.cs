using WebStore.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Assert = Xunit.Assert;
using WebStore.Interfaces.Services;
using WebStore.Domain.Entities;
using AutoMapper;
using WebStore.Domain.ViewModels;

namespace WebStore.Tests.Controllers;

[TestClass]
public class CatalogControllerTests
{
    [TestMethod]
    public void Details_Returns_with_View()
    {
        const int PRODUCT_ID = 100;
        const string PRODUCT_NAME = "Test Name";
        const int PRODUCT_ORDER = 10;
        const decimal PRODUCT_PRICE = 13.5m;
        const string PRIDUCT_IMAGE_URL = "/img/product.img";

        const int BRAND_ID = 5;
        const string BRAND_NAME = "Test Brand";
        const int BRAND_ORDER = 5;

        const int SECTION_ID = 9;
        const string SECTION_NAME = "Section Name";
        const int SECTION_ORDER = 3;

        var productServiceMock = new Mock<IProductData>();
        productServiceMock
            .Setup(s => s.GetProductById(It.IsAny<int>()))
            .Returns<int>(id => new Product
            {
                Id = PRODUCT_ID,
                Name = PRODUCT_NAME,
                Order = PRODUCT_ORDER,
                Price = PRODUCT_PRICE,
                ImageUrl = PRIDUCT_IMAGE_URL,
                BrandId = BRAND_ID,
                Brand = new()
                {
                    Id = BRAND_ID,
                    Name = BRAND_NAME,
                    Order = BRAND_ORDER,
                },
                SectionId = SECTION_ID,
                Section = new()
                {
                    Id = SECTION_ID,
                    Name = SECTION_NAME,
                    Order = SECTION_ORDER,
                }
            });
        var mapperMock = new Mock<IMapper>();
        var controller = new CatalogController(productServiceMock.Object, mapperMock.Object);
        var result = controller.Details(PRODUCT_ID);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<ProductViewModel>(viewResult.Model);

        Assert.Equal(PRODUCT_ID, model.Id);
        Assert.Equal(PRODUCT_NAME, model.Name);
        Assert.Equal(PRODUCT_PRICE, model.Price);
        Assert.Equal(PRIDUCT_IMAGE_URL, model.ImageUrl);
        Assert.Equal(BRAND_NAME, model.Brand);
        Assert.Equal(SECTION_NAME, model.Section);

        productServiceMock.Verify(s => s.GetProductById(It.Is<int>(id => id == PRODUCT_ID)), Times.Once());
        productServiceMock.VerifyNoOtherCalls();
    }
}
