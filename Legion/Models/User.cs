﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Legion.Models
{
    public class User
    {
        public User()
        {
            //Admin = false;
            UserName = string.Empty;
            Password = string.Empty;
            EmployerFirstName = string.Empty;
        }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public int Id { get; set; }

        public bool Admin {  get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string EmployerFirstName { get; set; }
    }
}
