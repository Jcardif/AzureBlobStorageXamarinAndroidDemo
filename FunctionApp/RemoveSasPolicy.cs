using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;

namespace FunctionApp
{
    public static class RemoveSasPolicy
    {
        [FunctionName("RemoveSas")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]HttpRequestMessage req, ILogger log)
        {
            log.LogInformation("RemoveSasPolicy trigger function processed a request.");

            var keyValuePairs = req.GetQueryNameValuePairs().ToList();
            var policyKey = keyValuePairs
                .Find(q => string.Compare(q.Key, "policyKey", StringComparison.OrdinalIgnoreCase) == 0)
                .Value;
            var containerName = keyValuePairs
                .Find(q => string.Compare(q.Key, "containerName", StringComparison.OrdinalIgnoreCase) == 0)
                .Value;
            log.LogInformation($"Received container name as {containerName} and policy key as {policyKey}");
            if (policyKey == null)
            {
                // Get request body
                dynamic data = await req.Content.ReadAsAsync<object>();
                policyKey = data?.policyKey;
                if (policyKey == null)
                {
                    log.LogInformation("policy Key not valid");
                    return req.CreateResponse(HttpStatusCode.BadRequest, "Pass a valid policy Key");
                }
            }

            log.LogInformation("Waiting to remove");
            Thread.Sleep(240000);
            log.LogInformation("Time to remove");
            var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["BlobConnectionSstring"].ConnectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();
            var blobContainer = blobClient.GetContainerReference(containerName);
            if (!blobContainer.Exists())
                return req.CreateResponse(HttpStatusCode.NotFound, "Requested Container not found");
            var permissions = blobContainer.GetPermissions();
            if (permissions.SharedAccessPolicies.ContainsKey(policyKey))
            {
                permissions.SharedAccessPolicies.Remove(policyKey);
                blobContainer.SetPermissions(permissions);
                return req.CreateResponse(HttpStatusCode.OK, "Policy Key Removed");
            }
            log.LogInformation("policy Key  not found");
            return req.CreateResponse(HttpStatusCode.NotFound, "Policy Key Not Found");
        }
    }
}
