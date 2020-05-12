using System;
using System.IO;
using System.Threading.Tasks;
using EnrollmentInterestSpike.Models;
using iTextSharp.text.pdf;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace EnrollmentInterestSpike
{
    public static class GenerateInterestDocument
    {
        [FunctionName("GenerateInterestDocument")]
        public static async Task Run([QueueTrigger("interest-submission",
            Connection = "AzureWebJobsStorage")]ResidentInterestInput interest,
            IBinder binder,
            [Blob("resources/interest-form.pdf", FileAccess.Read)] Stream template,
            [Table("submissions")] IAsyncCollector<Submission> submissionTable,
            [Queue("notifications")] IAsyncCollector<Submission> notificationQueue,
            ILogger log)
        {
            string documentOutputName = $"interest-submission/{interest.Firstname.ToLower()}_{interest.Lastname.ToLower()}.pdf";

            var interestOutputDoc = await binder.BindAsync<Stream>(
                new BlobAttribute(documentOutputName, FileAccess.Write)
                {
                    Connection = "AzureWebJobsStorage"

                });

            // Open existing PDF
            var pdfReader = new PdfReader(template);

            // PdfStamper, which will create
            var stamper = new PdfStamper(pdfReader, interestOutputDoc);

            var form = stamper.AcroFields;
            form.GenerateAppearances = true;

            form.SetField("prescriber", "");
            form.SetField("facilityname", interest.Facility);
            form.SetField("female", "x");
            form.SetField("male", "x");
            form.SetField("residentName", $"{interest.Firstname} {interest.Lastname}");
            form.SetField("dateOfAdmission", interest.DateOfAdmission);
            form.SetField("ssn", interest.SSN);
            form.SetField("dob", interest.DOB);
            form.SetField("phone", interest.Phone);
            form.SetField("email", interest.Email);
            form.SetField("station", interest.Station);
            form.SetField("roomNumber", interest.Room);
            form.SetField("bedNumber", interest.Bed);
            form.SetField("hasOtherInsurance", "X!");
            form.SetField("hasMedicarePlan", "X!");
            form.SetField("medicarePlan", interest.MedicarePlanNumber);
            form.SetField("hasPrivateInsuranceCarrier", "X!");
            form.SetField("hasMedicaidPlan", "X!");
            form.SetField("insuranceCarrier", "REPLACED!");
            form.SetField("medicaidPlan", interest.MedicaidPlanNumber);
            form.SetField("insuranceGroupId", interest.InsuranceGroupNumber);
            form.SetField("insuranceEffectiveDate", interest.EffectiveDate);
            form.SetField("insuranceId", interest.InsuranceIDNumber);
            form.SetField("insuranceCardNumber", "");
            form.SetField("insurancePhoneNumber", interest.InsurancePhone);
            form.SetField("pnc", interest.PCN);
            form.SetField("bin", interest.BIN);
            form.SetField("rpName", $"{interest.RPFirstname} {interest.RPLastname}");
            form.SetField("rpRelationship", interest.Relationship);
            form.SetField("rpAddress", "");
            form.SetField("rpCity", "");
            form.SetField("rpState", "");
            form.SetField("rpZip", "");
            form.SetField("rpEmail", interest.RPEmail);
            form.SetField("rpPhone", interest.RPPhone);

            // "Flatten" the form so it wont be editable/usable anymore
            stamper.FormFlattening = true;

            stamper.Close();
            pdfReader.Close();

            // save submission to table storage
            var submission = new Submission()
            {
                PartitionKey = "submissions",
                RowKey = DateTime.Now.ToString("hmmss"), // hacky-tacky for unique row key :)
                FacilityName = interest.Facility,
                ResidentName = $"{interest.Firstname} {interest.Lastname}",
                ResponsiblePartyName = $"{interest.RPFirstname} {interest.RPLastname}",
                EnrollmentFormPath = documentOutputName,
                SubmittedDateTime = DateTime.Now
            };
            await submissionTable.AddAsync(submission);

            // add to queue for notifications
            await notificationQueue.AddAsync(submission);

            log.LogInformation($"New interest form was generated for {interest.Firstname} {interest.Lastname}");
        }
    }
}
