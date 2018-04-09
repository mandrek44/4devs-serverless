
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Blob;

namespace FunctionApp1
{
    public static class GetSharedAccessSignatureForBlob
    {
        [FunctionName("GetSharedAccessSignatureForBlob")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequest req,
            [Blob("doneorders", FileAccess.Read, Connection = "OrdersStorage")]CloudBlobContainer photosContainer,
            TraceWriter log)
        {
            string fileName = req.Query["fileName"];
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return new BadRequestResult();
            }

            var photosBlob = await photosContainer.GetBlobReferenceFromServerAsync(fileName);
            var photoUri = GetBlobSasUri(photosBlob);

            return new JsonResult(new { PhotoUri = photoUri });
        }

        private static string GetBlobSasUri(ICloudBlob photosBlob)
        {
            var sasConstraints = new SharedAccessBlobPolicy();
            sasConstraints.SharedAccessStartTime = DateTimeOffset.UtcNow.AddHours(-1);
            sasConstraints.SharedAccessExpiryTime = DateTimeOffset.UtcNow.AddHours(12);

            sasConstraints.Permissions = SharedAccessBlobPermissions.List | SharedAccessBlobPermissions.Read;

            var token = photosBlob.GetSharedAccessSignature(sasConstraints);

            return photosBlob.Uri + token;
        }
    }
}
