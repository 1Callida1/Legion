using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Legion.Models
{
    public class Investor
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public DateOnly DateBirth { get; set; }
        public int PassprotSeries { get; set; }
        public int PassprotNumber { get; set; }
        public DateOnly PassportDateGiven { get; set; }
        public string PassportUnitCode { get; set; }
        public string PassportRegistration {  get; set; }
        public string City { get; set; }
        public string Phone { get; set; }
        public string Email {  get; set; }
        public string Note { get; set; }
    }
}
