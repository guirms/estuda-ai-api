using Domain.Interfaces.Externals;
using Domain.Utils.Constants;
using Domain.Utils.Helpers;
using Microsoft.Extensions.Configuration;

namespace Infra.CrossCutting.Externals
{
    public class PFLicSrvExternal(IHttpClientFactory client, IConfiguration configuration) :
        BaseExternal($"{configuration.GetTag("PFLicSrvIp")}:5001/", client, configuration), IPFLicSrvExternal
    {
        public async Task<string> OpenLicense(string content)
        {
            try
            {
                var queryParams = new Dictionary<string, string?>
                {
                    ["content"] = content,
                    ["pass"] = Pwd.PFLicSrv
                };

                return await Get("OpenLicense", queryParams);
            }
            catch
            {
                throw new HttpRequestException("ErrorCommunicatingWithExternalService");
            }
        }
    }
}
