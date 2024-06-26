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
using Legion.Models.Internal;
using System.Threading.Tasks;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using Legion.Views;
using Serilog;
using ILogger = Serilog.ILogger;
using System.IO;

namespace Legion.ViewModels
{
    public class ContractsViewModel : ViewModelBase
    {
        private ILogger _log = Log.Logger.ForContext<ContractsViewModel>();
        private ApplicationDbContext _context;
        private bool _isPaneOpen;
        private ObservableCollection<Models.Contract> _contracts;
        private string _searchText;
        private List<ContractType> _contractTypes;
        private ContractType _selectedContractType;
        private bool _loadingVisible = false;
        private bool _addContractsVisible = false;
        private bool _hiddenDataVisible = false;
        private bool _removeVisible = false;

        public ContractsViewModel(ApplicationDbContext context, IScreen? hostScreen = null)
        {
            _context = context;
            _isPaneOpen = false;

            ContractTypes = _context.ContractTypes.ToList();
            ContractTypes.Add(new ContractType() { TypeName = "Все", Bet = 0, CanAddMoney = false, ContractIdFormat = "", Formula = "", Period = 0 });
            SelectedContractType = ContractTypes.First(t => t.TypeName == "Все");

            HostScreen = hostScreen ?? Locator.Current.GetService<IScreen>()!;

            IsSearchTextExist = this.WhenAnyValue(
                x => x.SearchText,
                (text) =>
                    !string.IsNullOrWhiteSpace(text)
            );

            RefreshCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                Contracts = new ObservableCollection<Contract>(await _context.Contracts.ToListAsync());
            });

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

            NewContractCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                await HostScreen.Router.Navigate.Execute(new AddContractViewModel(context));

