using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace St10300512_CLDVPOE_Functions
{
    public class ProcessQueueMessage
    {
        private readonly ILogger<ProcessQueueMessage> _logger;

        public ProcessQueueMessage(ILogger<ProcessQueueMessage> logger)
        {
            _logger = logger;
        }

        [Function("ProcessQueueMessage")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            // Read the queue message from the HTTP request body
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var queueMessage = JsonSerializer.Deserialize<string>(requestBody);

            if (string.IsNullOrEmpty(queueMessage))
            {
                return new BadRequestObjectResult("Queue message cannot be null or empty.");
            }

            _logger.LogInformation($"Processed message: {queueMessage}");

            // Here you would add your logic to process the message
            await Task.CompletedTask; // Replace with actual processing logic

            return new OkObjectResult("Message processed successfully.");
        }
    }
}
