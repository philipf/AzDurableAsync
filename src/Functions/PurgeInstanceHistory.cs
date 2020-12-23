using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DurableTask.Core;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace AzDurableAsync.Functions
{
    public static class PurgeInstanceHistory
    {
        [FunctionName("PurgeInstanceHistory")]
        public static async Task Run(
            [DurableClient] IDurableOrchestrationClient client,
            [TimerTrigger("0 0 12 * * *")] TimerInfo myTimer, // Run at 12AM each day
            ILogger log)
        {
            log.LogDebug("PurgeInstanceHistory started");

            // remove messages older than 30 days
            var purgeResult = await client.PurgeInstanceHistoryAsync(
                DateTime.MinValue,
                DateTime.UtcNow.AddDays(-30),
                new List<OrchestrationStatus>
                {
                    OrchestrationStatus.Completed,
                    OrchestrationStatus.Terminated,
                    OrchestrationStatus.Failed
                });

            log.LogInformation($"PurgeInstanceHistory completed, instances deleted: {purgeResult.InstancesDeleted}");
        }
    }
}