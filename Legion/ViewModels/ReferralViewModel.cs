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
        private List<Contract> _contract = null!;
        private string _searchText = null!;
    
        public ReferralViewModel(ApplicationDbContext context, IScreen? hostScreen = null)
        {
            _context = context;
            HostScreen = hostScreen ?? Locator.Current.GetService<IScreen>()!;
            Contracts = _context.Contracts.Where(c => c.Referral != null).ToList();

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
                Contracts = new List<Contract>();
                SearchText.Split(' ').ToList().ForEach(word => Contracts.AddRange(_context.Contracts.Where(c => c.CustomId.ToLower().Contains(word.ToLower()) || c.Investor.FirstName.ToLower().Contains(word.ToLower()) || c.Investor.MiddleName.ToLower().Contains(word.ToLower()) || c.Investor.LastName.ToLower().Contains(word.ToLower()))));
                Contracts = Contracts.Distinct().ToList();
            }, IsSearchTextExist);

            DataGridBonusActionCommand = ReactiveCommand.CreateFromTask(async(Models.Contract ctr) =>
            {
                ctr.Referral.BonusClaim = !ctr.Referral.BonusClaim;
                _context.Referrals.Update(ctr.Referral);
                await _context.SaveChangesAsync();

                Contracts = await _context.Contracts.Where(c => c.Referral != null).ToListAsync();

            });
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    Contracts = _context.Contracts.Where(c => c.Referral != null).ToList();
                }

                this.RaiseAndSetIfChanged(ref _searchText, value);
            }
        }


        public List<Contract> Contracts
        {
            get => _contract;
            set => this.RaiseAndSetIfChanged(ref _contract, value);
        }

        public ReactiveCommand<Models.Contract, Unit> DataGridBonusActionCommand { get; set; } = null!;

        public ReactiveCommand<Unit, Unit> BackCommand { get; } = null!;
        public sealed override IScreen HostScreen { get; set; }
        public IObservable<bool> IsSearchTextExist { get; } = null!;
        public ReactiveCommand<Unit, Unit> SearchCommand { get; } = null!;
    }
}
