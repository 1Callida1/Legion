using Legion.Models;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using Splat;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using DynamicData;
using Contract = Legion.Models.Contract;

namespace Legion.ViewModels
{
    public class ContractsViewModel : ViewModelBase
    {
        private ApplicationDbContext _context;
        private bool _isPaneOpen;
        private ObservableCollection<Models.Contract> _contracts;
        private string _searchText;

        public ContractsViewModel(ApplicationDbContext context, IScreen? hostScreen = null)
        {
            _context = context;
            _isPaneOpen = false;
            _context.Contracts.Load();
            _context.RenewalContracts.Load();
            Contracts = _context.Contracts.Local.ToObservableCollection();

            _context.RenewalContracts.ToList().ForEach(rc =>
            {
                Contract tempContract = rc.Contract;
                tempContract.DateEnd = rc.NewDateEnd;

                Contracts.Add(tempContract);
            });

            HostScreen = hostScreen ?? Locator.Current.GetService<IScreen>()!;

            IsSearchTextExist = this.WhenAnyValue(
                x => x.SearchText,
                (text) =>
                    !string.IsNullOrWhiteSpace(text)
            );

            SearchCommand = ReactiveCommand.Create(() =>
            {
                Contracts = new ObservableCollection<Contract>();
                SearchText.Split(' ').ToList().ForEach(word => Contracts.AddRange(_context.Contracts.Where(c => c.CustomId.ToLower().Contains(word.ToLower()) || c.Investor.FirstName.ToLower().Contains(word.ToLower()) || c.Investor.MiddleName.ToLower().Contains(word.ToLower()) || c.Investor.LastName.ToLower().Contains(word.ToLower()) )));
                Contracts = new ObservableCollection<Contract>(Contracts.Distinct());
            }, IsSearchTextExist);

            PaneCommand = ReactiveCommand.Create(() =>
            {
                IsPaneOpen = !IsPaneOpen;
            });

            NewContractCommand = ReactiveCommand.Create(() =>
            {
                HostScreen.Router.Navigate.Execute(new AddContractViewModel(context));
            });

            DataGridEditActionCommand = ReactiveCommand.Create((Models.Contract ctr) =>
            {
                HostScreen.Router.Navigate.Execute(new AddContractViewModel(ctr, context));
            });

            DataGridCloseActionCommand = ReactiveCommand.Create((Models.Contract ctr) =>
            {
                ctr.Status = _context.ContractStatuses.First(s => s.Status == "Закрыт");
                _context.Contracts.Update(ctr);
                _context.SaveChanges();

                Contracts = new ObservableCollection<Contract>(_context.Contracts.ToList());
                _context.RenewalContracts.ToList().ForEach(rc =>
                {
                    Contract tempContract = rc.Contract;
                    tempContract.DateEnd = rc.NewDateEnd;

                    Contracts.Add(tempContract);
                });
                //TODO: Распечатать бланк закрытия договора
            });

            DataGridProlongationActionCommand = ReactiveCommand.Create((Models.Contract ctr) =>
            {
                //TODO: Распечатать бланк продления договора
            });

            DataGridShowPaymentsActionCommand = ReactiveCommand.Create((Models.Contract ctr) =>
            {
                HostScreen.Router.Navigate.Execute(new AddContractViewModel(ctr, context));
            });

            DataGridAddMoneyActionCommand = ReactiveCommand.Create((Models.Contract ctr) =>
            {
                //HostScreen.Router.Navigate.Execute(new AddContractViewModel(ctr, context));
            });

            DataGridPrintActionCommand = ReactiveCommand.Create((Models.Contract ctr) =>
            {
                Debug.WriteLine(ctr.ToString());
            });

            DataGridRemoveActionCommand = ReactiveCommand.Create((Models.Contract ctr) =>
            {
                Debug.WriteLine(ctr.Id.ToString() + "to remove");
                _context.Contracts.Remove(ctr);
                _context.SaveChangesAsync();
                _context.Investors.LoadAsync();
            });

            BackCommand = ReactiveCommand.Create(() =>
            {
                HostScreen.Router.NavigateBack.Execute();
            });
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


        public ObservableCollection<Models.Contract> Contracts
        {
            get => _contracts;
            set => this.RaiseAndSetIfChanged(ref _contracts, value);
        }
        public sealed override IScreen HostScreen { get; set; } = null!;

        public bool IsPaneOpen
        {
            get => _isPaneOpen;
            set => this.RaiseAndSetIfChanged(ref _isPaneOpen, value);
        }

        public ReactiveCommand<Unit, Unit> BackCommand { get; } = null!;

        public ReactiveCommand<Unit, Unit> PaneCommand { get; } = null!;
        public ReactiveCommand<Unit, Unit> NewContractCommand { get; } = null!;
        
        public ReactiveCommand<Unit, Unit> SearchCommand { get; } = null!;
        public IObservable<bool> IsSearchTextExist { get; } = null!;
        
        public ReactiveCommand<Models.Contract, Unit> DataGridPrintActionCommand { get; set; } = null!;
        public ReactiveCommand<Models.Contract, Unit> DataGridCloseActionCommand { get; set; } = null!;
        public ReactiveCommand<Models.Contract, Unit> DataGridProlongationActionCommand { get; set; } = null!;
        public ReactiveCommand<Models.Contract, Unit> DataGridShowPaymentsActionCommand { get; set; } = null!;
        public ReactiveCommand<Models.Contract, Unit> DataGridEditActionCommand { get; set; } = null!;
        public ReactiveCommand<Models.Contract, Unit> DataGridRemoveActionCommand { get; set; } = null!;
        public ReactiveCommand<Models.Contract, Unit> DataGridAddMoneyActionCommand { get; set; } = null!;
    }
}
