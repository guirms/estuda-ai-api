using Domain.Interfaces.Externals;
using Domain.Utils.Helpers;
using Microsoft.Extensions.Configuration;

namespace Infra.CrossCutting.Externals
{
    public class NodeRedExternal(IHttpClientFactory client, IConfiguration configuration) : BaseExternal($"{configuration.GetTag("NodeRedIp")}:1880/", client, configuration), INodeRedExternal
    {
        private const string BatchApi = "Batch/";

        public async Task Change()
        {
            try
            {
                var queryParams = new Dictionary<string, string?>
                {
                    ["customerId"] = "",
                };

                await Get(BatchApi + "Change", queryParams);
            }
            catch
            {
                throw new HttpRequestException("ErrorCommunicatingWithExternalService");
            }
        }
    }
}
