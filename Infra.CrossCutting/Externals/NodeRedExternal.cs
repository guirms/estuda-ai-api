using Domain.Interfaces.Externals;
using Domain.Objects.Requests.Batch;
using Domain.Utils.Helpers;
using Microsoft.Extensions.Configuration;

namespace Infra.CrossCutting.Externals
{
    public class NodeRedExternal(IHttpClientFactory client, IConfiguration configuration) : BaseExternal($"{configuration.GetTag("NodeRedIp")}:1880/", client, configuration), INodeRedExternal
    {
        private const string BatchApi = "Batch/";

        public async Task Change(BatchRequest batchRequest)
        {
            try
            {
                var queryParams = new Dictionary<string, string?>
                {
                    ["customerId"] = batchRequest.CustomerId.ToString(),
                    ["name"] = batchRequest.Name,
                    ["cnpj"] = batchRequest.Cnpj,
                    ["insertedDateTime"] = DateTime.Now.ToStringBrFormat(),
                    ["authToken"] = batchRequest.AuthToken,
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
