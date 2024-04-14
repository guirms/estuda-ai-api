namespace Domain.Interfaces.Externals
{
    public interface ITotvsExternal
    {
        Task<int> GetCustomerId(string document, bool isCpf);
        Task<bool> IsCustomerActive(int totvsUserId);
    }
}
