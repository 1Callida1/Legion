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
using System.Diagnostics.Contracts;
using System.Net;
using Contract = Legion.Models.Contract;

namespace Legion.ViewModels
{
    public class ExpiringContractViewModel : ViewModelBase
    {
        private readonly ApplicationDbContext _context;
        private List<Contract> _contracts;

        public ExpiringContractViewModel(ApplicationDbContext context, IScreen? hostScreen = null)
        {
            _context = context;
            DateTime dateTime = DateTime.Now;
            _contracts = _context.Contracts.Where(c => c.DateEnd <= new DateTime(dateTime.Year, dateTime.Month, DateTime.DaysInMonth(dateTime.Year, dateTime.Month)) && c.DateEnd >= new DateTime(dateTime.Year, dateTime.Month, 1)).ToList();
            HostScreen = hostScreen ?? Locator.Current.GetService<IScreen>()!;
            _context.ContractTypes.Load();

            BackCommand = ReactiveCommand.Create(() =>
            {
                HostScreen.Router.NavigateBack.Execute();
            });

        }

        public List<Contract> Contracts
        {
            get => _contracts;
            set => this.RaiseAndSetIfChanged(ref _contracts, value);
        }

        public ObservableCollection<ContractType> ContractTypes => _context.ContractTypes.Local.ToObservableCollection();
        public sealed override IScreen HostScreen { get; set; }
        public ReactiveCommand<Unit, Unit> BackCommand { get; } = null!;
    }
}
