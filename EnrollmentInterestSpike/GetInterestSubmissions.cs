using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using EnrollmentInterestSpike.Models;
using System.Linq;
using Microsoft.WindowsAzure.Storage.Table;
using System.Net.Http;
using System.Net;
using System.Text;

namespace EnrollmentInterestSpike
{
    public static class GetInterestSubmissions
    {
        [FunctionName("GetInterestSubmissions")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            [Table("submissions")] CloudTable submissionTable,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            TableQuery<Submission> submissionQuery = new TableQuery<Submission>();

            var submissions = await submissionTable.ExecuteQuerySegmentedAsync(submissionQuery, null);

            var dataResponse = JsonConvert.SerializeObject(
                submissions.OrderByDescending(o => o.Timestamp).ToList() // hack due to TableQuery non support for OrderBy
                );

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(dataResponse, Encoding.UTF8, "application/json")
            };
        }
    }
}
