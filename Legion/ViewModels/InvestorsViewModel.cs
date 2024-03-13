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
using DynamicData.Binding;
using Microsoft.EntityFrameworkCore;
using System.Collections;

namespace Legion.ViewModels
{
    public class InvestorsViewModel : ViewModelBase
    {
        private ApplicationDbContext _context;
        private bool _isPaneOpen;
        private string _searchText;
        private ObservableCollection<Investor> _investors;

        public InvestorsViewModel()
        {
            _context = new ApplicationDbContext(new DbContextOptions<ApplicationDbContext>());
        }

        public InvestorsViewModel(IScreen hostScreen, ApplicationDbContext context)
        {
            _context = context;
            _isPaneOpen = false;
            _context.Investors.Load();
            Investors = _context.Investors.Local.ToObservableCollection();

            IsSearchTextExist = this.WhenAnyValue(
                x => x.SearchText,
                (text) =>
                    !string.IsNullOrWhiteSpace(text)
            );

            SearchCommand = ReactiveCommand.Create(() =>
            {
                Investors = new ObservableCollection<Investor>(_context.Investors
                    .Where(inv => (inv.LastName + inv.FirstName + inv.MiddleName).Contains(SearchText) || inv.DateBirth.ToString().Contains(SearchText)).ToList());
            }, IsSearchTextExist);

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
                hostScreen.Router.Navigate.Execute(new AddInvestorViewModel(inv, hostScreen, context));
            });


            DataGridPrintActionCommand = ReactiveCommand.Create((Investor inv) =>
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

        public ObservableCollection<Investor> Investors
        {
            get => _investors;
            set => this.RaiseAndSetIfChanged(ref _investors, value);
        }

        public override IScreen HostScreen => throw new NotImplementedException();
        public bool IsPaneOpen
        {
            get => _isPaneOpen;
            set => this.RaiseAndSetIfChanged(ref _isPaneOpen, value);
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (value.Length == 0)
                {
                    Investors = _context.Investors.Local.ToObservableCollection();
                }
                this.RaiseAndSetIfChanged(ref _searchText, value);
            }
        }

        public IObservable<bool> IsSearchTextExist { get; }

        public ReactiveCommand<Unit, Unit> PaneCommand { get;}
        public ReactiveCommand<Unit, Unit> NewInvestorCommand { get; }
        public ReactiveCommand<Unit, Unit> SearchCommand { get; }
        public ReactiveCommand<Investor, Unit> DataGridPrintActionCommand { get; set; }
        public ReactiveCommand<Investor, Unit> DataGridEditActionCommand { get; set; }
        public ReactiveCommand<Investor, Unit> DataGridRemoveActionCommand { get; set; }
    }
}
