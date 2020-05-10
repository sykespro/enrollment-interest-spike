using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace EnrollmentInterestSpike.Models
{
    public class Submission : TableEntity
    {

        public string ResidentName { get; set; }
        public string FacilityName { get; set; }
        public string ResponsiblePartyName { get; set; }
        public string EnrollmentFormPath { get; set; }
        public DateTime SubmittedDateTime { get; set; }

        public Submission()
        {

        }
    }
}
