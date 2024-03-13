using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Legion.Models
{
    public class Card
    {
        public int Id { get; set; }
        public string? CardNumber { get; set; }
        public int InvestorId { get; set; }
        public Investor Investor { get; set; }
    }
}
