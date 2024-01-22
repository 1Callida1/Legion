using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Legion.Model
{
    internal class User
    {
        private string _username;
        public string UserName
        {
            get => _username;
            set => _username = value;
        }

        private string _password;
        public string Password
        {
            get => _password;
            set => _password = value;
        }
    }
}
