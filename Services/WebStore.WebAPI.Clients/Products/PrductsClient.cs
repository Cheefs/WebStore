using System.Net;
using System.Net.Http.Json;
using WebStore.Domain;
using WebStore.Domain.DTO;
using WebStore.Domain.Entities;
using WebStore.Interfaces.Services;
using WebStore.WebAPI.Clients.Base;

namespace WebStore.WebAPI.Clients.Products
{
    public class PrductsClient : BaseClient, IProductData
    {
        public PrductsClient(HttpClient client) : base(client, "api/products") { }

        public IEnumerable<Section> GetSections()
        {
            var result = Get<IEnumerable<SectionDTO>>($"{Address}/sections") ?? Enumerable.Empty<SectionDTO>();
            return result.FromDTO();
        }

        public Section? GetSectionById(int id) => Get<SectionDTO>($"{Address}/sections/{id}").FromDTO();

        public IEnumerable<Brand> GetBrands()
        {
            var result = Get<IEnumerable<BrandDTO>>($"{Address}/brands") ?? Enumerable.Empty<BrandDTO>();
            return result.FromDTO();
        }

        public Brand? GetBrandById(int id) => Get<BrandDTO>($"{Address}/brands/{id}").FromDTO();


        public Product? GetProductById(int id) => Get<ProductDTO>($"{Address}/{id}").FromDTO();

        public IEnumerable<Product> GetProducts(ProductFilter? filter = null)
        {
            var response = Post(Address, filter ?? new());
            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                return Enumerable.Empty<Product>();
            }

            return response.EnsureSuccessStatusCode()
                .Content
                .ReadFromJsonAsync<IEnumerable<ProductDTO>>()
                .Result?
                .FromDTO()!;
        }
    }
}
