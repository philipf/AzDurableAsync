using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace AzDurableAsync.Functions
{
    public static class Result
    {
        [FunctionName("Result")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, methods: "get", Route = "result/{instanceId}")]
            HttpRequest req,
            [DurableClient] IDurableOrchestrationClient client,
            string instanceId,
            ILogger log)
        {

            var status = await client.GetStatusAsync(instanceId);

            if (status.RuntimeStatus == OrchestrationRuntimeStatus.Completed)
            {
                return new OkObjectResult(status.Output);
            }
            else if (status.RuntimeStatus == OrchestrationRuntimeStatus.Running)
            {
                string checkStatusLocation = $"{req.Scheme}://{req.Host}/api/result/{instanceId}";
                string message = $"Your submission is being processed. The current status is {client.GetStatusAsync().Status}. To check the status later, go to: GET {checkStatusLocation}";

                ActionResult response = new AcceptedResult(checkStatusLocation, message); // The GET status location is returned as an http header
                return response;
            }
            else
            {
                return new BadRequestObjectResult(status.RuntimeStatus.ToString());
            }
        }

    }
}