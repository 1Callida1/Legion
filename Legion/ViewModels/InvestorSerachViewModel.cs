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
using DynamicData;
using Avalonia.Interactivity;
using Tmds.DBus.Protocol;

namespace Legion.ViewModels
{
    public class InvestorSerachViewModel : ViewModelBase
    {
        private ApplicationDbContext _context;
        private string _searchText = null!;
        private ObservableCollection<Investor> _investors = null!;
        private Investor _selectedInvestor = null!;

        public InvestorSerachViewModel(ApplicationDbContext context,IScreen? hostScreen = null)
        {
            HostScreen = hostScreen ?? Locator.Current.GetService<IScreen>()!;
            _context = context;
            _context.Investors.Load();
            Investors = _context.Investors.Local.ToObservableCollection();

            IsSearchTextExist = this.WhenAnyValue(
                x => x.SearchText,
                (text) =>
                    !string.IsNullOrWhiteSpace(text)
            );

            BackCommand = ReactiveCommand.Create(() => new Investor());
            SearchCommand = ReactiveCommand.Create(() =>
            {
                Investors = new ObservableCollection<Investor>();
                SearchText.Split(' ').ToList().ForEach(word => Investors.Add(_context.Investors.Where(inv =>
                    inv.LastName.Contains(word) || inv.FirstName.Contains(word) || inv.MiddleName.Contains(word) ||
                    inv.Email.Contains(word) || inv.Phone.Contains(word) || inv.City.Contains(word) ||
                    inv.DateBirth.ToString().Contains(word))));
                Investors = new ObservableCollection<Investor>(Investors.Distinct());
            }, IsSearchTextExist);

            SelectInvestor = ReactiveCommand.Create(() => SelectedInvestor!);
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

        public Investor SelectedInvestor
        { 
            get => _selectedInvestor;
            set => this.RaiseAndSetIfChanged(ref _selectedInvestor, value);
        }

        public ObservableCollection<Investor> Investors
        {
            get => _investors;
            set => this.RaiseAndSetIfChanged(ref _investors, value);
        }

        public ReactiveCommand<Unit, Investor> SelectInvestor { get; }

        public ReactiveCommand<Unit, Investor> BackCommand { get; }

        public sealed override IScreen HostScreen { get; set; } = null!;
        public IObservable<bool> IsSearchTextExist { get; } = null!;
        public ReactiveCommand<Unit, Unit> SearchCommand { get; } = null!;
    }
}
