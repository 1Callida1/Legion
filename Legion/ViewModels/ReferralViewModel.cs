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
    public class ReferralViewModel : ViewModelBase
    {
        private readonly ApplicationDbContext _context;
        private ObservableCollection<Referral> _referral = null!;
        private ObservableCollection<Contract> _contract = null!;
        private string _searchText = null!;
    
        public ReferralViewModel(ApplicationDbContext context, IScreen? hostScreen = null)
        {
            _context = context;
            HostScreen = hostScreen ?? Locator.Current.GetService<IScreen>()!;
            Contracts = _context.Contracts.Local.ToObservableCollection();

            BackCommand = ReactiveCommand.Create(() =>
            {
                HostScreen.Router.NavigateBack.Execute();
            });

            IsSearchTextExist = this.WhenAnyValue(
                x => x.SearchText,
                (text) =>
                    !string.IsNullOrWhiteSpace(text)
            );

            SearchCommand = ReactiveCommand.Create(() =>
            {
               
            }, IsSearchTextExist);
        }
        public ObservableCollection<Referral> Referrals
        {
            get => _referral;
            set => this.RaiseAndSetIfChanged(ref _referral, value);
        }
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (value.Length == 0)
                {
                    Contracts = _context.Contracts.Local.ToObservableCollection();
                }
                this.RaiseAndSetIfChanged(ref _searchText, value);
            }
        }


        public ObservableCollection<Contract> Contracts
        {
            get => _contract;
            set => this.RaiseAndSetIfChanged(ref _contract, value);
        }
        public ReactiveCommand<Unit, Unit> BackCommand { get; } = null!;
        public sealed override IScreen HostScreen { get; set; }
        public IObservable<bool> IsSearchTextExist { get; } = null!;
        public ReactiveCommand<Unit, Unit> SearchCommand { get; } = null!;
    }
}
