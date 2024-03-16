using Microsoft.Extensions.Hosting;
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
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string MiddleName { get; set; } = null!;
        public DateOnly DateBirth { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        public int PassprotSeries { get; set; }
        public int PassprotNumber { get; set; }
        public string Given { get; set; } = null!;
        public DateOnly PassportDateGiven { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        public string PassportUnitCode { get; set; } = null!;
        public string PassportRegistration {  get; set; } = null!;
        public string City { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Email {  get; set; } = null!;
        public bool PayType { get; set; }
        public string Note { get; set; } = null!;
        public string? CardNumber { get; set; }

        public ICollection<Contract> Contracts { get; } = new List<Contract>();
    }
}
