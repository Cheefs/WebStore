using Microsoft.AspNetCore.Mvc;
using SimpleMvcSitemap;
using WebStore.Interfaces.Services;

namespace WebStore.Controllers;

[ApiController]
[Route("/sitemap")]
public class SiteMapApiController : ControllerBase
{
    [HttpGet]
    public IActionResult Get([FromServices] IProductData ProductData)
    {
        var nodes = new List<SitemapNode>
        {
            new(Url.Action("Index", "Home")),
            new(Url.Action("Greetings", "Home")),
            new(Url.Action("Contacts", "Home")),
            new(Url.Action("Test", "Home")),
            new(Url.Action("Index", "WebAPI")),
            new(Url.Action("Index", "Catalog")),
        };

        nodes.AddRange(ProductData.GetSections().Select(s => 
            new SitemapNode(Url.Action("Index", "Catalog", new { SectionId = s.Id }))
        ));

        nodes.AddRange(ProductData.GetBrands().Select(s =>
            new SitemapNode(Url.Action("Index", "Catalog", new { BrandId = s.Id }))
        ));

        nodes.AddRange(ProductData.GetProducts().Items.Select(s =>
          new SitemapNode(Url.Action("Details", "Catalog", new { s.Id }))
        ));

        return new SitemapProvider().CreateSitemap(new SitemapModel(nodes));
    }
}
