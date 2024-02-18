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

namespace Legion.ViewModels
{
    public class InvestorsViewModel : ViewModelBase
    {
        private ApplicationDbContext _context;
        private bool _isPaneOpen;

        public InvestorsViewModel(IScreen hostScreen, ApplicationDbContext context)
        {
            _context = context;
            _isPaneOpen = false;

            PaneCommand = ReactiveCommand.Create(() =>
            {
                IsPaneOpen = !IsPaneOpen;
            });

            NewInvestorCommand = ReactiveCommand.Create(() =>
            {
                hostScreen.Router.Navigate.Execute(new AddInvestorViewModel(hostScreen, context));
            });
        }

        public List<Investor> Investors => _context.Investors.ToList();

        public override IScreen HostScreen => throw new NotImplementedException();
        public bool IsPaneOpen
        {
            get => _isPaneOpen;
            set => this.RaiseAndSetIfChanged(ref _isPaneOpen, value);
        }

        public ReactiveCommand<Unit, Unit> PaneCommand { get;}
        public ReactiveCommand<Unit, Unit> NewInvestorCommand { get; }
    }
}
