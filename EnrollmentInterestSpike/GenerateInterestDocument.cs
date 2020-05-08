using System;
using System.IO;
using iTextSharp.text.pdf;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace EnrollmentInterestSpike
{
    public static class GenerateInterestDocument
    {
        [FunctionName("GenerateInterestDocument")]
        public static void Run([QueueTrigger("interest-submission",
            Connection = "AzureWebJobsStorage")]ResidentInterestInput interest,
            [Blob("resources/interest-form.pdf", FileAccess.Read)] Stream template,
            [Blob("interest-submission/test.pdf", FileAccess.Write)] Stream interestOutputDoc,
            ILogger log)
        {

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
            form.SetField("rpName", "REPLACED!");
            form.SetField("rpRelationship", "REPLACED!");
            form.SetField("rpAddress", "REPLACED!");
            form.SetField("rpCity", "REPLACED!");
            form.SetField("rpState", "REPLACED!");
            form.SetField("rpZip", "REPLACED!");
            form.SetField("rpEmail", "REPLACED!");
            form.SetField("rpPhone", "REPLACED!");

            // "Flatten" the form so it wont be editable/usable anymore
            stamper.FormFlattening = true;

            stamper.Close();
            pdfReader.Close();
      
            log.LogInformation($"New interest from was generated for {interest.Firstname} {interest.Lastname}");
        }
    }
}
