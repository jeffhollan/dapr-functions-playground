using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using System.Web.Http;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace function1
{
    public class GetState
    {
        private readonly string DaprUrl = Environment.GetEnvironmentVariable("dapr-url");
        private readonly string StateStore = Environment.GetEnvironmentVariable("state-store");
        private readonly HttpClient _client;
        public GetState(IHttpClientFactory factory)
        {
            _client = factory.CreateClient();
        }
        
        [FunctionName("GetState")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var res = await _client.GetAsync($"{DaprUrl}/v1.0/state/{StateStore}/data");

            return new FileStreamResult(await res.Content.ReadAsStreamAsync(), "application/json");
        }

        [FunctionName("AddQueueMessage")]
        public IActionResult AddQueueMessage(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "addQueue/{queueName}")] HttpRequest req,
            [RabbitMQ(
                ConnectionStringSetting = "RabbitMQConnection",
                QueueName = "{queueName}")] out string outputMessage,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            outputMessage = DateTime.UtcNow.ToString();

            return new OkResult();
        }
    }
}
