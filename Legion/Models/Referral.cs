using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Legion.Models
{
    public class Referral
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public int Id { get; set; }

        public Investor InvestorCalled { get; set; } = null!;
        public Investor InvestorInvited { get; set; } = null!;
        public int Bonus { get; set; }
        public bool BonusClaim {  get; set; }
        public string? Note { get; set; } = string.Empty;
    }
}
