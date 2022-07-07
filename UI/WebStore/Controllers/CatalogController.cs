using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using WebStore.Domain;
using WebStore.Domain.ViewModels;
using WebStore.Services.Mapping;
using WebStore.Interfaces.Services;

namespace WebStore.Controllers;

public class CatalogController : Controller
{
    private readonly IProductData _productData;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;

    public CatalogController(IProductData ProductData, IMapper Mapper, IConfiguration Configuration)
    {
        _productData = ProductData;
        _mapper = Mapper;
        _configuration = Configuration;
    }

    public IActionResult Index([Bind("SectionId,BrandId,PageNumber,PageSize")] ProductFilter filter)
    {
        filter.PageSize ??= int.TryParse(_configuration["CatalogPageSize"], out var page_size) ? page_size : null;
        var products = _productData.GetProducts(filter);

        return View(new CatalogViewModel
        {
            BrandId = filter.BrandId,
            SectionId = filter.SectionId,
            Products = products.Items.OrderBy(p => p.Order).Select(p => _mapper.Map<ProductViewModel>(p)),
            PageModel = new()
            {
                Page = filter.PageNumber,
                PageSize = filter.PageSize ?? 0,
                TotalPages = products.PageCount,
            }
        });
    }

    public IActionResult Details(int Id)
    {
        var product = _productData.GetProductById(Id);
        if (product is null)
            return NotFound();

        return View(product.ToView());
    }
}
