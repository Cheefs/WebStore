using Microsoft.AspNetCore.Mvc;
using WebStore.Domain;
using WebStore.Domain.DTO;
using WebStore.Interfaces;
using WebStore.Interfaces.Services;

namespace WebStore.WebAPI.Controllers;

[ApiController]
[Route(WebApiAdresses.V1.Products)]
public class ProductsApiController : ControllerBase
{
    private readonly IProductData _productData;
    private readonly ILogger<ProductsApiController> _logger;

    public ProductsApiController(IProductData productData, ILogger<ProductsApiController> logger)
    {
        this._productData = productData;
        this._logger = logger;
    }

    [HttpGet("sections")]
    public IActionResult GetSections() => Ok(_productData.GetSections().ToDTO());

    [HttpGet("sections/{id:int}")]
    public IActionResult GetSectionById(int id)
    {
        var result = _productData.GetSectionById(id);
        if (result is null)
        {
            return NotFound(new { id });
        }
        return Ok(result.ToDTO());
    }

    [HttpGet("brands")]
    public IActionResult GetBrands() => Ok(_productData.GetBrands().ToDTO());

    [HttpGet("brands/{id:int}")]
    public IActionResult GetBrandById(int id)
    {
        var result = _productData.GetBrandById(id);

        if(result is null)
        {
            return NotFound(new { id });
        }

        return Ok(result.ToDTO());
    }

    [HttpPost]
    public IActionResult GetProducts([FromBody] ProductFilter productFilter)
    {
        var products = _productData.GetProducts(productFilter);
        if(products.Any())
        {
            return Ok(products.ToDTO());
        }
        return NoContent();
    }

    [HttpGet("{id:int}")]
    public IActionResult GetProductById(int id)
    {
        var result = _productData.GetProductById(id);
        if(result is null)
        {
            return NotFound(new { id });
        }

        return Ok(result.ToDTO());
    }
}
