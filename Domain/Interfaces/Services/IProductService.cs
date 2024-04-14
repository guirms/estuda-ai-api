using Domain.Objects.Requests.Customer;
using Domain.Objects.Responses.Machine;

namespace Domain.Interfaces.Services
{
    public interface IProductService
    {
        Task<IEnumerable<ProductToTableResponse>?> GetToTable(int currentPage, string? productName);
        Task Save(SaveProductRequest saveProductRequest);
        Task Update(UpdateProductRequest updateProductRequest);
        Task Delete(int productId);
    }
}