using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Legion.Models
{
    public class Contract
    {
        public string Id { get; set; }
        public int StatusId { get; set; }
        public int InvestorId { get; set; }
        public DateOnly DateStart { get; set; }
        public DateOnly DateEnd { get; set; }
        public int Amount { get; set; }
        public int ContractTypeId { get; set; }
        public int RepeatedId { get; set; }
        public int ReferralId { get; set; }
        public string Note {  get; set; }
    }
}