                Contracts = new ObservableCollection<Contract>(_context.Contracts.ToList());
            });

            DataGridEditActionCommand = ReactiveCommand.Create((Models.Contract ctr) =>
            {
                HostScreen.Router.Navigate.Execute(new AddContractViewModel(ctr, context));
            });

            DataGridCloseActionCommand = ReactiveCommand.CreateFromTask( async (Models.Contract ctr) =>
            {
                ContractStatus closed;
                try
                {
                    closed = await _context.ContractStatuses.FirstAsync(s => s.Status == "Закрыт");
                }
                catch (Exception ex)
                {
                    _log.Error(ex.Message);

                    var box = MessageBoxManager
                        .GetMessageBoxStandard("Ошибка!", "Не удалось закрыть договор!",
                            ButtonEnum.Ok, Icon.Error);

                    await box.ShowAsPopupAsync(Locator.Current.GetService<MainWindow>());

                    return;
                }
                
                if (ctr.Status == closed)
                    return;

                ctr.Status = closed;

                try
                {
                    _context.Contracts.Update(ctr);
                    _context.SaveChanges();
                }
                catch (Exception ex)
                {
                    _log.Error(ex.Message);

                    var box = MessageBoxManager
                        .GetMessageBoxStandard("Ошибка!", "Не удалось закрыть договор!",
                            ButtonEnum.Ok, Icon.Error);

                    await box.ShowAsPopupAsync(Locator.Current.GetService<MainWindow>());

                    return;
                }

                Contracts = new ObservableCollection<Contract>(await _context.Contracts.ToListAsync());

                string subPath = PathHelper.generatePath(ctr);

                Helpers.ReportGenerator.WordGenerator.GenerateDocument(ctr, "Закрытие договора", subPath);
            });

            DataGridProlongationActionCommand = ReactiveCommand.CreateFromTask(async (Models.Contract ctr) =>
            {
                string? result = (string?) await ShowDialog.Handle(new AddIntegerViewModel("Введите количество месяцев:", "Добавить"));

                if (string.IsNullOrWhiteSpace(result))
                    return;

                ctr = _context.Contracts.AsNoTracking().First(c => c.Id == ctr.Id);
                Contract copyContract = _context.Contracts.First(c => c.Id == ctr.Id);
                copyContract.Id = 0;
                copyContract.DateEnd = copyContract.DateEnd.AddMonths(int.Parse(result));
                copyContract.Repeated = true;
                copyContract.Referral = null;
                copyContract.RepeatNumber = _context.Contracts.Count(c => c.CustomId == ctr.CustomId) + 1;
                copyContract.DateProlonagtion = DateTime.Now;
                await _context.Contracts.AddAsync(copyContract);

                await _context.SaveChangesAsync();

                Contracts = new ObservableCollection<Contract>(await _context.Contracts.ToListAsync());

                string subPath = PathHelper.generatePath(ctr);

                Helpers.ReportGenerator.WordGenerator.GenerateDocument(ctr, "Доп соглашение пролонгация", subPath, copyContract);

                byte[] reportExcel = Helpers.ReportGenerator.ExcelGenerator.GeneratePayments(ctr, int.Parse(result));
                string path = subPath +
                   $"/Акт выплат №{ctr.CustomId.Replace("/", ".")} от " +
                   DateTime.Now.ToString("dd.MM.yyyy") +
                   $" {ctr.Investor.LastName} {ctr.Investor.FirstName[0]}. {ctr.Investor.MiddleName[0]}..xlsx";
                File.WriteAllBytes($"{path}", reportExcel);
            });

            DataGridShowPaymentsActionCommand = ReactiveCommand.CreateFromTask(async (Models.Contract ctr) =>
            {
                await ShowPaymentsDialog.Handle(new PaymentsHistoryViewModel(context, ctr));
            });

            DataGridAddMoneyActionCommand = ReactiveCommand.CreateFromTask(async (Contract ctr) =>
            {
                string? result = (string?) await ShowDialog.Handle(new AddIntegerViewModel("Введите сумму пополнения:", "Пополнить"));

                if (string.IsNullOrWhiteSpace(result))
                    return;

                ctr.Amount += int.Parse((string)result);

                AdditionalPayment ap = new AdditionalPayment()
                    { Amount = int.Parse((string)result), Contract = ctr, Date = DateTime.Now };

                await _context.AdditionalPayments.AddAsync(ap);

                _context.Contracts.Update(ctr);
                await _context.SaveChangesAsync();

                Contracts = new ObservableCollection<Contract>(await _context.Contracts.ToListAsync());

                string subPath = PathHelper.generatePath(ctr);

                Helpers.ReportGenerator.WordGenerator.GenerateDocument(ctr, "Доп соглашение накопительный", subPath, null, ap);

                byte[] reportExcel = Helpers.ReportGenerator.ExcelGenerator.GeneratePayments(ctr, int.Parse(result));
                string path = subPath +
                   $"/Акт выплат пополненный №{ctr.CustomId.Replace("/", ".")} от " +
                   DateTime.Now.ToString("dd.MM.yyyy") +
                   $" {ctr.Investor.LastName} {ctr.Investor.FirstName[0]}. {ctr.Investor.MiddleName[0]}..xlsx";
                File.WriteAllBytes($"{path}", reportExcel);
            });

            DataGridPrintActionCommand = ReactiveCommand.Create((Models.Contract ctr) =>
            {
                string subPath = PathHelper.generatePath(ctr);

                Helpers.ReportGenerator.WordGenerator.GenerateDocument(ctr, "Акт", subPath);    
                switch (ctr.ContractType.TypeName)
                {
                    case "Накопительный Е":
                        Helpers.ReportGenerator.WordGenerator.GenerateDocument(ctr, "Договор накопительный Е", subPath);
                        break;
                    case "Накопительный":
                        Helpers.ReportGenerator.WordGenerator.GenerateDocument(ctr, "Договор накопительный", subPath);
                        break;
                    case "Инвестиционный":
                        Helpers.ReportGenerator.WordGenerator.GenerateDocument(ctr, "Договор инвестирования 12", subPath);
                        break;
                    case "Трехгодовой":
                        Helpers.ReportGenerator.WordGenerator.GenerateDocument(ctr, "Договор инвестирования 36", subPath);
                        break;
                    case "Доходный":
                        Helpers.ReportGenerator.WordGenerator.GenerateDocument(ctr, "Договор доходный", subPath);
                        break;
                    case "ТАНАКА инвестиционный":
                        Helpers.ReportGenerator.WordGenerator.GenerateDocument(ctr, "Договор инвестирования ТАНАКА", subPath);
                        break;
                    case "ТАНАКА накопительный":
                        Helpers.ReportGenerator.WordGenerator.GenerateDocument(ctr, "Договор накопительный ТАНАКА", subPath);
                        break;
                }
                byte[] reportExcel = Helpers.ReportGenerator.ExcelGenerator.GeneratePayments(ctr);
                string path = subPath +
                   $"/Акт выплат №{ctr.CustomId.Replace("/", ".")} {ctr.Investor.LastName} {ctr.Investor.FirstName[0]}. {ctr.Investor.MiddleName[0]}..xlsx";
                File.WriteAllBytes($"{path}", reportExcel);
            });

            DataGridRemoveActionCommand = ReactiveCommand.CreateFromTask(async (Models.Contract ctr) =>
            {
                Debug.WriteLine(ctr.Id.ToString() + "to remove");

                _context.Contracts.Remove(ctr);

                await _context.SaveChangesAsync();
                await _context.Investors.LoadAsync();

                Contracts = new ObservableCollection<Contract>(_context.Contracts.ToList());
            });

            DataGridAddMoneyHistoryActionCommand = ReactiveCommand.CreateFromTask(async (Models.Contract ctr) =>
            {
                await ShowAdditionalPaymentsDialog.Handle(new AdditionalPaymentsHistoryViewModel(context, ctr));

                Contracts = new ObservableCollection<Contract>(_context.Contracts.ToList());
            });

            BackCommand = ReactiveCommand.Create(() =>
            {
                HostScreen.Router.NavigateBack.Execute();
            });
        }

        private async Task LoadContractsAsync()
        {
            LoadingVisible = true;
            Contracts = new ObservableCollection<Contract>(await _context.Contracts.ToListAsync());
            await _context.UserRoles.LoadAsync();

            var user = Locator.Current.GetService<User>()!;
            AddContractsVisible = user.UserRole != null && user.UserRole.CanAddContracts;
            HiddenDataVisible = user.UserRole != null && user.UserRole.CanSeeHiddenData;
            RemoveVisible = user.UserRole != null && user.UserRole.CanDeleteData;
            LoadingVisible = false;
        }

        public bool RemoveVisible
        {
            get => _removeVisible;
            set => this.RaiseAndSetIfChanged(ref _removeVisible, value);
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
                    Contracts = new ObservableCollection<Contract>(_context.Contracts.Local
                        .Where(c => c.ContractType.Id == value.Id).ToList());
                }
                else
                    LoadContractsAsync();
            }
        }

        public bool AddContractsVisible
        {
            get => _addContractsVisible;
            set => this.RaiseAndSetIfChanged(ref _addContractsVisible, value);
        }

        public bool HiddenDataVisible
        {
            get => _hiddenDataVisible;
            set => this.RaiseAndSetIfChanged(ref _hiddenDataVisible, value);
        }

        public List<ContractType> ContractTypes
        {
            get => _contractTypes;
            set => this.RaiseAndSetIfChanged(ref _contractTypes, value);
        }

        public bool LoadingVisible
        {
            get => _loadingVisible;
            set => this.RaiseAndSetIfChanged(ref _loadingVisible, value);
        }

        public ReactiveCommand<Unit, Unit> BackCommand { get; } = null!;
        public ReactiveCommand<Unit, Unit> RefreshCommand { get; } = null!;

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
