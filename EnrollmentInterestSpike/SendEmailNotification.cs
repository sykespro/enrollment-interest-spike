using System;
using EnrollmentInterestSpike.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace EnrollmentInterestSpike
{
    public static class SendEmailNotification
    {
        [FunctionName("SendEmailNotification")]
        public static void Run([QueueTrigger("notifications", Connection = "AzureWebJobsStorage")] Submission submission, ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {submission}");
        }
    }
}
