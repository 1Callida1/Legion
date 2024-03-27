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

namespace Legion.ViewModels
{
    public class ReportsViewModel : ViewModelBase
    {
        private readonly ApplicationDbContext _context;
        private ObservableCollection<Investor> _investors = null!;

        public ReportsViewModel(ApplicationDbContext context, IScreen? hostScreen = null)
        {
            _context = context;
            HostScreen = hostScreen ?? Locator.Current.GetService<IScreen>()!;
            Investors = _context.Investors.Local.ToObservableCollection();

            var reportExcel = new ExcelGenerator().Generate(null, null, Investors);
            File.WriteAllBytes("reportExcel.xlsx", reportExcel);

        }
        public sealed override IScreen HostScreen { get; set; }

        public ObservableCollection<Investor> Investors
        {
            get => _investors;
            set => this.RaiseAndSetIfChanged(ref _investors, value);
        }
    }
}
