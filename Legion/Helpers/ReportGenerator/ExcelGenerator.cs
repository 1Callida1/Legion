using Legion.Models;
using NickBuhro.NumToWords.Russian;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using SQLitePCL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static OfficeOpenXml.ExcelErrorValue;

namespace Legion.Helpers.ReportGenerator
{
    public class ExcelGenerator
    {
        private ApplicationDbContext _context = null!;
        public static byte[] GenerateReportCash(ObservableCollection<Legion.Models.Contract> cntrs, DateTime start)
        {
            ExcelPackage package = new ExcelPackage();
            ObservableCollection<Legion.Models.Contract> contracts = cntrs;

            ExcelWorksheet sheet = package.Workbook.Worksheets.Add("Доход безналичный");
            double sum_all_day = 0;
            contracts.ToList().ForEach(contract => sum_all_day += Convert.ToDouble(contract.Amount) * Convert.ToDouble(contract.Bet) / 100.0);

            sheet.Column(2).Width = 38;
            sheet.Column(3).Width = 25;
            sheet.Column(4).Width = 38;
            sheet.Cells[2, 2].Value = "ЗАЯВЛЕНИЕ";
            sheet.Cells[2, 2, 2, 4].Merge = true;
            sheet.Cells[2, 2, 2, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.Cells[2, 2].Style.Font.SetFromFont("Times New Roman", 14);

            sheet.Cells[3, 2].Value = $"Прошу выдать денежные средства на выплату дохода инвесторам за {start.Date.ToString("dd-MM-yyyy")},";
            sheet.Cells[3, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.Cells[3, 2, 3, 4].Merge = true;
            sheet.Cells[3, 2, 3, 4].Style.Font.SetFromFont("Times New Roman", 14);

            sheet.Cells[4, 2].Value = "в сумме:";
            sheet.Cells[4, 2].Style.Font.SetFromFont("Times New Roman", 14);

            sheet.Cells[5, 2].Value = $"Общая - {sum_all_day}";
            sheet.Cells[5, 2].Style.Font.Size = 14;
            sheet.Cells[5, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
            sheet.Cells[5, 2].Style.Fill.BackgroundColor.SetColor(ExcelIndexedColor.Indexed13);
            //table

            sheet.Cells[7, 2].Value = "ФИО";
            sheet.Cells[7, 3].Value = "СУММА";
            sheet.Cells[7, 4].Value = "ФОРМА ОПЛАТЫ";
            sheet.Cells[7, 2, 7, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


            sheet.Cells[7, 2, 7, 4].Style.Font.Bold = true;

            sheet.Cells[8, 2].Value = $"{start.Date.Day} число";
            sheet.Cells[8, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.Cells[8, 2].Style.Font.Bold = true;
            sheet.Cells[8, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
            sheet.Cells[8, 2].Style.Fill.BackgroundColor.SetColor(ExcelIndexedColor.Indexed13);

            int row = 9;
            int column = 2;

            double sum = 0;

            foreach (Models.Contract contract in contracts)
            {
                sheet.Cells[row, column].Value = $"{contract.Investor.LastName} {contract.Investor.FirstName} {contract.Investor.MiddleName}";
                sheet.Cells[row, column + 1].Value = Convert.ToDouble(contract.Amount) * Convert.ToDouble(contract.Bet) / 100.0;
                sheet.Cells[row, column + 2].Value = contract.Investor.CardNumber;

                sum += Convert.ToDouble(contract.Amount) * Convert.ToDouble(contract.Bet) / 100.0;
                row++;
            }

            sheet.Cells[9, 2, row, column + 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            sheet.Cells[row + 1, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.Cells[row + 1, 3].Value = sum.ToString();
            sheet.Cells[9, 2, row, 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
            sheet.Cells[9, 2, row, 4].Style.Fill.BackgroundColor.SetColor(ExcelIndexedColor.Indexed13);

            //table grid
            sheet.Cells[7, 2, row + 1, 4].Style.Border.BorderAround(ExcelBorderStyle.Hair);
            createTableBorder(sheet.Cells[7, 2, row + 1, 4], "Thin");

            sheet.Cells[row + 6, 2].Value = "Заместитель руководителя";
            sheet.Cells[row + 6, 4].Value = "Судаков Ю.С.";
            sheet.Cells[row + 6, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            sheet.Cells[row + 6, 2, row + 6, 4].Style.Font.SetFromFont("Times New Roman", 14);
            return package.GetAsByteArray();
        }

        public static byte[] GenerateReportCashless(ObservableCollection<Models.Contract> cntrs, DateTime start)
        {
            ExcelPackage package = new ExcelPackage();
            ObservableCollection<Models.Contract> contracts = cntrs;

            ExcelWorksheet sheet = package.Workbook.Worksheets.Add("Доход наличный");
            double sum_all_day = 0;
            contracts.ToList().ForEach(contract => sum_all_day += Convert.ToDouble(contract.Amount) * Convert.ToDouble(contract.Bet) / 100.0);

            sheet.Column(2).Width = 38;
            sheet.Column(3).Width = 25;
            sheet.Column(4).Width = 38;
            sheet.Cells[2, 2].Value = "ЗАЯВЛЕНИЕ";
            sheet.Cells[2, 2, 2, 4].Merge = true;
            sheet.Cells[2, 2, 2, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.Cells[2, 2].Style.Font.SetFromFont("Times New Roman", 14);

            sheet.Cells[3, 2].Value = $"Прошу выдать денежные средства на выплату дохода инвесторам за {start.Date.ToString("dd-MM-yyyy")},";
            sheet.Cells[3, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.Cells[3, 2, 3, 4].Merge = true;
            sheet.Cells[3, 2, 3, 4].Style.Font.SetFromFont("Times New Roman", 14);

            sheet.Cells[4, 2].Value = "в сумме:";
            sheet.Cells[4, 2].Style.Font.SetFromFont("Times New Roman", 14);

            sheet.Cells[5, 2].Value = $"Общая - {sum_all_day}";
            sheet.Cells[5, 2].Style.Font.Size = 14;
            sheet.Cells[5, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
            sheet.Cells[5, 2].Style.Fill.BackgroundColor.SetColor(ExcelIndexedColor.Indexed13);

            sheet.Cells[7, 2].Value = "ФИО";
            sheet.Cells[7, 3].Value = "СУММА";
            sheet.Cells[7, 4].Value = "ФОРМА ОПЛАТЫ";
            sheet.Cells[7, 2, 7, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            sheet.Cells[7, 2, 7, 4].Style.Font.Bold = true;

            sheet.Cells[8, 2].Value = $"{start.Date.Day} число";
            sheet.Cells[8, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.Cells[8, 2].Style.Font.Bold = true;
            sheet.Cells[8, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
            sheet.Cells[8, 2].Style.Fill.BackgroundColor.SetColor(ExcelIndexedColor.Indexed13);

            int row = 9;
            int column = 2;

            double sum = 0;

            foreach (Models.Contract contract in contracts)
            {
                sheet.Cells[row, column].Value = $"{contract.Investor.LastName} {contract.Investor.FirstName} {contract.Investor.MiddleName}";
                sheet.Cells[row, column + 1].Value = Convert.ToDouble(contract.Amount) * Convert.ToDouble(contract.Bet) / 100.0;
                sheet.Cells[row, column + 2].Value = "Наличный";

                sum += Convert.ToDouble(contract.Amount) * Convert.ToDouble(contract.Bet) / 100.0;
                row++;
            }

            sheet.Cells[9, 2, row, column + 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            sheet.Cells[row + 1, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.Cells[row + 1, 3].Value = sum.ToString();
            sheet.Cells[9, 2, row, 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
            sheet.Cells[9, 2, row, 4].Style.Fill.BackgroundColor.SetColor(ExcelIndexedColor.Indexed13);

            //table grid
            sheet.Cells[7, 2, row + 1, 4].Style.Border.BorderAround(ExcelBorderStyle.Hair);
            createTableBorder(sheet.Cells[7, 2, row + 1, 4], "Thin");

            sheet.Cells[row + 6, 2].Value = "Заместитель руководителя";
            sheet.Cells[row + 6, 4].Value = "Судаков Ю.С.";
            sheet.Cells[row + 6, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            sheet.Cells[row + 6, 2, row + 6, 4].Style.Font.SetFromFont("Times New Roman", 14);
            return package.GetAsByteArray();
        }

        public static byte[] GeneratePayments(Models.Contract cntrs)
        {
            ExcelPackage package = new ExcelPackage();
            Models.Contract contract = cntrs;

            ExcelWorksheet sheet = package.Workbook.Worksheets.Add("Акт выплат");

            sheet.Row(1).Height = 56;
            sheet.Row(2).Height = 13;
            sheet.Row(3).Height = 14;
            sheet.Row(4).Height = 13;
            sheet.Row(5).Height = 23;
            sheet.Row(6).Height = 23;
            sheet.Row(7).Height = 12;
            sheet.Row(8).Height = 13;
            sheet.Row(9).Height = 16;
            sheet.Row(10).Height = 23;
            sheet.Row(11).Height = 23;
            sheet.Row(12).Height = 35;
            sheet.Row(13).Height = 15;
            sheet.Row(14).Height = 22;
            sheet.Row(15).Height = 16;
            sheet.Row(16).Height = 15;
            sheet.Row(17).Height = 15;
            sheet.Row(18).Height = 15;
            sheet.Column(1).Width = 12;
            sheet.Column(2).Width = 15;
            sheet.Column(3).Width = 15;
            sheet.Column(4).Width = 15;
            sheet.Column(5).Width = 15;
            sheet.Column(6).Width = 11;
            sheet.Column(7).Width = 11;

            int mounthCount = Math.Abs((contract.DateStart.Month - contract.DateEnd.Month) + 12 * (contract.DateStart.Year - contract.DateEnd.Year));

            sheet.Cells[4, 1].Value = "Приложение № 2";
            sheet.Cells[4, 1].Style.Font.Size = 10;
            sheet.Cells[4, 1].Style.Font.Bold = true;

            sheet.Cells[5, 1].Value = $"Начисление дохода согласно Договора инвестирования (Все про договор) от {contract.DateStart.ToString("dd-MM-yyyy")}";
            sheet.Cells[5, 1].Style.Font.SetFromFont("Verdana", 9);
            sheet.Cells[5, 1, 5, 7].Merge = true;
            sheet.Cells[5, 1, 5, 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
            sheet.Cells[5, 1, 5, 7].Style.Fill.BackgroundColor.SetColor(ExcelIndexedColor.Indexed13);

            sheet.Cells[9, 2].Value = "Инвестиции";
            sheet.Cells[9, 2].Style.Font.SetFromFont("Verdana", 10);
            sheet.Cells[9, 2, 9, 7].Merge = true;
            createTableBorder(sheet.Cells[9, 1, 9, 2], "Medium");

            sheet.Cells[10, 1, 14, 1].Style.WrapText = true;
            sheet.Cells[10, 1, 18, 7].Style.Font.SetFromFont("Verdana", 8);

            sheet.Cells[10, 1].RichText.Add("Сумма" + "\r\n");
            sheet.Cells[10, 1].RichText.Add("инвестиции:");
            sheet.Cells[10, 2].Value = contract.Amount.ToString("### ### ### руб.");

            sheet.Cells[11, 1].RichText.Add("Кол-во" + "\r\n");
            sheet.Cells[11, 1].RichText.Add("месяцев:");
            sheet.Cells[11, 2].Value = $"{mounthCount}";

            sheet.Cells[12, 1].RichText.Add("Процентная" + "\r\n");
            sheet.Cells[12, 1].RichText.Add("ставка в:" + "\r\n");
            sheet.Cells[12, 1].RichText.Add("месяц");
            sheet.Cells[12, 2].Value = contract.Bet.ToString("F2") + "%";

            sheet.Cells[13, 1].Value = "Дата:";
            sheet.Cells[13, 2].Value = contract.DateStart.ToString("dd.MM.yyyy");
            sheet.Cells[13, 2].Style.Numberformat.Format = "dd.MM.yyyy";

            sheet.Cells[14, 1].RichText.Add("ФИО" + "\r\n");
            sheet.Cells[14, 1].RichText.Add("Инвестора:");
            sheet.Cells[14, 2].Value = $"{contract.Investor.LastName} {contract.Investor.FirstName} {contract.Investor.MiddleName}";
            sheet.Cells[14, 2, 14, 7].Merge = true;

            sheet.Cells[14, 2, 14, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.Cells[10, 1, 14, 7].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            sheet.Cells[10, 7, 13, 7].Style.Border.Right.Style = ExcelBorderStyle.Medium;
            createTableBorder(sheet.Cells[9, 1, 9, 7], "Medium");
            createTableBorder(sheet.Cells[10, 1, 13, 2], "Medium");
            createTableBorder(sheet.Cells[14, 1, 15, 7], "Medium");

            sheet.Cells[10, 1, 13, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            sheet.Cells[10, 1, 13, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            sheet.Cells[16, 1, 18, 7].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            sheet.Cells[16, 1, 18, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.Cells[16, 1, 18, 7].Style.WrapText = true;

            sheet.Cells[16, 1].RichText.Add("Дата" + "\r\n");
            sheet.Cells[16, 1].RichText.Add("выплаты");
            sheet.Cells[16, 1, 18, 1].Merge = true;

            sheet.Cells[16, 2].RichText.Add("Возврат суммы" + "\r\n");
            sheet.Cells[16, 2].RichText.Add("инвестиции");
            sheet.Cells[16, 2, 18, 2].Merge = true;

            sheet.Cells[16, 3].RichText.Add("Остаток суммы" + "\r\n");
            sheet.Cells[16, 3].RichText.Add("инвестиции");
            sheet.Cells[16, 3, 18, 3].Merge = true;

            sheet.Cells[16, 4].RichText.Add("Проценты на" + "\r\n");
            sheet.Cells[16, 4].RichText.Add("инвестиции");
            sheet.Cells[16, 4, 18, 4].Merge = true;

            sheet.Cells[16, 5].Value = "Итого к выплате";
            sheet.Cells[16, 5, 18, 5].Merge = true;

            sheet.Cells[16, 6].RichText.Add("Дата" + "\r\n");
            sheet.Cells[16, 6].RichText.Add("выплаты");
            sheet.Cells[16, 6, 18, 6].Merge = true;

            sheet.Cells[16, 7].Value = "Подпись";
            sheet.Cells[16, 7, 18, 7].Merge = true;

            sheet.Cells[16, 1, 18, 7].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            sheet.Cells[16, 1, 18, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            int row = 20;
            int column = 1;

            double sum = 0;

            for (int month = 0; month <= mounthCount - 1; month++)
            {
                sheet.Column(row).Width = 16;

                sheet.Cells[row, column].Value = contract.DateStart.AddMonths(month + 1).ToString("dd.MM.yyyy");

                sheet.Cells[row, column + 1].Value = " -  руб";

                sheet.Cells[row, column + 2].Value = contract.Amount.ToString("### ### ### руб.");

                string monthPayment = contract.ContractType.Formula.Replace("x", (Convert.ToDouble(contract.Bet) / 100).ToString()).Replace("p", contract.Amount.ToString()).Replace("m", mounthCount.ToString());
                double result = Convert.ToDouble(new DataTable().Compute(monthPayment.Replace(",", "."), null));
                sheet.Cells[row, column + 3].Value = result.ToString("### ### ### руб.");

                sheet.Cells[row, 1, row, 7].Style.Font.SetFromFont("Verdana", 8);

                if ((month + 1) % 3 == 0)
                {
                    sheet.Cells[row, 2, row, 4].Style.Font.Bold = true;
                }

                sum += result;

                row++;
            }

            sheet.Cells[19, 1, row, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            sheet.Cells[row, 1].Value = "Итого:";

            sheet.Cells[row, column + 1].Value = " -  руб";

            sheet.Cells[row, column + 2].Value = contract.Amount.ToString("### ### ### руб.");

            sheet.Cells[row, column + 3].Value = sum.ToString("### ### ### руб.");

            sheet.Cells[row, 2, row, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            sheet.Column(row).Width = 23;
            sheet.Cells[row, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
            sheet.Cells[row, 1, row, 7].Style.Font.SetFromFont("Verdana", 8);
            sheet.Cells[row, 1, row, 4].Style.Font.Bold = true;

            createTableBorder(sheet.Cells[15, 1, row, 7], "Medium");

            sheet.Cells[row + 3, 1].Value = "ИНВЕСТОР  _____________________________________________________________     _________________";
            sheet.Cells[row + 3, 1, row + 3, 6].Merge = true;
            sheet.Cells[row + 3, 1, row + 3, 6].Style.Font.Size = 10;

            return package.GetAsByteArray();
        }

        public static byte[] MonthlyContract(ObservableCollection<Models.Contract> cntrs, DateTime start, DateTime end, ApplicationDbContext context)
        {
            ExcelPackage package = new ExcelPackage();
            ObservableCollection<Models.Contract> contracts = cntrs;

            ApplicationDbContext _context = context;

            contracts = new ObservableCollection<Models.Contract>(contracts.OrderByDescending(x => x.DateStart).Reverse());

            ExcelWorksheet sheet = package.Workbook.Worksheets.Add("Ежемесячная ведомость по договорам");

            sheet.Cells[2, 1].Value = "Договор №";
            sheet.Cells[2, 2].Value = "Дата";
            sheet.Cells[2, 3].Value = "ФИО";
            sheet.Cells[2, 4].Value = "Дата рожд.";
            sheet.Cells[2, 5].Value = "Телефон";
            sheet.Cells[2, 6].Value = "Кто привел";
            sheet.Cells[2, 7].Value = "Сумма";
            sheet.Cells[2, 8].Value = "% ставка";
            sheet.Cells[2, 9].Value = "Срок";
            sheet.Cells[2, 10].Value = "Форма оплаты";
            sheet.Cells[2, 11].Value = "Номер карты";
            sheet.Cells[2, 12].Value = "Сумма доб.";

            sheet.Cells[2, 1, 2, 12].AutoFitColumns();

            sheet.Cells[2, 1, 2, 12].Style.Font.Bold = true;

            int row = 3;
            int column = 1;
            DateTime currentDate = new DateTime();

            foreach (Models.Contract contract in contracts)
            {
                if (currentDate != contract.DateStart)
                {
                    currentDate = contract.DateStart;

                    sheet.Cells[row, column].Value = currentDate.ToString("dd.MM.yyyy");
                    sheet.Cells[row, column, row, 12].Merge = true;
                    sheet.Cells[row, column, row, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    sheet.Cells[row, column, row, 12].Style.Font.Bold = true;

                    row++;
                }

                sheet.Cells[row, column].Value = contract.CustomId;

                sheet.Cells[row, column + 2].Value = $"{contract.Investor.LastName} {contract.Investor.FirstName} {contract.Investor.MiddleName}";

                sheet.Cells[row, column + 3].Value = contract.Investor.DateBirth.ToString("dd.MM.yyyy");

                sheet.Cells[row, column + 4].Value = contract.Investor.Phone.ToString();

                if (contract.Referral != null)
                {
                    sheet.Cells[row, column + 5].Value = $"{contract.Referral.InvestorCalled.LastName} {contract.Referral.InvestorCalled.FirstName[0]}. {contract.Referral.InvestorCalled.LastName[0]}.";
                }

                sheet.Cells[row, column + 6].Value = contract.Amount.ToString("### ### ###");

                sheet.Cells[row, column + 7].Value = contract.Bet;

                if (contract.DateProlonagtion.ToString("dd.MM.yyyy") != contract.DateStart.ToString("dd.MM.yyyy"))
                {
                    sheet.Cells[row, column + 8].Value = Math.Abs((contract.DateStart.Month - contract.DateEnd.Month) + 12 * (contract.DateStart.Year - contract.DateEnd.Year));
                }
                else
                {
                    sheet.Cells[row, column + 8].Value = Math.Abs((contract.DateProlonagtion.Month - contract.DateEnd.Month) + 12 * (contract.DateProlonagtion.Year - contract.DateEnd.Year));
                }

                sheet.Cells[row, column + 9].Value = new Converters.BoolToPayTypeStringConverter().Convert(contract.Investor.PayType, typeof(string), "", CultureInfo.CurrentCulture);

                if (contract.Repeated)
                {
                    AdditionalPayment payment = _context.AdditionalPayments.FirstOrDefault(x => x.Contract == contract && x.Date.Day == contract.DateProlonagtion.Day);
                    if (payment != null)
                    {
                        sheet.Cells[row, column + 10].Value = "Перез. с доб.";

                        sheet.Cells[row, column + 11].Value = payment.Amount.ToString("### ### ###");
                    }
                    else
                    {
                        sheet.Cells[row, column + 10].Value = "Перезаключение";
                    }
                }
                else if(contract.RepeatNumber != 0)
                {
                   string[] ArrayNumberOfContracts = new string[7] { "второй", "третий", "четвертый", "пятый", "шестой", "седьмой", "восьмой" };
                   sheet.Cells[row, column + 10].Value = $"{ArrayNumberOfContracts[contract.RepeatNumber - 1]} договор";
                }
                else
                {
                    sheet.Cells[row, column + 10].Value = contract.Investor.CardNumber;
                }

                row++;
            }

            createTableBorder(sheet.Cells[2, 1, row - 1, 12], "Thin");
            sheet.Cells[2, 1, row - 1, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            sheet.Cells[2, 1, row - 1, 12].Style.Font.Size = 10;

            return package.GetAsByteArray();
        }

        public static byte[] ReferalBonus(ObservableCollection<Models.Contract> cntrs)//, DateTime start, DateTime end)
        {
            ExcelPackage package = new ExcelPackage();
            ObservableCollection<Models.Contract> contracts = cntrs;

            ExcelWorksheet sheet = package.Workbook.Worksheets.Add("Реферальная ведомость");

            contracts = new ObservableCollection<Models.Contract>(contracts.OrderByDescending(x => x.DateStart).Reverse());

            sheet.Row(1).Height = 15;
            sheet.Row(2).Height = 20;
            sheet.Row(3).Height = 15;
            sheet.Row(4).Height = 48;
            sheet.Column(1).Width = 14;
            sheet.Column(2).Width = 24;
            sheet.Column(3).Width = 14;
            sheet.Column(4).Width = 15;
            sheet.Column(5).Width = 15;
            sheet.Column(6).Width = 24;
            sheet.Column(7).Width = 15;
            sheet.Column(8).Width = 16;
            sheet.Column(9).Width = 30;

            sheet.Cells[2, 1].Value = "Ведомость по реферальному бонусу за {Период}";
            sheet.Cells[2, 1].Style.Font.SetFromFont("Times New Roman", 12);
            sheet.Cells[2, 1].Style.Font.Bold = true;
            sheet.Cells[2, 1].Style.Font.UnderLine = true;
            sheet.Cells[2, 1, 2, 8].Merge = true;
            sheet.Cells[2, 1, 2, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            ExcelRichText ert = sheet.Cells[4, 1].RichText.Add("Дата\r\nзаключения\r\nдоговора");
            ert.Bold = true;

            sheet.Cells[4, 2].Value = "ФИО инвестора";

            sheet.Cells[4, 3].Value = "Сумма";

            ert = sheet.Cells[4, 4].RichText.Add("Наличные/\r\nбезналичные");
            ert.Bold = true;

            sheet.Cells[4, 5].IsRichText = true;
            ert = sheet.Cells[4, 5].RichText.Add("Первый/\r\nповторный\r\nдоговор");
            ert.Bold = true;

            sheet.Cells[4, 6].Value = "От кого";

            ert = sheet.Cells[4, 7].RichText.Add("Реферальный\r\nбонус 3%");
            ert.Bold = true;

            sheet.Cells[4, 8].Value = "Подпись";

            sheet.Cells[4, 9].Value = "Примечание";

            sheet.Cells[4, 1, 4, 9].Style.Font.SetFromFont("Times New Roman", 12);
            sheet.Cells[4, 1, 4, 9].Style.Font.Bold = true;
            sheet.Cells[4, 1, 4, 9].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            sheet.Cells[4, 1, 4, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            int row = 5;
            int column = 1;

            foreach (Models.Contract contract in contracts)
            {
                if(contract.Referral == null) continue;
                sheet.Cells[row, column].Value = contract.DateStart.ToString("dd.MM.yyyy");

                sheet.Cells[row, column + 1].Style.WrapText = true;
                sheet.Cells[row, column + 1].RichText.Add(contract.Investor.LastName + " " + contract.Investor.FirstName + "\r\n");
                sheet.Cells[row, column + 1].RichText.Add(contract.Investor.MiddleName);

                sheet.Cells[row, column + 2].Value = contract.Amount.ToString("### ### ### руб.");

                sheet.Cells[row, column + 3].Value = new Converters.BoolToPayTypeStringConverter().Convert(contract.Investor.PayType, typeof(string), "", CultureInfo.CurrentCulture);

                sheet.Cells[row, column + 4].Value = new Converters.ContractRepeatedConverter().Convert(contract.Repeated, typeof(string), "", CultureInfo.CurrentCulture);

                sheet.Cells[row, column + 5].Style.WrapText = true;
                sheet.Cells[row, column + 5].RichText.Add(contract.Referral.InvestorCalled.LastName + " " + contract.Referral.InvestorCalled.FirstName + "\r\n");
                sheet.Cells[row, column + 5].RichText.Add(contract.Referral.InvestorCalled.MiddleName);

                if (contract.Repeated == true)
                {
                    sheet.Cells[row, column + 6].Value = 0.ToString("### ### ### руб.");
                }
                else
                {
                    sheet.Cells[row, column + 6].Value = (Convert.ToDouble(contract.Referral.Bonus) / 100.0 * Convert.ToDouble(contract.Amount)).ToString("### ### ### руб.");
                }

                sheet.Cells[row, column + 8].Value = contract.Referral.Note;

                sheet.Cells[row, 1, row, 9].Style.Font.SetFromFont("Times New Roman", 11);
                sheet.Cells[row, 1, row, 9].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                sheet.Cells[row, 1, row, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                row++;
            }

            createTableBorder(sheet.Cells[4, 1, row - 1, 9], "Thin");

            return package.GetAsByteArray();
        }

        private static void createTableBorder(ExcelRange modelTable, string borderStyle)
        {
            switch (borderStyle)
            {
                case "Medium":
                    {
                        modelTable.Style.Border.Top.Style = ExcelBorderStyle.Medium;
                        modelTable.Style.Border.Left.Style = ExcelBorderStyle.Medium;
                        modelTable.Style.Border.Right.Style = ExcelBorderStyle.Medium;
                        modelTable.Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                        break;
                    }
                case "Thin":
                    {
                        modelTable.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        modelTable.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        modelTable.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        modelTable.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        break;
                    }
            }
        }
    }
}
