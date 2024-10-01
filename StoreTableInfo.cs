using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Azure.Data.Tables;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using ST10300512_CLDVPOE.Models;

namespace St10300512_CLDVPOE_Functions
{
    public class StoreTableInfo
    {
        private readonly ILogger<StoreTableInfo> _logger;
        private readonly TableServiceClient _tableServiceClient;

        public StoreTableInfo(ILogger<StoreTableInfo> logger)
        {
            _logger = logger;
            var connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            _tableServiceClient = new TableServiceClient(connectionString);
        }

        [Function("StoreTableInfo")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request to store table info.");

            var body = await new StreamReader(req.Body).ReadToEndAsync();
            CustomerProfile profile;

            try
            {
                profile = JsonSerializer.Deserialize<CustomerProfile>(body);
            }
            catch (JsonException ex)
            {
                _logger.LogError($"Deserialization failed: {ex.Message}");
                return new BadRequestObjectResult("Invalid profile data.");
            }

            var tableClient = _tableServiceClient.GetTableClient("CustomerProfiles");
            await tableClient.CreateIfNotExistsAsync();

            await tableClient.AddEntityAsync(profile);

            return new OkObjectResult("Profile stored successfully.");
        }
    }
}

