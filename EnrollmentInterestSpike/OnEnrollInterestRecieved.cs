using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using EnrollmentInterestSpike.Models;

namespace EnrollmentInterestSpike
{
    public static class OnEnrollInterestRecieved
    {
        [FunctionName("OnEnrollInterestRecieved")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post",
            Route = null)] HttpRequest req,
            [Queue("interest-submission")] IAsyncCollector<ResidentInterestInput> interestQueue,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var submission = JsonConvert.DeserializeObject<ResidentInterestInput>(requestBody);
            await interestQueue.AddAsync(submission);

            string responseMessage = $"Interest Submission Recieved: {submission.Firstname} {submission.Lastname}";

            return new OkObjectResult(responseMessage);
        }
    }

    
}
