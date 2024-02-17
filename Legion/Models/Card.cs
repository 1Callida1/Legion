using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Legion.Models
{
    internal class Card
    {
        public int Id { get; set; }
        public int InvestorId { get; set; }
        public string CardNumber { get; set; }
    }
}
