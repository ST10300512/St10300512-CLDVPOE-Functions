using System.IO;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Azure.Storage.Blobs;
using ST10300512_CLDVPOE.Models;
namespace St10300512_CLDVPOE_Functions
{
    public class UploadBlob
    {
        private readonly ILogger<UploadBlob> _logger;
        private readonly BlobServiceClient _blobServiceClient;

        public UploadBlob(ILogger<UploadBlob> logger, BlobServiceClient blobServiceClient)
        {
            _logger = logger;
            _blobServiceClient = blobServiceClient; 
        }

        [Function("UploadBlob")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            if (req.Form.Files.Count == 0)
            {
                return new BadRequestObjectResult("No file uploaded.");
            }

            var file = req.Form.Files[0];
            string containerName = "product-images"; 
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync(); 

            string blobName = file.FileName; 
            var blobClient = containerClient.GetBlobClient(blobName);

            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, true);
            }

            return new OkObjectResult($"File {blobName} uploaded successfully to {containerName}.");
        }
    }
}
