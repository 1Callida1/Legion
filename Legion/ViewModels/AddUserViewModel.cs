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
using System.Collections;
using System.ComponentModel;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using ReactiveUI.Validation.Extensions;
using Splat;
using Material.Styles.Controls;

namespace Legion.ViewModels
{
    public class AddUserViewModel : ViewModelBase
    {
        private readonly ApplicationDbContext _context;
        private User _user = null!;

        public AddUserViewModel(User user, ApplicationDbContext context, IScreen? hostScreen = null) : this(
            context, hostScreen)
        {
            User = user;
            SubmitText = "Редактировать пользователя";
            _context.UserRoles.Load();

            SaveCommand = ReactiveCommand.Create(() =>
            {
                _context.Users.Update(User);

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

        public AddUserViewModel(ApplicationDbContext context, IScreen? hostScreen = null)
        {
            HostScreen = hostScreen ?? Locator.Current.GetService<IScreen>()!;
            _context = context;
            User = new User();
            SubmitText = "Добавить пользователя";
            _context.UserRoles.Load();
            Role = UserRoles.First();

            BackCommand = ReactiveCommand.Create(() =>
            {
                HostScreen.Router.NavigateBack.Execute();
            });

            SaveCommand = ReactiveCommand.Create(() =>
            {
                _context.Users.Add(User);

                try
                {
                    _context.SaveChanges();
                    HostScreen.Router.NavigateBack.Execute();
                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message);
                }
            }, this.IsValid());

            this.ValidationRule(
                x => x.UserName,
                userName => !string.IsNullOrWhiteSpace(userName),
                "Некорректное имя");

            this.ValidationRule(
                x => x.Password,
                password => !string.IsNullOrWhiteSpace(password),
                "Некорректный пароль");
        }

        public string UserName
        {
            get => User.UserName;
            set
            {
                User.UserName = value;
                this.RaisePropertyChanged();
            }
        }

        public string Password
        {
            get => User.Password;
            set
            {
                User.Password = value;
                this.RaisePropertyChanged();
            }
        }

        public UserRole Role
        {
            get => User.UserRole;
            set
            {
                User.UserRole = value;
                this.RaisePropertyChanged();
            }
        }

        public ObservableCollection<UserRole> UserRoles => _context.UserRoles.Local.ToObservableCollection();
        public ReactiveCommand<Unit, Unit> BackCommand { get; } = null!;
        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
        public string SubmitText { get; protected set; }
        public User User { get => _user; set => this.RaiseAndSetIfChanged(ref _user, value); }
        public sealed override IScreen HostScreen { get; set; }
    }
}
