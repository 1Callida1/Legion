using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Legion.Models
{
    public class Referral
    {
        public int Id { get; set; }
        public Investor InvestorCalled { get; set; } = null!;
        public Investor InvestorInvited { get; set; } = null!;
        public int Bonus {  get; set; }
        public string Note { get; set; } = null!;
    }
}
