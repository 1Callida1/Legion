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

namespace Legion.ViewModels
{
    public class InvestorSerachViewModel : ViewModelBase
    {
        private ApplicationDbContext _context;

        public InvestorSerachViewModel()
        {
            _context = new ApplicationDbContext(new DbContextOptions<ApplicationDbContext>());
        }

        public InvestorSerachViewModel(ApplicationDbContext context, IScreen? hostScreen = null)
        {
            HostScreen = hostScreen ?? Locator.Current.GetService<IScreen>()!;
            _context = context;
            _context.Investors.Load();

            BackCommand = ReactiveCommand.Create(() =>
            {
                HostScreen.Router.NavigateBack.Execute();
            });
        }

        public ObservableCollection<Investor> Investors => _context.Investors.Local.ToObservableCollection();
        public ReactiveCommand<Unit, Unit> BackCommand { get; }

        public sealed override IScreen HostScreen { get; set; }
    }
}
