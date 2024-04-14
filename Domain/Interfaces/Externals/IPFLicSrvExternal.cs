namespace Domain.Interfaces.Externals
{
    public interface IPFLicSrvExternal
    {
        Task<string> OpenLicense(string content);
    }
}
