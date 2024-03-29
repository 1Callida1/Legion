using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Legion.Models
{
    public class Contract
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public int Id { get; set; }

        public string CustomId { get; set; } = string.Empty;
        public float Bet { get; set; } = 0;
        public ContractStatus Status { get; set; } = null!;
        public Investor Investor { get; set; }
        public DateTime DateStart { get; set; } = DateTime.Now;
        public DateTime DateEnd { get; set; }
        public int Amount { get; set; }
        public ContractType ContractType { get; set; } = null!;
        public User Manager { get; set; } = null!;
        public bool Repeated { get; set; } = false;
        public int RepeatNumber { get; set; } = 0;
        public Referral? Referral { get; set; } = null!;
        public DateTime DateProlonagtion { get; set; } = DateTime.Now;
        public string? Note {  get; set; } = null!;
    }
}
