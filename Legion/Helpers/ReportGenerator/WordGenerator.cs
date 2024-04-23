using Avalonia.Markup.Xaml.Templates;
using DocxTemplater;
using GroupDocs.Editor;
using GroupDocs.Editor.Formats;
using GroupDocs.Editor.HtmlCss.Resources;
using GroupDocs.Editor.Options;
using Legion.Helpers.Calculations;
using Legion.Models;
using Microsoft.Extensions.Options;
using NickBuhro.NumToWords.Russian;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Legion.Models.Internal;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using DocumentFormat.OpenXml.Drawing;

namespace Legion.Helpers.ReportGenerator
{
    internal class WordGenerator
    {
        private class ContractOptimazed
        {
            public string ContractId { get; set; }
            public string DateStartFormat { get; set; }
            public string DateStart {  get; set; }
            public string InvestorNameFull { get; set;}
            public string InvestorNameShort { get; set; }
            public string Amount {  get; set; }
            public string AmountWords { get; set;}
            public string DateStartFormatBrackets { get; set; }
            public string DateEndFormatBrackets { get;set; }
            public string DayPayment { get; set; }
            public string Bet { get; set; }
            public string BetWords { get; set; }
            public string MonthPayment { get; set; }
            public string MonthPaymentWords { get; set; }
            public string Dob { get; set; }
            public string PassprotSeries { get; set; } = null;
            public string PassprotNumber { get; set; } = null;
            public string PassprotGiven { get;set; } = null;
            public string PassportDateGiven { get;set; } = null;
            public string PassportUnitCode { get;set; } = null;
            public string PassportRegistration { get; set; } = null;
            public string Phone { get;set; }
            public string DateProlongation { get; set; }
            public string ProlongationDate { get; set; } = null;
            public string AdditionalPaymentSum { get; set; } = null;
            public string AdditionalPaymentSumWords { get; set; } = null;
            public string AdditionalPaymentDate { get; set; } = null;
            public string ContractTypeName { get; set; }
            public string BetYear { get; set; }
            public List<CustomBet> CustomBet { get; set; }
        }
        private class CustomBet
        {
            public string YearEnd { get; set; } = null;
            public string YearNotFormat { get; set; } = null;
            public string YearBet { get; set; } = null;
            public string sumYearBet { get; set; } = null;
            public string YearBetWords { get; set; } = null;
            public string YearMonthPayment { get; set; } = null;
            public string YearMonthPaymentWords { get; set; } = null;
        }
        public static void GenerateDocument(Models.Contract cntr, string documentType, string subPath, Models.Contract cntrProlongation = null, AdditionalPayment additionalPayment = null)
        {
            Legion.Models.Contract contract = cntr;

            CultureInfo culture = new CultureInfo("ru-RU");
            culture.DateTimeFormat.MonthNames = culture.DateTimeFormat.MonthGenitiveNames;
            culture.DateTimeFormat.AbbreviatedMonthNames = culture.DateTimeFormat.AbbreviatedMonthGenitiveNames;

            ContractOptimazed contractOptimazed = new ContractOptimazed {
                ContractId = contract.CustomId,
                DateStartFormat = $"{contract.DateStart.Day} {contract.DateStart.ToString("MMMM", culture)} {contract.DateStart.Year}",
                InvestorNameFull = $"{contract.Investor.LastName} {contract.Investor.FirstName} {contract.Investor.MiddleName}",
                InvestorNameShort = $"{contract.Investor.LastName} {contract.Investor.FirstName[0]}. {contract.Investor.MiddleName[0]}.",
                Amount = contract.Amount.ToString("### ### ###"),
                AmountWords = RussianConverter.Format(contract.Amount),
                DateStartFormatBrackets = $"«{contract.DateStart.Day}» {contract.DateStart.ToString("MMMM", culture)} {contract.DateStart.Year}",
                DateEndFormatBrackets = $"«{contract.DateEnd.Day}» {contract.DateEnd.ToString("MMMM", culture)} {contract.DateEnd.Year}",
                DayPayment = contract.DateEnd.Day.ToString(),
                Bet = contract.Bet.ToString(),
                BetWords = RussianConverter.FormatCurrency((decimal)contract.Bet)
                    .Replace("рублей ноль копеек процентов", "процентов")
                    .Replace("рубля пятьдесят копеек процентов", "целых пять десятых процентов")
                    .Replace("рубля ноль копеек процентов", "процентов")
                    .Replace("рублей пятьдесят копеек процентов", "целых пять десятых процентов")
                    .Replace("рублей ноль копеек", "процентов")
                    .Replace("рубля пятьдесят копеек", "целых пять десятых процентов")
                    .Replace("рубля ноль копеек", "процентов")
                    .Replace("пятьдесят копеек", "целых пять десятых процентов")
                    .Replace("рублей ", ""),
                MonthPayment = (contract.Amount * contract.Bet / 100).ToString("### ### ###"),
                MonthPaymentWords = RussianConverter.Format((long)(contract.Amount * contract.Bet / 100)),
                Dob = contract.Investor.DateBirth.ToString("dd.MM.yyyy"),
                PassprotSeries = contract.Investor.PassprotSeries,
                PassprotNumber = contract.Investor.PassprotNumber,
                PassprotGiven = contract.Investor.Given,
                PassportDateGiven = contract.Investor.PassportDateGiven.ToString("dd.MM.yyyy"),
                PassportUnitCode = contract.Investor.PassportUnitCode,
                PassportRegistration = contract.Investor.PassportRegistration,
                Phone = contract.Investor.Phone,
                DateProlongation = $"«{DateTime.Now.Day}» {DateTime.Now.ToString("MMMM", culture)} {DateTime.Now.Year}",
                DateStart = contract.DateStart.ToString("dd.MM.yyyy"),
                ContractTypeName = contract.ContractType.TypeName,
                BetYear = (contract.Bet * 12.0).ToString()
            };
            if(additionalPayment != null)
            {
                contractOptimazed.AdditionalPaymentSum = additionalPayment.Amount.ToString("### ### ###");
                contractOptimazed.AdditionalPaymentSumWords = RussianConverter.Format(additionalPayment.Amount);
                contractOptimazed.AdditionalPaymentDate = $"«{additionalPayment.Date.Day}» {additionalPayment.Date.ToString("MMMM", culture)} {additionalPayment.Date.Year}";
            }
            if (cntrProlongation != null) contractOptimazed.ProlongationDate = $"«{cntrProlongation.DateEnd.Day}» {cntrProlongation.DateEnd.ToString("MMMM", culture)} {cntrProlongation.DateEnd.Year}";

            if(contract.ContractType.NextYearBetCoef != 0)
            {
                int yearCount = Math.Abs((contract.DateStart.Month - contract.DateEnd.Month) + 12 * (contract.DateStart.Year - contract.DateEnd.Year)) / 12;
                List<CustomBet> customBets = new List<CustomBet>();
                for (int currentYear = 1; currentYear < yearCount + 1; currentYear++)
                {
                    CustomBet customBet = new CustomBet {
                        YearNotFormat = contract.DateStart.AddYears(currentYear).ToString("dd.MM.yyyy"),
                        sumYearBet = (contract.Bet + (contract.ContractType.NextYearBetCoef * currentYear) * 12.0).ToString(),
                        YearEnd = $"«{contract.DateStart.Day}» {contract.DateStart.ToString("MMMM", culture)} {Convert.ToInt32(contract.DateStart.Year) + currentYear}",
                        YearBet = (contract.Bet + (contract.ContractType.NextYearBetCoef * currentYear)).ToString(),
                        YearBetWords = RussianConverter.FormatCurrency((decimal)(contract.Bet + (contract.ContractType.NextYearBetCoef * currentYear)))
                            .Replace("рублей ноль копеек процентов", "процентов")
                            .Replace("рубля пятьдесят копеек процентов", "целых пять десятых процентов")
                            .Replace("рубля ноль копеек процентов", "процентов")
                            .Replace("рублей пятьдесят копеек процентов", "целых пять десятых процентов")
                            .Replace("рублей ноль копеек", "процентов")
                            .Replace("рубля пятьдесят копеек", "целых пять десятых процентов")
                            .Replace("рубля ноль копеек", "процентов")
                            .Replace("пятьдесят копеек", "целых пять десятых процентов")
                            .Replace("рублей ", ""),
                        YearMonthPayment = (contract.Amount * (contract.Bet + (contract.ContractType.NextYearBetCoef * currentYear)) / 100).ToString("### ### ###"),
                        YearMonthPaymentWords = RussianConverter.Format((long)(contract.Amount * (contract.Bet + (contract.ContractType.NextYearBetCoef * currentYear)) / 100))
                    };
                    customBets.Add(customBet);
                }
                contractOptimazed.CustomBet = customBets;
            }

            var template = DocxTemplate.Open($"{Locator.Current.GetService<Settings>().TemplatesFolder}/Шаблон {documentType}.docx");
            template.BindModel("ds", contractOptimazed);

            string TemplateName = "";
            switch (documentType)
            {
                case "Закрытие договора":
                    TemplateName = "Заявление на закрытие договора";
                    break;
                case "Приложение № 3":
                    TemplateName = $"Приложение № 3 №{contractOptimazed.ContractId.Replace("/", ".")} {contractOptimazed.InvestorNameShort}.";
                    break;
                case "Акт":
                    TemplateName = $"Акт №{contractOptimazed.ContractId.Replace("/", ".")} {contractOptimazed.InvestorNameShort}.";
                    break;
                case "Доп соглашение накопительный":
                    TemplateName = $"Доп соглашение от {contractOptimazed.DateStart}";
                    break;
                case "Доп соглашение пролонгация":
                    TemplateName = $"Доп. соглашение к договору {contractOptimazed.ContractId.Replace("/", ".")} от {contractOptimazed.DateStart} {contractOptimazed.InvestorNameShort}.";
                    break;
                case "Договор инвестирования 12":
                    TemplateName = $"Договор {contractOptimazed.ContractId.Replace("/", ".")} {contractOptimazed.InvestorNameShort}.";
                    break;
                case "Договор инвестирования 36":
                    TemplateName = $"Договор {contractOptimazed.ContractId.Replace("/", ".")} {contractOptimazed.InvestorNameShort}.";
                    break;
                case "Договор доходный":
                    TemplateName = $"Договор {contractOptimazed.ContractId.Replace("/", ".")} {contractOptimazed.InvestorNameShort}.";
                    break;
                case "Договор накопительный":
                    TemplateName = $"Договор накопительный {contractOptimazed.ContractId.Replace("/", ".")} {contractOptimazed.InvestorNameShort}.";
                    break;
                case "Договор накопительный Е":
                    TemplateName = $"Договор накопительный {contractOptimazed.ContractId.Replace("/", ".")} {contractOptimazed.InvestorNameShort}.";
                    break;
                case "Договор инвестирования ТАНАКА":
                    TemplateName = $"Договор МКК {contractOptimazed.ContractId.Replace("/", ".")} {contractOptimazed.InvestorNameShort}.";
                    break;
                case "Договор накопительный ТАНАКА":
                    TemplateName = $"Договор МКК {contractOptimazed.ContractId.Replace("/", ".")} {contractOptimazed.InvestorNameShort}.";
                    break;
            }

            template.Save($"{subPath}/{TemplateName}.docx");
        }
        public Settings Settings => Locator.Current.GetService<Settings>()!;
    }
}