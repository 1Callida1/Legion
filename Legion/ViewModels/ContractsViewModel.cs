﻿using Legion.Models;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using DynamicData;
using Legion.Helpers;
using Contract = Legion.Models.Contract;
using SkiaSharp;
using System.Security.Principal;

namespace Legion.ViewModels
{
    public class ContractsViewModel : ViewModelBase
    {
        private ApplicationDbContext _context;
        private bool _isPaneOpen;
        private ObservableCollection<Models.Contract> _contracts;
        private string _searchText;
        private List<ContractType> _contractTypes;
        private ContractType _selectedContractType;

        public ContractsViewModel(ApplicationDbContext context, IScreen? hostScreen = null)
        {
            _context = context;
            ContractTypes = context.ContractTypes.ToList();
            ContractTypes.Add(new ContractType() {TypeName = "Все", Bet = 0, CanAddMoney = false, ContractIdFormat = "", Formula = "", Period = 0});
            _selectedContractType = ContractTypes.First(t => t.TypeName == "Все");
            _isPaneOpen = false;
            
            Contracts = new ObservableCollection<Contract>(_context.Contracts.ToList());

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

            ShowDialog = new Interaction<AddIntegerViewModel, object>()!;
            ShowAdditionalPaymentsDialog = new Interaction<AdditionalPaymentsHistoryViewModel, object>()!;
            ShowPaymentsDialog = new Interaction<PaymentsHistoryViewModel, object>()!;

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
                
                //TODO: Распечатать бланк закрытия договора
            });

            DataGridProlongationActionCommand = ReactiveCommand.CreateFromTask(async (Models.Contract ctr) =>
            {
                string? result = (string?) await ShowDialog.Handle(new AddIntegerViewModel("Введите количество месяцев:", "Добавить"));

                if (result == null)
                    return;

                Contract copyContract = _context.Contracts.First(c => c.Id == ctr.Id);
                copyContract.Id = 0;
                copyContract.DateEnd = copyContract.DateEnd.AddMonths(int.Parse(result));
                copyContract.Repeated = true;
                copyContract.RepeatNumber = _context.Contracts.Count(c => c.CustomId == ctr.CustomId) + 1;
                await _context.Contracts.AddAsync(copyContract);

                await _context.SaveChangesAsync();

                Contracts = new ObservableCollection<Contract>(await _context.Contracts.ToListAsync());
            });

            DataGridShowPaymentsActionCommand = ReactiveCommand.CreateFromTask(async (Models.Contract ctr) =>
            {
                await ShowPaymentsDialog.Handle(new PaymentsHistoryViewModel(context, ctr));
            });

            DataGridAddMoneyActionCommand = ReactiveCommand.CreateFromTask(async (Contract ctr) =>
            {
                object? result = await ShowDialog.Handle(new AddIntegerViewModel("Введите сумму пополнения:", "Пополнить"));

                if (result == null)
                    return;

                ctr.Amount += int.Parse((string)result);

                await _context.AdditionalPayments.AddAsync(new AdditionalPayment()
                    { Amount = int.Parse((string)result), Contract = ctr, Date = DateTime.Now });

                _context.Contracts.Update(ctr);
                await _context.SaveChangesAsync();

                Contracts = new ObservableCollection<Contract>(await _context.Contracts.ToListAsync());
            });

            DataGridPrintActionCommand = ReactiveCommand.Create((Models.Contract ctr) =>
            {
                Debug.WriteLine(ctr.ToString());
            });

            DataGridRemoveActionCommand = ReactiveCommand.CreateFromTask(async (Models.Contract ctr) =>
            {
                Debug.WriteLine(ctr.Id.ToString() + "to remove");

                _context.Contracts.Remove(ctr);

                await _context.SaveChangesAsync();
                await _context.Investors.LoadAsync();
            });

            DataGridAddMoneyHistoryActionCommand = ReactiveCommand.CreateFromTask(async (Models.Contract ctr) =>
            {
                await ShowAdditionalPaymentsDialog.Handle(new AdditionalPaymentsHistoryViewModel(context, ctr));
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

        public ContractType SelectedContractType
        {
            get => _selectedContractType;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedContractType, value);

                if (value.TypeName != "Все")
                {
                    Contracts = new ObservableCollection<Contract>(_context.Contracts
                        .Where(c => c.ContractType.Id == value.Id).ToList());
                }
                else
                    Contracts = new ObservableCollection<Contract>(_context.Contracts.ToList());
            }
        }

        public List<ContractType> ContractTypes
        {
            get => _contractTypes;
            set => this.RaiseAndSetIfChanged(ref _contractTypes, value);
        }

        public ReactiveCommand<Unit, Unit> BackCommand { get; } = null!;

        public ReactiveCommand<Unit, Unit> PaneCommand { get; } = null!;
        public ReactiveCommand<Unit, Unit> NewContractCommand { get; } = null!;
        
        public ReactiveCommand<Unit, Unit> SearchCommand { get; } = null!;
        public IObservable<bool> IsSearchTextExist { get; } = null!;

        public Interaction<AddIntegerViewModel, object?> ShowDialog { get; } = null!;
        public Interaction<AdditionalPaymentsHistoryViewModel, object?> ShowAdditionalPaymentsDialog { get; } = null!;
        public Interaction<PaymentsHistoryViewModel, object?> ShowPaymentsDialog { get; } = null!;

        public ReactiveCommand<Models.Contract, Unit> DataGridPrintActionCommand { get; set; } = null!;
        public ReactiveCommand<Models.Contract, Unit> DataGridCloseActionCommand { get; set; } = null!;
        public ReactiveCommand<Models.Contract, Unit> DataGridProlongationActionCommand { get; set; } = null!;
        public ReactiveCommand<Models.Contract, Unit> DataGridShowPaymentsActionCommand { get; set; } = null!;
        public ReactiveCommand<Models.Contract, Unit> DataGridEditActionCommand { get; set; } = null!;
        public ReactiveCommand<Models.Contract, Unit> DataGridRemoveActionCommand { get; set; } = null!;
        public ReactiveCommand<Models.Contract, Unit> DataGridAddMoneyActionCommand { get; set; } = null!;
        public ReactiveCommand<Models.Contract, Unit> DataGridAddMoneyHistoryActionCommand { get; set; } = null!;
        
    }
}
