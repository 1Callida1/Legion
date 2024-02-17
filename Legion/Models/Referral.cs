using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Legion.Models
{
    internal class Referral
    {
        public int Id { get; set; }
        public int InvestorCalledId { get; set; }
        public int InvestorInvitedId { get; set; }
        public int Bonus {  get; set; }
        public string Note { get; set; }
    }
}
