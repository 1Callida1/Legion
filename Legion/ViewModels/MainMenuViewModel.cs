﻿using System.Reactive;
using Avalonia.Controls.ApplicationLifetimes;
using Legion.Models;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using Splat;

namespace Legion.ViewModels;

public class MainMenuViewModel : ViewModelBase
{
    private readonly ApplicationDbContext _context;
    private bool _userManagingVisible;
    private bool _reportsVisible;

    public MainMenuViewModel(ApplicationDbContext context, IScreen? hostScreen = null)
    {
        _context = context;
        var user = Locator.Current.GetService<User>()!;
        _context.UserRoles.Load();
        UserManagingVisible = user.UserRole != null && user.UserRole.CanManageUsers;
        ReportsVisible = user.UserRole != null && user.UserRole.CanViewReports;
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

        UsersCommand =
            ReactiveCommand.Create(() => { HostScreen.Router.Navigate.Execute(new UserViewModel(_context)); });

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

    public bool ReportsVisible
    {
        get => _reportsVisible;
        set => this.RaiseAndSetIfChanged(ref _reportsVisible, value);
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