﻿using Avalonia;
using Legion.Models;
using Legion.ViewModels;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using System.Diagnostics.Contracts;
using Microsoft.EntityFrameworkCore;
using Splat;

namespace Legion.ViewModels
{
    public class AddContractViewModel : ViewModelBase
    {
        private ApplicationDbContext _context = null!;
        private Models.Contract _contract = null!;
        private bool _backgroundPaneVisible = false;

        public AddContractViewModel()
        {
            BackgroundPaneVisible = false;
        }

        public AddContractViewModel(Models.Contract contract, ApplicationDbContext context, IScreen? hostScreen = null) : this(
            context, hostScreen)
        {
            Contract = contract;
            SubmitText = "Редактировать договор";
            SaveCommand = ReactiveCommand.Create(() =>
            {
                _context.Contracts.Update(Contract);

                try
                {
                    _context.SaveChanges();
                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message);
                }
                finally
                {
                    BackCommand.Execute();
                }
            });
        }

        public AddContractViewModel(ApplicationDbContext context, IScreen? hostScreen = null)
        {
            HostScreen = hostScreen ?? Locator.Current.GetService<IScreen>()!;
            _context = context;
            Contract = new Models.Contract() { Manager = Locator.Current.GetService<User>()!};
            SubmitText = "Добавить новый договор";
            _context.ContractTypes.Load();
            _context.ContractStatuses.Load();
            Contract.ContractType = ContractTypes.First();
            Contract.Status = ContractStatuses.First();

            ShowDialog = new Interaction<InvestorSerachViewModel, Investor?>();

            SearchInvestorCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                BackgroundPaneVisible = true;
                Investor? result = await ShowDialog.Handle(new InvestorSerachViewModel(context));
                BackgroundPaneVisible = false;
                if (result != null && !string.IsNullOrWhiteSpace(result.FirstName))
                    Contract.Investor = result;

                this.RaisePropertyChanged(nameof(InvestorData));
            });

            BackCommand = ReactiveCommand.Create(() =>
            {
                HostScreen.Router.NavigateBack.Execute();
            });

            SaveCommand = ReactiveCommand.Create(() =>
            {
                _context.Contracts.Add(Contract);

                try
                {
                    _context.SaveChanges();
                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message);
                }
                finally
                {
                    BackCommand.Execute();
                }
            });
        }

        public bool BackgroundPaneVisible
        {
            get => _backgroundPaneVisible;
            set => this.RaiseAndSetIfChanged(ref _backgroundPaneVisible, value);
        }

        public int? Amount
        {
            get => Contract.Amount;
            set
            {
                if (value == null || value < 0)
                {
                    Contract.Amount = 0;
                }
                else
                {
                    Contract.Amount = value.Value;
                }
                this.RaisePropertyChanged();
            }
        }

        public string InvestorData
        {
            get
            {
                if (Contract.Investor != null && !string.IsNullOrWhiteSpace(Contract.Investor.LastName))
                    return
                        $"{Contract.Investor.LastName} {Contract.Investor.FirstName[0]}.{Contract.Investor.MiddleName[0]}. {Contract.Investor.PassprotSeries} {Contract.Investor.PassprotNumber}";
                
                return string.Empty;
            }
        } 

        public ObservableCollection<ContractStatus> ContractStatuses => _context.ContractStatuses.Local.ToObservableCollection();
        public ObservableCollection<ContractType> ContractTypes => _context.ContractTypes.Local.ToObservableCollection();

        public ReactiveCommand<Unit, Unit> SearchInvestorCommand { get; } = null!;
        public ReactiveCommand<Unit, Unit> BackCommand { get; } = null!;
        public ReactiveCommand<Unit, Unit> SaveCommand { get; } = null!;
        public string SubmitText { get; protected set; } = null!;
        public Models.Contract Contract { get => _contract; set => this.RaiseAndSetIfChanged(ref _contract, value); }

        public Interaction<InvestorSerachViewModel, Investor?> ShowDialog { get; } = null!;

        public sealed override IScreen HostScreen { get; set; } = null!;
    }
}