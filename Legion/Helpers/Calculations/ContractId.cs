using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Legion.Helpers.Calculations
{
    public static class ContractId
    {
        public static string Generate(string formula, int id)
        {
            return formula.Replace("yyyy", DateTime.Now.Year.ToString()).Replace("yy", DateTime.Now.ToString("yy"))
                .Replace("id", id.ToString());
        }
    }
}
