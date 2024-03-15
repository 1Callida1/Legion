using Avalonia;
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
        private ApplicationDbContext _context;
        private Models.Contract _contract;

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
            Contract = new Legion.Models.Contract();
            SubmitText = "Добавить новый договор";
            _context.ContractTypes.Load();
            _context.ContractStatuses.Load();
            Contract.ContractType = ContractTypes.First();
            Contract.Status = ContractStatuses.First();
            SearchInvestorCommand = ReactiveCommand.Create(() =>
            {
                var a = HostScreen.Router.CurrentViewModel;
                HostScreen.Router.Navigate.Execute(new InvestorSerachViewModel(context));
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

        public ObservableCollection<ContractStatus> ContractStatuses => _context.ContractStatuses.Local.ToObservableCollection();
        public ObservableCollection<ContractType> ContractTypes => _context.ContractTypes.Local.ToObservableCollection();

        public ReactiveCommand<Unit, Unit> SearchInvestorCommand { get; }
        public ReactiveCommand<Unit, Unit> BackCommand { get; }
        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
        public string SubmitText { get; protected set; }
        public Legion.Models.Contract Contract { get => _contract; set => this.RaiseAndSetIfChanged(ref _contract, value); }

        public override IScreen HostScreen { get; set; }
    }
}