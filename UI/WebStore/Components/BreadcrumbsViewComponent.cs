using Microsoft.AspNetCore.Mvc;
using WebStore.Domain.ViewModels;
using WebStore.Interfaces.Services;

namespace WebStore.Components;

public class BreadcrumbsViewComponent : ViewComponent
{
    private readonly IProductData _ProductData;

    public BreadcrumbsViewComponent(IProductData ProductData) => _ProductData = ProductData;

    public IViewComponentResult Invoke()
    {
        var model = new BreadcrumbsViewModel();

        if (int.TryParse(Request.Query["sectionId"], out var sectionId))
        {
            model.Section = _ProductData.GetSectionById(sectionId);

            if (model.Section is { ParentId: { } parentSectionId, Parent: null })
                model.Section.Parent = _ProductData.GetSectionById(parentSectionId);
        }

        if (int.TryParse(Request.Query["BrandId"], out var brandId))
            model.Brand = _ProductData.GetBrandById(brandId);

        if (int.TryParse(Request.RouteValues["id"]?.ToString(), out var productId))
            model.Product = _ProductData.GetProductById(productId)?.Name;

        return View(model);
    }
}