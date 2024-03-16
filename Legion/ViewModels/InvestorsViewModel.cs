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
using Splat;

namespace Legion.ViewModels
{
    public class InvestorsViewModel : ViewModelBase
    {
        private ApplicationDbContext _context;
        private bool _isPaneOpen;
        private string _searchText = null!;
        private ObservableCollection<Investor> _investors = null!;

        public InvestorsViewModel()
        {
            _context = new ApplicationDbContext(new DbContextOptions<ApplicationDbContext>());
        }

        public InvestorsViewModel(ApplicationDbContext context, IScreen? hostScreen = null)
        {
            _context = context;
            _isPaneOpen = false;
            _context.Investors.Load();
            Investors = _context.Investors.Local.ToObservableCollection();

            HostScreen = hostScreen ?? Locator.Current.GetService<IScreen>()!;

            IsSearchTextExist = this.WhenAnyValue(
                x => x.SearchText,
                (text) =>
                    !string.IsNullOrWhiteSpace(text)
            );

            SearchCommand = ReactiveCommand.Create(() =>
            {
                Investors = new ObservableCollection<Investor>(_context.Investors
                    .Where(inv => inv.LastName.Contains(SearchText) || inv.FirstName.Contains(SearchText) || inv.MiddleName.Contains(SearchText) || inv.Email.Contains(SearchText) || inv.Phone.Contains(SearchText) || inv.City.Contains(SearchText) || inv.DateBirth.ToString().Contains(SearchText)).ToList());
            }, IsSearchTextExist);

            PaneCommand = ReactiveCommand.Create(() =>
            {
                IsPaneOpen = !IsPaneOpen;
            });

            NewInvestorCommand = ReactiveCommand.Create(() =>
            {
                HostScreen.Router.Navigate.Execute(new AddInvestorViewModel(context));
            });

            DataGridEditActionCommand = ReactiveCommand.Create((Investor inv) =>
            {
                HostScreen.Router.Navigate.Execute(new AddInvestorViewModel(inv, context));
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

            BackCommand = ReactiveCommand.Create(() =>
            {
                HostScreen.Router.NavigateBack.Execute();
            });
        }

        public ObservableCollection<Investor> Investors
        {
            get => _investors;
            set => this.RaiseAndSetIfChanged(ref _investors, value);
        }

        public sealed override IScreen HostScreen { get; set; } = null!;

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

        public ReactiveCommand<Unit, Unit> BackCommand { get; } = null!;
        public IObservable<bool> IsSearchTextExist { get; } = null!;
        public string InvCitySBText { get; } = null!;
        public string InvRegistrationSBText { get; } = null!;
        public DateOnly InvDobDPDate { get; }
        public int InvDobCBItem { get; }
        public ReactiveCommand<Investor, Unit> SerachCommand { get; set; } = null!;
        public ReactiveCommand<Unit, Unit> PaneCommand { get;} = null!;
        public ReactiveCommand<Unit, Unit> NewInvestorCommand { get; } = null!;
        public ReactiveCommand<Unit, Unit> SearchCommand { get; } = null!;
        public ReactiveCommand<Investor, Unit> DataGridPrintActionCommand { get; set; } = null!;
        public ReactiveCommand<Investor, Unit> DataGridEditActionCommand { get; set; } = null!;
        public ReactiveCommand<Investor, Unit> DataGridRemoveActionCommand { get; set; } = null!;
    }
}
