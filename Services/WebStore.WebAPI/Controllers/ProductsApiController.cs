using Microsoft.AspNetCore.Mvc;
using WebStore.Domain;
using WebStore.Domain.DTO;
using WebStore.Interfaces;
using WebStore.Interfaces.Services;

namespace WebStore.WebAPI.Controllers;

/// <summary>Апи продуктов</summary>
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

    /// <summary>Получить все секции</summary>
    [HttpGet("sections")]
    public IActionResult GetSections() => Ok(_productData.GetSections().ToDTO());

    /// <summary>
    /// Получить секцию по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор</param>
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

    /// <summary>
    /// Получить все бренды
    /// </summary>
    [HttpGet("brands")]
    public IActionResult GetBrands() => Ok(_productData.GetBrands().ToDTO());

    /// <summary>
    /// Получить секцию по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор</param>
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

    /// <summary>
    /// Получить продукты используя фильтр
    /// </summary>
    /// <param name="productFilter">Фильтр продуктов</param>
    [HttpPost]
    public IActionResult GetProducts([FromBody] ProductFilter productFilter)
    {
        var products = _productData.GetProducts(productFilter);
        if(products.TotalCount > 0)
        {
            return Ok(products.ToDTO());
        }
        return NoContent();
    }
    /// <summary>
    /// Получить продукт по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор</param>
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
