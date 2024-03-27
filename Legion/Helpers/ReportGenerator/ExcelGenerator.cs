using Legion.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Legion.Helpers.ReportGenerator
{
    public class ExcelGenerator
    {
        public byte[] Generate(ObservableCollection<Legion.Models.Contract> contracts = null, ObservableCollection<RenewalContract> renewalContracts = null, ObservableCollection<Investor> investors = null)
        {
            ExcelPackage package = new();

            ExcelWorksheet sheet = package.Workbook.Worksheets.Add("Доход безнал");
            sheet.Cells[8, 2].Value = "Имя инвестора:";
            int row = 9;
            int column = 2;
            foreach (Investor investor in investors)
            {
                sheet.Cells[row, column].Value = investor.FirstName;
                row++;
            }

            return package.GetAsByteArray();

        }
    }
}
