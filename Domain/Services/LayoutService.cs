using AutoMapper;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using Domain.Models;
using Domain.Objects.Requests.Customer;
using Domain.Objects.Responses.Machine;
using Domain.Utils.Helpers;

namespace Domain.Services
{
    public class LayoutService(ILayoutRepository layoutRepository, IProductRepository productRepository, IMapper mapper) : ILayoutService
    {
        public async Task<IEnumerable<LayoutToTableResponse>?> GetToTable(int currentPage, string? layoutName) =>
            await layoutRepository.GetLayoutTypedData<LayoutToTableResponse>(currentPage, layoutName);

        public async Task Save(SaveLayoutRequest saveLayoutRequest)
        {
            if (!await productRepository.HasProductById(saveLayoutRequest.ProductId))
                throw new InvalidOperationException("ProductNotFound");

            await layoutRepository.Save(mapper.Map<Layout>(saveLayoutRequest));
        }

        public async Task Update(UpdateLayoutRequest updateLayoutRequest)
        {
            var layout = await layoutRepository.GetById(updateLayoutRequest.LayoutId)
                    ?? throw new InvalidOperationException("LayoutNotFound");

            layout = updateLayoutRequest.MapIgnoringNullProperties(layout);
            layout.UpdatedAt = DateTime.Now;

            await layoutRepository.Update(layout);
        }

        public async Task Delete(int layoutId) => await layoutRepository.Delete(layoutId);
    }
}
