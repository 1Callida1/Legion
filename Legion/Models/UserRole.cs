using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Legion.Models
{
    public class UserRole
    {
        public int Id { get; set; }
        public string Role { get; set; } = null!;
    }
}
