using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Azure.Storage.Blobs;
using System.IO;
using System.Threading.Tasks;
using ST10300512_CLDVPOE.Models;
namespace St10300512_CLDVPOE_Functions
{
    public class UploadFile
    {
        private readonly ILogger<UploadFile> _logger;
        private readonly string _connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");

        public UploadFile(ILogger<UploadFile> logger)
        {
            _logger = logger;
        }

        [Function("UploadFile")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request to upload a file.");

            var formCollection = await req.ReadFormAsync();
            var file = formCollection.Files[0];

            if (file.Length > 0)
            {
                var blobClient = new BlobClient(_connectionString, "contracts-logs", file.FileName);
                using (var stream = file.OpenReadStream())
                {
                    await blobClient.UploadAsync(stream);
                }

                return new OkObjectResult($"File {file.FileName} uploaded successfully.");
            }

            return new BadRequestObjectResult("No file uploaded.");
        }
    }
}
