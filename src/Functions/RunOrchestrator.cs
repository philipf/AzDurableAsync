using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Build.Framework;

namespace AzDurableAsync.Functions
{
    public static class RunOrchestrator
    {
        [FunctionName("RunOrchestrator")]
        public static async Task<int> Run([OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            string payload = context.GetInput<string>(); // dummy payload
            var queueMessageResult = await context.CallActivityAsync<bool>("QueueMessage", payload);

            if (queueMessageResult)
            {
                int score = await context.WaitForExternalEvent<int>("Processed", TimeSpan.FromMinutes(5));
                return score;
            }

            return -1;
        }
    }
}