using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

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

    public class ResidentInterestInput
    {
        // facility information
        public string Facility { get; set; }

        // resident information
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string DateOfAdmission { get; set; } // date conversion
        public string Gender { get; set; }
        public string DOB { get; set; } // date formatted without time
        public string SSN { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

        // resident in-facility information
        public string Station { get; set; }
        public string Room { get; set; }
        public string Bed { get; set; }

        // resident insurance information
        public string MedicaidPlanNumber { get; set; }
        public string MedicarePlanNumber { get; set; }
        public string PrivateInsuranceCarrier { get; set; }
        public string CardNumber { get; set; }
        public string EffectiveDate { get; set; } // date formatted without time
        public string InsuranceIDNumber { get; set; }
        public string InsuranceGroupNumber { get; set; }
        public string InsurancePhone { get; set; }
        public string BIN { get; set; }
        public string PCN { get; set; }

        // responsible party (RP) information
        public string Relationship { get; set; }
        public string RPFirstname { get; set; } // refactor at some point
        public string RPLastname { get; set; }
        public string Address { get; set; }        
        public string RPPhone { get; set; }
        public string RPEmail { get; set; }

    }
}
