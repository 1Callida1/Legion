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
    public class AdditionalPaymentsHistoryViewModel : ViewModelBase
    {
        private ApplicationDbContext _context;
        private ObservableCollection<AdditionalPayment> _payments = null!;

        public AdditionalPaymentsHistoryViewModel(ApplicationDbContext context, Contract contract, IScreen? hostScreen = null)
        {
            HostScreen = hostScreen ?? Locator.Current.GetService<IScreen>()!;
            _context = context;

            Payments = new ObservableCollection<AdditionalPayment>(_context.AdditionalPayments
                .Where(ap => ap.Contract.Id == contract.Id).ToList());
            
            BackCommand = ReactiveCommand.Create(() => new Contract());
        }

        public ObservableCollection<AdditionalPayment> Payments
        {
            get => _payments;
            set => this.RaiseAndSetIfChanged(ref _payments, value);
        }

        public Interaction<InvestorSerachViewModel, Investor?> ShowDialog { get; } = null!;
        public ReactiveCommand<Unit, Contract> BackCommand { get; }

        public sealed override IScreen HostScreen { get; set; } = null!;
    }
}
