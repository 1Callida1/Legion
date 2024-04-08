using Avalonia;
using Legion.Models;
using Legion.ViewModels;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Splat;
using Legion.Helpers.ReportGenerator;
using System.IO;
using System.Diagnostics.Contracts;
using Avalonia.Controls.Shapes;
using Legion.Helpers.Calculations;
using Legion.Models.Internal;

namespace Legion.ViewModels
{
    public class ReportsViewModel : ViewModelBase
    {
        private readonly ApplicationDbContext _context;
        private ObservableCollection<Investor> _investors = null!;
        private ObservableCollection<Models.Contract> _contracts;
        private bool _datePickerVisible = false;
        private int _dateOffsetVariant = 0;
        private DateTimeOffset _startDateTime = new(DateTime.Now);
        private DateTimeOffset _endDateTime = new(DateTime.Now);

        public ReportsViewModel(ApplicationDbContext context, IScreen? hostScreen = null)
        {
            _context = context;
            HostScreen = hostScreen ?? Locator.Current.GetService<IScreen>()!;
            Contracts = new ObservableCollection<Models.Contract>(_context.Contracts.ToList());
            Settings settings = Locator.Current.GetService<Settings>()!;
            string outputPath = $"{settings.ArchievFolder}/Отчеты";
            if(!Directory.Exists(outputPath))
                Directory.CreateDirectory(outputPath);

            BackCommand = ReactiveCommand.Create(() =>
            {
                HostScreen.Router.NavigateBack.Execute();
            });

            GetEveryDayReportCommand = ReactiveCommand.Create(() =>
            {
                byte[] reportExcel = ExcelGenerator.GenerateReportCash(Contracts, DateTime.Now); //Дата из промежутка, добавить райз ошибки если выбрана дата больше чем один день
                File.WriteAllBytes($"{outputPath}/доход за  {DateTime.Now:dd.MM.yyyy} безналичные.xlsx", reportExcel);
                reportExcel = ExcelGenerator.GenerateReportCashless(Contracts, DateTime.Now);
                File.WriteAllBytes($"{outputPath}/доход за  {DateTime.Now:dd.MM.yyyy} наличные.xlsx", reportExcel);
            });

            GetRefferalReportCommand = ReactiveCommand.Create(() =>
            {
                byte[] reportExcel = ExcelGenerator.ReferalBonus(Contracts);
                File.WriteAllBytes($"{outputPath}/Ведомость по реферальному бонусу.xlsx", reportExcel);
            });

            GetContractsReportCommand = ReactiveCommand.Create(() =>
            {
                byte[] reportExcel = ExcelGenerator.MonthlyContract(Contracts, DateTime.Now, DateTime.Now.AddMonths(1), _context);
                File.WriteAllBytes($"{outputPath}/ежемесячная ведомость по договорам.xlsx", reportExcel);
            });
        }
        public sealed override IScreen HostScreen { get; set; }
        public ReactiveCommand<Unit, Unit> BackCommand { get; } = null!;
        public ReactiveCommand<Unit, Unit> GetEveryDayReportCommand { get; } = null!;
        public ReactiveCommand<Unit, Unit> GetRefferalReportCommand { get; } = null!;
        public ReactiveCommand<Unit, Unit> GetContractsReportCommand { get; } = null!;


        public ObservableCollection<Models.Contract> Contracts
        {
            get => _contracts;
            set => this.RaiseAndSetIfChanged(ref _contracts, value);
        }

        public int DateOffsetVariant
        {
            // 0 - за день
            // 1 - месяц
            // 2 - кастом
            get => _dateOffsetVariant;
            set
            {
                DatePickerVisible = value == 2;
                this.RaiseAndSetIfChanged(ref _dateOffsetVariant, value);
            }
        }

        public bool DatePickerVisible
        {
            get => _datePickerVisible;
            set => this.RaiseAndSetIfChanged(ref _datePickerVisible, value);
        }

        public DateTimeOffset StartDateTime
        {
            get => _startDateTime;
            set
            {
                if (value == null)
                    return;

                this.RaiseAndSetIfChanged(ref _startDateTime, value);
            }
        }

        public DateTimeOffset EndDateTime
        {
            get => _endDateTime;
            set
            {
                if (value == null)
                    return;

                this.RaiseAndSetIfChanged(ref _endDateTime, value);
            }
        }

        public ObservableCollection<Investor> Investors
        {
            get => _investors;
            set => this.RaiseAndSetIfChanged(ref _investors, value);
        }
    }
}
