using System.Net;
using System.Net.Http.Json;
using WebStore.Domain;
using WebStore.Domain.DTO;
using WebStore.Domain.Entities;
using WebStore.Interfaces;
using WebStore.Interfaces.Services;
using WebStore.WebAPI.Clients.Base;

namespace WebStore.WebAPI.Clients.Products
{
    public class PrductsClient : BaseClient, IProductData
    {
        public PrductsClient(HttpClient client) : base(client, WebApiAdresses.V1.Products) { }

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

        public Page<Product> GetProducts(ProductFilter? filter = null)
        {
            var response = Post(Address, filter ?? new());
            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                return new(Enumerable.Empty<Product>(), 0, 0, 0);
            }

            return response.EnsureSuccessStatusCode()
                .Content
                .ReadFromJsonAsync<Page<ProductDTO>>()
                .Result?
                .FromDTO()!;
        }
    }
}
