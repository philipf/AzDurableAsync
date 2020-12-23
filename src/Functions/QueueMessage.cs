using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace AzDurableAsync.Functions
{
    public static class QueueMessage
    {
        [FunctionName("QueueMessage")]
        public static bool Run([ActivityTrigger] string payload,
//            IDurableActivityContext context,
            ILogger log)
        {
            log.LogDebug($"Dummy, queue: {payload}");
            //log.LogDebug($"Dummy, queue: {context.InstanceId} - {payload}");

            return true;
        }
    }
}