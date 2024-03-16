using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Legion.Models
{
    public class RenewalContract
    {
        public int Id { get; set; }
        public Contract Contract { get; set; } = null!;
        public DateOnly NewDateEnd { get; set; }
    }
}
