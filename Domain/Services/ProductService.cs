using AutoMapper;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using Domain.Models;
using Domain.Objects.Requests.Customer;
using Domain.Objects.Responses.Machine;
using Domain.Utils.Helpers;

namespace Domain.Services
{
    public class ProductService(IProductRepository productRepository, IAssetRepository assetRepository, IMapper mapper) : IProductService
    {
        public async Task<IEnumerable<ProductToTableResponse>?> GetToTable(int currentPage, string? productName) =>
            await productRepository.GetProductTypedData<ProductToTableResponse>(currentPage, productName);

        public async Task Save(SaveProductRequest saveProductRequest)
        {
            if (!await assetRepository.HasAssetById(saveProductRequest.AssetId))
                throw new InvalidOperationException("AssetNotFound");

            await productRepository.Save(mapper.Map<Product>(saveProductRequest));
        }

        public async Task Update(UpdateProductRequest updateProductRequest)
        {
            var product = await productRepository.GetById(updateProductRequest.ProductId)
                    ?? throw new InvalidOperationException("ProductNotFound");

            product = updateProductRequest.MapIgnoringNullProperties(product);
            product.UpdatedAt = DateTime.Now;

            await productRepository.Update(product);
        }

        public async Task Delete(int productId) => await productRepository.Delete(productId);
    }
}
