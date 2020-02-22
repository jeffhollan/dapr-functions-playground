using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;

namespace function1
{
    public static class RabbitMqTrigger
    {
        [FunctionName("RabbitMqTrigger-AzureFunction")]
        public static void Run(
            [RabbitMQTrigger("queue", ConnectionStringSetting = "RabbitMQConnection")] BasicDeliverEventArgs inputMessage,
            ILogger log)
        {
            var content = Encoding.UTF8.GetString(inputMessage.Body);
            // var isRedelivered = inputMessage.Redelivered;
            var messageId = inputMessage.BasicProperties.MessageId;
            log.LogInformation($"C# Queue trigger function processed: {content} with messageId {messageId}.");
        }

        [FunctionName("RabbitMqTrigger-Dapr")]
        public static async Task<IActionResult> Dapr(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "rabbitmq")] HttpRequest req,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            string requestHeaders = JsonConvert.SerializeObject(req.Headers);
            log.LogInformation($"C# Queue trigger function processed: {requestBody}");
            log.LogInformation($"Request headers\n\n{requestHeaders}");

            return new OkResult();
        }
    }
}
