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

            var reportExcel = new ExcelGenerator().ReferalBonus(Contracts);
            File.WriteAllBytes("reportExcel.xlsx", reportExcel);

            BackCommand = ReactiveCommand.Create(() =>
            {
                HostScreen.Router.NavigateBack.Execute();
            });
        }
        public sealed override IScreen HostScreen { get; set; }
        public ReactiveCommand<Unit, Unit> BackCommand { get; } = null!;


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
