using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace AzDurableAsync.Functions
{
    public static class Submit
    {
        [FunctionName("Submit")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "submit")] HttpRequest req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {

            string payload = req.Body.ToString();

                string instanceId = await starter.StartNewAsync("RunOrchestrator", null);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            string checkStatusLocation = $"{req.Scheme}://{req.Host}/api/result/{instanceId}";
            string message = $"Your submission is being processed. The current status is {starter.GetStatusAsync().Status}. To check the status later, go to: GET {checkStatusLocation}"; 

            ActionResult response = new AcceptedResult(checkStatusLocation, message); // The GET status location is returned as an http header
            req.HttpContext.Response.Headers.Add("retry-after", "20"); // To inform the client how long to wait before checking the status. 
            return response;
        }
    }
}