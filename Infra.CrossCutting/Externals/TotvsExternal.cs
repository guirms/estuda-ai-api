using Domain.Interfaces.Externals;
using Domain.Utils.Helpers;
using Microsoft.Extensions.Configuration;

namespace Infra.CrossCutting.Externals
{
    public class TotvsExternal(IHttpClientFactory client, IConfiguration configuration) :
        BaseExternal($"{configuration.GetTag("TotvsExternalIp")}:5002/", client, configuration), ITotvsExternal
    {
        public async Task<int> GetCustomerId(string document, bool isCpf)
        {
            try
            {
                //var queryParams = new Dictionary<string, string?>
                //{
                //    ["document"] = document,
                //    ["isCpf"] = isCpf.ToString(),
                //};

                //var response = await Get("GetCustomerId", queryParams);

                //return int.Parse(response);
                return await Task.FromResult(new Random().Next(0, 1001));
            }
            catch
            {
                throw new HttpRequestException("ErrorCommunicatingWithExternalService");
            }
        }

        public async Task<bool> IsCustomerActive(int totvsUserId)
        {
            try
            {
                //var queryParams = new Dictionary<string, string?>
                //{
                //    ["totvsUserId"] = totvsUserId.ToString()
                //};

                //var response = await Get("IsCustomerActive", queryParams);

                //return bool.Parse(response);
                return await Task.FromResult(true);
            }
            catch
            {
                throw new HttpRequestException("ErrorCommunicatingWithExternalService");
            }
        }
    }
}
