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
    public class InvestorsViewModel : ViewModelBase
    {
        private ApplicationDbContext _context;
        private bool _isPaneOpen;

        public InvestorsViewModel()
        {
            _context = new ApplicationDbContext(new DbContextOptions<ApplicationDbContext>());
        }

        public InvestorsViewModel(IScreen hostScreen, ApplicationDbContext context)
        {
            _context = context;
            _isPaneOpen = false;
            _context.Investors.Load();

            PaneCommand = ReactiveCommand.Create(() =>
            {
                IsPaneOpen = !IsPaneOpen;
            });

            NewInvestorCommand = ReactiveCommand.Create(() =>
            {
                hostScreen.Router.Navigate.Execute(new AddInvestorViewModel(hostScreen, context));
            });

            DataGridEditActionCommand = ReactiveCommand.Create((Investor inv) =>
            {
                Debug.WriteLine(inv.ToString());
            });


            DataGridEditActionCommand = ReactiveCommand.Create((Investor inv) =>
            {
                Debug.WriteLine(inv.ToString());
            });

            DataGridRemoveActionCommand = ReactiveCommand.Create((Investor inv) =>
            {
                Debug.WriteLine(inv.Id.ToString() + "to remove");
                _context.Investors.Remove(inv);
                _context.SaveChangesAsync();
                _context.Investors.LoadAsync();
            });
        }

        public ObservableCollection<Investor> Investors => _context.Investors.Local.ToObservableCollection();

        public override IScreen HostScreen => throw new NotImplementedException();
        public bool IsPaneOpen
        {
            get => _isPaneOpen;
            set => this.RaiseAndSetIfChanged(ref _isPaneOpen, value);
        }

        public ReactiveCommand<Unit, Unit> PaneCommand { get;}
        public ReactiveCommand<Unit, Unit> NewInvestorCommand { get; }
        public ReactiveCommand<Investor, Unit> DataGridPrintActionCommand { get; set; }
        public ReactiveCommand<Investor, Unit> DataGridEditActionCommand { get; set; }
        public ReactiveCommand<Investor, Unit> DataGridRemoveActionCommand { get; set; }
    }
}
