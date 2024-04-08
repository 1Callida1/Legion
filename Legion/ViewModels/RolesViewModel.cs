using Legion.Models;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace Legion.ViewModels
{
    public class RolesViewModel : ViewModelBase
    {
        private readonly ApplicationDbContext _context;
        private ObservableCollection<UserRole> _roles = null!;

        public RolesViewModel(ApplicationDbContext context, IScreen? hostScreen = null)
        {
            _context = context;
            HostScreen = hostScreen ?? Locator.Current.GetService<IScreen>()!;
            _context.UserRoles.Load();
            Roles = _context.UserRoles.Local.ToObservableCollection();

            BackCommand = ReactiveCommand.Create(() =>
            {
                HostScreen.Router.NavigateBack.Execute();
            });

            NewRoleCommand = ReactiveCommand.Create(() =>
            {
                HostScreen.Router.Navigate.Execute(new AddRoleViewModel(context));
            });

            DataGridEditActionCommand = ReactiveCommand.Create((UserRole us) =>
            {
                if (us.Role == "Admin")
                    return;
                HostScreen.Router.Navigate.Execute(new AddRoleViewModel(us, context));
            });

            DataGridRemoveActionCommand = ReactiveCommand.CreateFromTask(async (UserRole us) =>
            {
                Debug.WriteLine(us.Id.ToString() + "to remove");
                if(us.Role == "Admin")
                    return;
                _context.UserRoles.Remove(us);
                await _context.SaveChangesAsync();
                await _context.UserRoles.LoadAsync();
            });
        }

        public ObservableCollection<UserRole> Roles
        {
            get => _roles;
            set => this.RaiseAndSetIfChanged(ref _roles, value);
        }

        public ReactiveCommand<Unit, Unit> NewRoleCommand { get; } = null!;
        public ReactiveCommand<Unit, Unit> BackCommand { get; } = null!;
        public sealed override IScreen HostScreen { get; set; }
        public ReactiveCommand<UserRole, Unit> DataGridEditActionCommand { get; set; } = null!;
        public ReactiveCommand<UserRole, Unit> DataGridRemoveActionCommand { get; set; } = null!;
    }
}
