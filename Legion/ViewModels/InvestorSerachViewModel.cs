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

namespace Legion.ViewModels
{
    public class InvestorSerachViewModel : ViewModelBase
    {
        private ApplicationDbContext _context;

        public InvestorSerachViewModel()
        {
            _context = new ApplicationDbContext(new DbContextOptions<ApplicationDbContext>());
        }

        public InvestorSerachViewModel(IScreen hostScreen, ApplicationDbContext context)
        {
            _context = context;
            _context.Investors.Load();
        }

        public ObservableCollection<Investor> Investors => _context.Investors.Local.ToObservableCollection();

        public override IScreen HostScreen => throw new NotImplementedException();
    }
}
