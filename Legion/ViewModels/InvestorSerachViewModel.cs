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
using DynamicData;

namespace Legion.ViewModels
{
    public class InvestorSerachViewModel : ViewModelBase
    {
        private ApplicationDbContext _context;
        private string _searchText;
        private ObservableCollection<Investor> _investors;

        public InvestorSerachViewModel()
        {
            _context = new ApplicationDbContext(new DbContextOptions<ApplicationDbContext>());
        }

        public InvestorSerachViewModel(IScreen hostScreen, ApplicationDbContext context)
        {
            _context = context;
            _context.Investors.Load();
            Investors = _context.Investors.Local.ToObservableCollection();

            IsSearchTextExist = this.WhenAnyValue(
                x => x.SearchText,
                (text) =>
                    !string.IsNullOrWhiteSpace(text)
            );

            SearchCommand = ReactiveCommand.Create(() =>
            {
                Investors = new ObservableCollection<Investor>();
                SearchText.Split(' ').ToList().ForEach(word => Investors.Add(_context.Investors.Where(inv => inv.LastName.Contains(word) || inv.FirstName.Contains(word) || inv.MiddleName.Contains(word) || inv.Email.Contains(word) || inv.Phone.Contains(word) || inv.City.Contains(word) || inv.DateBirth.ToString().Contains(word))));
                Investors = new ObservableCollection<Investor>(Investors.Distinct());
            }, IsSearchTextExist);
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

        public ObservableCollection<Investor> Investors
        {
            get => _investors;
            set => this.RaiseAndSetIfChanged(ref _investors, value);
        }

        public IObservable<bool> IsSearchTextExist { get; }
        public ReactiveCommand<Unit, Unit> SearchCommand { get; }
        public override IScreen HostScreen => throw new NotImplementedException();
    }
}
