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
using Legion.Views;
using Avalonia.Controls.ApplicationLifetimes;

namespace Legion.ViewModels
{
    public class MainMenuViewModel : ViewModelBase
    {
        private readonly ApplicationDbContext _context;
        private bool _userManagingVisible;

        public MainMenuViewModel(ApplicationDbContext context, IScreen? hostScreen = null)
        {
            _context = context;
            User user = Locator.Current.GetService<User>()!;
            UserManagingVisible = user.UserRole != null && user.UserRole.Role is "Admin";
            HostScreen = hostScreen ?? Locator.Current.GetService<IScreen>()!;

            InvestorsCommand = ReactiveCommand.Create(() =>
            {

                HostScreen.Router.Navigate.Execute(new InvestorsViewModel(_context));

            });

            ContractsCommand = ReactiveCommand.Create(() =>
            {
                
                HostScreen.Router.Navigate.Execute(new ContractsViewModel(_context));

            });

            ReferralsCommand = ReactiveCommand.Create(() =>
            {

                HostScreen.Router.Navigate.Execute(new ReferralViewModel(_context));

            });

            ExpiringContractCommand = ReactiveCommand.Create(() =>
            {

                HostScreen.Router.Navigate.Execute(new ExpiringContractViewModel(_context));

            });

            UsersCommand = ReactiveCommand.Create(() =>
            {

                HostScreen.Router.Navigate.Execute(new UserViewModel(_context));

            });

            ReportsCommand = ReactiveCommand.Create(() =>
            {
                HostScreen.Router.Navigate.Execute(new ReportsViewModel(_context));
            });

            SettingsCommand = ReactiveCommand.Create(() =>
            {
                HostScreen.Router.Navigate.Execute(new SettingsViewModel());
            });

            ExitCommand = ReactiveCommand.Create(() =>
            {
                Locator.Current.GetService<IClassicDesktopStyleApplicationLifetime>()!.Shutdown();
            });
        }

        public bool UserManagingVisible
        {
            get => _userManagingVisible;
            set => this.RaiseAndSetIfChanged(ref _userManagingVisible, value);
        }

        public ReactiveCommand<Unit, Unit> InvestorsCommand { get; }
        public ReactiveCommand<Unit, Unit> ContractsCommand { get; }
        public ReactiveCommand<Unit, Unit> ReferralsCommand { get; }
        public ReactiveCommand<Unit, Unit> UsersCommand { get; }
        public ReactiveCommand<Unit, Unit> ExpiringContractCommand { get; }
        public ReactiveCommand<Unit, Unit> SettingsCommand { get; }
        public ReactiveCommand<Unit, Unit> ReportsCommand { get; }
        public ReactiveCommand<Unit, Unit> ExitCommand { get; }
        public sealed override IScreen HostScreen { get; set; }
    }
}
