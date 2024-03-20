using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Legion.Models
{
    public class Investor
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public int Id { get; set; }

        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string MiddleName { get; set; } = null!;
        public DateTime DateBirth { get; set; } = DateTime.Now;
        public string PassprotSeries { get; set; }
        public string PassprotNumber { get; set; }
        public string Given { get; set; } = null!;
        public DateTime PassportDateGiven { get; set; } = DateTime.Now;
        public string PassportUnitCode { get; set; } = null!;
        public string? PassportRegistration {  get; set; } = null!;
        public string City { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Email {  get; set; } = null!;
        public bool PayType { get; set; }
        public bool IsCurrentInvestor { get; set; } = false;
        public string Note { get; set; } = null!;
        public string? CardNumber { get; set; }

        public ICollection<Contract> Contracts { get; } = new List<Contract>();
    }
}
