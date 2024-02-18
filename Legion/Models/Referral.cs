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
        public Investor InvestorCalled { get; set; }
        public Investor InvestorInvited { get; set; }
        public int Bonus {  get; set; }
        public string Note { get; set; }
    }
}
