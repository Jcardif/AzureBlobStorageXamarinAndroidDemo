using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace FunctionApp
{
    public static class GenerateSas
    {
        [FunctionName("GenerateSAS")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("GenerateBlobStorageSas function processed a request.");
            var containerName = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "containerName", StringComparison.OrdinalIgnoreCase) == 0)
                .Value;
            if (containerName == null)
            {
                // Get request body
                dynamic data = await req.Content.ReadAsAsync<object>();
                containerName = data?.containerName;
                if (containerName == null)
                {
                    log.Info("Conatiner Name not valid");
                    return req.CreateResponse(HttpStatusCode.BadRequest, "Pass a valid container name");
                }
            }
            var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["BlobConnectionSstring"].ConnectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();
            var blobContainer = blobClient.GetContainerReference(containerName);
            if (!blobContainer.Exists())
                return req.CreateResponse(HttpStatusCode.NotFound, "Requested Container not found");

            var containerPermissions = blobContainer.GetPermissions();
            var key = "policy_" + Guid.NewGuid();
            containerPermissions.SharedAccessPolicies.Add(key, new SharedAccessBlobPolicy
            {
                SharedAccessExpiryTime = DateTimeOffset.UtcNow.AddMinutes(7),
                Permissions = SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.Write
            });
            blobContainer.SetPermissions(containerPermissions);
            var sasToken = blobContainer.GetSharedAccessSignature(null, key);
            var client = new HttpClient();
            var uri = "https://" + $"aeroclubfunctionapp.azurewebsites.net/api/RemoveSas?code=DulpvoQCZJ0T5V6xy5GIa8pSir7fO5M4ygL58DOykAtVCX6Vf7zwOg==&policyKey={key}&containerName={containerName}";
            client.GetAsync(uri);
            return req.CreateResponse(HttpStatusCode.OK, sasToken);
        }
    }
}
