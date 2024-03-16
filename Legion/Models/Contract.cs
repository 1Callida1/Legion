using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Legion.Models
{
    public class Contract
    {
        public Contract()
        {
            Id = string.Empty;
            DateStart = DateOnly.FromDateTime(DateTime.Now);
        }
        public string Id { get; set; }
        public ContractStatus Status { get; set; } = null!;
        public int InvestorId { get; set; }
        public Investor Investor { get; set; } = null!;
        public DateOnly DateStart { get; set; }
        public DateOnly DateEnd { get; set; }
        public int Amount { get; set; }
        public ContractType ContractType { get; set; } = null!;
        public User Manager { get; set; } = null!;
        public Repeated? Repeated { get; set; }
        public Referral Referral { get; set; } = null!;
        public string Note {  get; set; } = null!;
    }
}
