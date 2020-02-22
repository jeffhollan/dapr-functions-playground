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
using System.Collections.Generic;

namespace function1
{
    public class Hello
    {
        private readonly string DaprUrl = Environment.GetEnvironmentVariable("dapr-url");
        private readonly string Function2Url = Environment.GetEnvironmentVariable("function2-url");
        private readonly string StateStore = Environment.GetEnvironmentVariable("state-store");
        private readonly HttpClient _client;
        public Hello(IHttpClientFactory factory)
        {
            _client = factory.CreateClient();
        }
        [FunctionName("Hello")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Hello function triggered.");
            
            log.LogInformation($"Invoking {DaprUrl}/v1.0/invoke/{Function2Url}");

            // securely call another function
            var res = await _client.GetAsync($"{DaprUrl}/v1.0/invoke/{Function2Url}");

            var stateRequest = new List<State>
            {
                new State
                {
                    Key = "data",
                    Value = await res.Content.ReadAsStringAsync()
                }
            };
            
            //store response
            _ = await _client.PostAsJsonAsync($"{DaprUrl}/v1.0/state/{StateStore}", stateRequest);

            return new OkResult();
        }
    }

    public class State {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
