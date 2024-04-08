using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Legion.Models
{
    public class UserRole
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public int Id { get; set; }

        public string Role { get; set; } = null!;
        public bool CanAddContracts { get; set; } = false; // +
        public bool CanViewReports { get; set; } = false; // +
        public bool CanSeeHiddenData { get; set; } = false; // +
        public bool CanDeleteData { get; set; } = false;
        public bool CanManageUsers { get; set; } = false; // +
    }
}
