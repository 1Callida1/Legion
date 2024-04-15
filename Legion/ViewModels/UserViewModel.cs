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
using Legion.Helpers.Calculations;

namespace Legion.ViewModels
{
    public class UserViewModel : ViewModelBase
    {
        private readonly ApplicationDbContext _context;
        private ObservableCollection<User> _user = null!;
        private bool _loadingVisible = false;


        public UserViewModel(ApplicationDbContext context, IScreen? hostScreen = null)
        {
            _context = context;
            HostScreen = hostScreen ?? Locator.Current.GetService<IScreen>()!;
            LoadUsesAsync();

            BackCommand = ReactiveCommand.Create(() =>
            {
                HostScreen.Router.NavigateBack.Execute();
            });

            NewUserCommand = ReactiveCommand.Create(() =>
            {
                HostScreen.Router.Navigate.Execute(new AddUserViewModel(context));
            });

            DataGridEditActionCommand = ReactiveCommand.Create((User us) =>
            {
                HostScreen.Router.Navigate.Execute(new AddUserViewModel(us, context));
            });

            DataGridRemoveActionCommand = ReactiveCommand.CreateFromTask(async (User us) =>
            {
                Debug.WriteLine(us.Id.ToString() + "to remove");
                _context.Users.Remove(us);
                await _context.SaveChangesAsync();
                LoadUsesAsync();
            });

            RolesCommand = ReactiveCommand.Create(() =>
            {
                HostScreen.Router.Navigate.Execute(new RolesViewModel(context));
            });
        }

        public bool LoadingVisible
        {
            get => _loadingVisible;
            set => this.RaiseAndSetIfChanged(ref _loadingVisible, value);
        }

        private async Task LoadUsesAsync()
        {
            LoadingVisible = true;
            await _context.Users.LoadAsync();
            Users = _context.Users.Local.ToObservableCollection();
            await _context.UserRoles.LoadAsync();

            LoadingVisible = false;
        }

        public ObservableCollection<User> Users
        {
            get => _user;
            set => this.RaiseAndSetIfChanged(ref _user, value);
        }
        public ReactiveCommand<Unit, Unit> NewUserCommand { get; } = null!;
        public ReactiveCommand<Unit, Unit> BackCommand { get; } = null!;
        public ReactiveCommand<Unit, Unit> RolesCommand { get; } = null!;
        public sealed override IScreen HostScreen { get; set; }
        public ReactiveCommand<User, Unit> DataGridEditActionCommand { get; set; } = null!;
        public ReactiveCommand<User, Unit> DataGridRemoveActionCommand { get; set; } = null!;
    }
}
