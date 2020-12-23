using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace AzDurableAsync.Functions
{
    public static class OnScoreReceived
    {
        [FunctionName("OnScoreReceived")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, methods: "post", Route = "score/{instanceId}")]
            HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient client,
            string instanceId,
            ILogger log)
        {
            log.LogInformation("Before raising event");
            int score = new Random().Next(1, 100);
            await client.RaiseEventAsync(instanceId, "Processed", score);
            log.LogInformation("After raising event");

            return new OkResult();
        }
    }
}