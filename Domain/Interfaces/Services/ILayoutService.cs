using Domain.Objects.Requests.Customer;
using Domain.Objects.Responses.Machine;

namespace Domain.Interfaces.Services
{
    public interface ILayoutService
    {
        Task<IEnumerable<LayoutToTableResponse>?> GetToTable(int currentPage, string? layoutName);
        Task Save(SaveLayoutRequest saveLayoutRequest);
        Task Update(UpdateLayoutRequest updateLayoutRequest);
        Task Delete(int layoutId);
    }
}