using Legion.Models.Internal;
using SkiaSharp;
using Splat;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Legion.Helpers
{
    internal class PathHelper
    {
        public static string generatePath(Models.Contract contract)
        {
            string subPath = $"{Settings.ArchievFolder}";

            switch (contract.ContractType.TypeName)
            {
                case "Накопительный Е":
                    subPath += "/Договор накопительный Е";
                    break;
                case "Накопительный":
                    subPath += "/Договор накопительный";
                    break;
                case "Инвестиционный":
                    subPath += "/Договор инвестирования 12";
                    break;
                case "Трехгодовой":
                    subPath += "/Договор инвестирования 36";
                    break;
                case "Доходный":
                    subPath += "/Договор доходный";
                    break;
                case "ТАНАКА инвестиционный":
                    subPath += "/Договор инвестирования ТАНАКА";
                    break;
                case "ТАНАКА накопительный":
                    subPath += "/Договор накопительный ТАНАКА";
                    break;
            }

            if (contract.ContractType.TypeName.Contains("ТАНАКА"))
            {
                subPath += $"/Договор МКК {contract.CustomId.Replace("/", ".")} " +
                $"{contract.Investor.LastName} {contract.Investor.FirstName[0]}. {contract.Investor.MiddleName[0]}";
            }
            else if (contract.ContractType.TypeName.Contains("Накопительный"))
            {
                subPath += $"/Договор Накопительный {contract.CustomId.Replace("/", ".")} " +
                $"{contract.Investor.LastName} {contract.Investor.FirstName[0]}. {contract.Investor.MiddleName[0]}";
            }
            else
            {
                subPath += $"/Договор {contract.CustomId.Replace("/", ".")} " +
                $"{contract.Investor.LastName} {contract.Investor.FirstName[0]}. {contract.Investor.MiddleName[0]}";
            }

            bool exists = Directory.Exists(subPath);

            if (!exists)
                Directory.CreateDirectory(subPath);

            return subPath;
        }

        public static Settings Settings => Locator.Current.GetService<Settings>()!;
    }
}
