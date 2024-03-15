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

namespace Legion.ViewModels
{
    public class MainMenuViewModel : ViewModelBase
    {
        private ApplicationDbContext _context;
        public MainMenuViewModel()
        {
            
        }

        public MainMenuViewModel(IScreen hostScreen, ApplicationDbContext context)
        {
            _context = context;
            HostScreen = hostScreen;

            InvestorsCommand = ReactiveCommand.Create(() =>
            {

                HostScreen.Router.Navigate.Execute(new InvestorsViewModel(HostScreen, _context));

            });

            ContractsCommand = ReactiveCommand.Create(() =>
            {
                
                HostScreen.Router.Navigate.Execute(new ContractsViewModel(HostScreen, _context));

            });
        }

        public ReactiveCommand<Unit, Unit> InvestorsCommand { get; }
        public ReactiveCommand<Unit, Unit> ContractsCommand { get; }
        public override IScreen HostScreen { get; }
    }
}
