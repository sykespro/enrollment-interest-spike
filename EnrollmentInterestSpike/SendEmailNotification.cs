using System;
using EnrollmentInterestSpike.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Mail;

namespace EnrollmentInterestSpike
{
    public static class SendEmailNotification
    {
        [FunctionName("SendEmailNotification")]
        public static void Run([QueueTrigger("notifications", Connection = "AzureWebJobsStorage")] Submission submission,
             [SendGrid(ApiKey = "SendGrid:ApiKey")] ICollector<SendGridMessage> sender,
            ILogger log)
        {
            string enrollmentDocUrl = Environment.GetEnvironmentVariable("EnollmentDocumentPath") + submission.EnrollmentFormPath;

            var message = new SendGridMessage();
            message.From = new EmailAddress(Environment.GetEnvironmentVariable("SendGrid:EmailSender"));
            message.AddTo(new EmailAddress(Environment.GetEnvironmentVariable("SendGrid:TestEmailReciever")));

            message.Subject = "ValueMed Enrollment Interest Submitted";
            message.HtmlContent =
                $"New enrollment interest from submitted for <strong>{submission.ResidentName}</strong>, " +
                $"resident at <strong>{submission.FacilityName}</strong>. " +
                $"This submission was created at <strong>{submission.SubmittedDateTime.ToString()}</strong>.<br/><br/>" +
                $"Responsible Party: <strong>{submission.ResponsiblePartyName}</strong><br/><br/>" +
                $"<a href='{enrollmentDocUrl}'>View Enrollment Document</a><br/><br/>" +
                $"<i>*** This email if for demonstration purposes only and should not be used in a production. " +
                $"The data used is auto generated, does not belong to a real person.</i>";

            sender.Add(message);

            log.LogInformation($"C# Queue trigger function processed: {submission}");
        }
    }
}
