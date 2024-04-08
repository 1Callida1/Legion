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
        }
        public static void GenerateDocument(Models.Contract cntr, string documentType, Models.Contract cntrProlongation = null, AdditionalPayment additionalPayment = null)
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
                BetWords = RussianConverter.FormatCurrency((decimal)contract.Bet),
                MonthPayment = (contract.Amount * contract.Bet / 100).ToString("### ### ###"),
                MonthPaymentWords = RussianConverter.FormatCurrency((decimal)(contract.Amount * contract.Bet / 100)),
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
                ContractTypeName = contract.ContractType.TypeName
            };
            if(additionalPayment != null)
            {
                contractOptimazed.AdditionalPaymentSum = additionalPayment.Amount.ToString("### ### ###");
                contractOptimazed.AdditionalPaymentSumWords = RussianConverter.Format(additionalPayment.Amount);
                contractOptimazed.AdditionalPaymentDate = $"«{additionalPayment.Date.Day}» {additionalPayment.Date.ToString("MMMM", culture)} {additionalPayment.Date.Year}";
            }
            if (cntrProlongation != null) contractOptimazed.ProlongationDate = $"«{cntrProlongation.DateEnd.Day}» {cntrProlongation.DateEnd.ToString("MMMM", culture)} {cntrProlongation.DateEnd.Year}";

            var template = DocxTemplate.Open($"{Locator.Current.GetService<Settings>().TemplatesFolder}/Шаблон {documentType}.docx");
            template.BindModel("ds", contractOptimazed);

            string TemplateName = "";
            switch (documentType)
            {
                case "Закрытие договора":
                {
                    TemplateName = "Заявление на закрытие договора инвестирования";
                    break;
                }
                case "Акт":
                {
                    TemplateName = $"Акт {contractOptimazed.ContractId.Replace("/", ".")} {contractOptimazed.InvestorNameShort}.";
                    break;
                }
                case "Доп соглашение":
                {
                    TemplateName = $"Доп соглашение от {contractOptimazed.DateStart}";
                    break;
                }
                case "Пролонгация":
                {
                    TemplateName = $"Доп соглашение к договору от {contractOptimazed.DateStart} {contractOptimazed.InvestorNameShort}.";
                    break;
                }
                case "Договор инвестирования":
                {
                    TemplateName = $"Договор инвестирования {contractOptimazed.InvestorNameShort}.";
                    break;
                }
                case "Договор накопительный":
                {
                    TemplateName = $"Договор накопительный {contractOptimazed.ContractId.Replace("/", ".")} {contractOptimazed.InvestorNameShort}.";
                    break;
                }
            }

            string subPath = $"{Locator.Current.GetService<Settings>().ArchievFolder}/{contractOptimazed.ContractTypeName} {contractOptimazed.ContractId.Replace("/", ".")} {contractOptimazed.InvestorNameShort}";

            bool exists = Directory.Exists(subPath);

            if (!exists)
                Directory.CreateDirectory(subPath);


            template.Save($"{subPath}/{TemplateName}.docx");
        }
        public Settings Settings => Locator.Current.GetService<Settings>()!;
    }
}