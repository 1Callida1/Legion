using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Legion.Models
{
    public class ContractType
    {
        public int Id { get; set; }
        public string TypeName { get; set; } = string.Empty;
        public int Period { get; set; }
        public int Bet { get; set; }
        public string Formula { get; set; } = string.Empty;
        public string ContractIdFormat { get; set; } = string.Empty;
    }
}
