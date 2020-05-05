using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace EnrollmentInterestSpike
{
    public static class GenerateInterestDocument
    {
        [FunctionName("GenerateInterestDocument")]
        public static void Run([QueueTrigger("interest-submission",
            Connection = "AzureWebJobsStorage")]string myQueueItem,
            ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
        }
    }
}
