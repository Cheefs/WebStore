using WebStore.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Linq;

using Assert = Xunit.Assert;
using WebStore.Interfaces.TestApi;

namespace WebStore.Tests.Controllers;

[TestClass]
public class WebAPIControllerTests
{
    [TestMethod]
    public void Index_Returns_with_View_with_Values()
    {
        var expected = Enumerable.Range(1, 10).Select(i => $"Value-{i}");
        var serviceMock = new Mock<IValuesService>();
        serviceMock.Setup(x => x.GetValues()).Returns(expected);

        var controller = new WebAPIController(serviceMock.Object);
        var result = controller.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        var actual = Assert.IsAssignableFrom<IEnumerable<string>>(viewResult.Model);

        Assert.Equal(expected, actual);
        serviceMock.Verify(s => s.GetValues(), Times.Once());
        serviceMock.VerifyNoOtherCalls();
    }
}
