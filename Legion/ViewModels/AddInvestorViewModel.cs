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

namespace Legion.ViewModels
{
    public class AddInvestorViewModel : ViewModelBase
    {
        private ApplicationDbContext _context;
        private Investor _investor;

        public AddInvestorViewModel(IScreen hostScreen, ApplicationDbContext context)
        {
            HostScreen = hostScreen;
            _context = context;
            Investor = new Investor();

            BackCommand = ReactiveCommand.Create(() =>
            {
                hostScreen.Router.NavigateBack.Execute();
            });

            SaveCommand = ReactiveCommand.Create(() =>
            {
                _context.Investors.Add(Investor);

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

        public ReactiveCommand<Unit, Unit> BackCommand { get; }
        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
        public Investor Investor { get => _investor; set => this.RaiseAndSetIfChanged(ref _investor, value); }

        public override IScreen HostScreen { get; }
    }
}