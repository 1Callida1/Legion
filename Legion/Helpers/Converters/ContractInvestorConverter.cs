using Avalonia.Data.Converters;
using Avalonia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Legion.Models;

namespace Legion.Helpers.Converters
{
    public class ContractInvestorConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        {
            Investor investor = ((Contract)value).Investor;
            return $"{investor.LastName} {investor.FirstName[0]}. {investor.MiddleName[0]}.";
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
