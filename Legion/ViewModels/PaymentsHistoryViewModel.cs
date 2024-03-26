using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using Legion.Models;
using ReactiveUI;
using Splat;

namespace Legion.ViewModels
{
    public class PaymentsHistoryViewModel : ViewModelBase
    {
        private ApplicationDbContext _context;
        private List<AdditionalPayment> _payments = null!;

        public PaymentsHistoryViewModel(ApplicationDbContext context, Contract contract, IScreen? hostScreen = null)
        {
            HostScreen = hostScreen ?? Locator.Current.GetService<IScreen>()!;
            _context = context;

            Payments = new List<AdditionalPayment>();
            

            for (int i = 1; i < (DateTime.Now.Month - contract.DateStart.Month)+1; i++)
            {
                Payments.Add(new AdditionalPayment() { Amount=contract.Amount / 100 * contract.ContractType.Bet, Contract = contract, Date = contract.DateStart.AddMonths(i)});
            }

            BackCommand = ReactiveCommand.Create(() => new Contract());
        }

        public List<AdditionalPayment> Payments
        {
            get => _payments;
            set => this.RaiseAndSetIfChanged(ref _payments, value);
        }

        public Interaction<InvestorSerachViewModel, Investor?> ShowDialog { get; } = null!;
        public ReactiveCommand<Unit, Contract> BackCommand { get; }

        public sealed override IScreen HostScreen { get; set; } = null!;
    }
}
