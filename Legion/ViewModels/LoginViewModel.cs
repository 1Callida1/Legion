using Avalonia;
using Legion.Models;
using Legion.ViewModels;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Legion.Views;
using System.Reactive.Disposables;
using Avalonia.Platform;
using Splat;

namespace Legion.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private ApplicationDbContext _context;
        public override IScreen HostScreen { get; set; }

        public LoginViewModel(ApplicationDbContext context, IScreen? hostScreen = null)
        {
            Activator = new ViewModelActivator();
            _context = context;
            HostScreen = hostScreen ?? Locator.Current.GetService<IScreen>()!;


            IsInputValid = this.WhenAnyValue(
                x => x.UserName, 
                x => x.Password,
                (username, password) =>
                    !string.IsNullOrWhiteSpace(username) &&
                    !string.IsNullOrWhiteSpace(password)
            );

            SubmitCommand = ReactiveCommand.Create(() =>
            {
                Debug.WriteLine($"{UserName} : {Password}");

                User? authenticateUser = _context.Users.FirstOrDefault(user =>
                    user.UserName == UserName.ToLower() && user.Password == Password);

                if (authenticateUser == null)
                {
                    WrongData = true;
                    return;
                }

                Locator.CurrentMutable.RegisterConstant(authenticateUser);
                HostScreen.Router.Navigate.Execute(new MainMenuViewModel(_context));

            }, IsInputValid);
        }

        private string _username = string.Empty;
        private string _password = string.Empty;
        private bool _wrongData = false;

        public IObservable<bool> IsInputValid { get; }
        public bool WrongData { 
            get => _wrongData;
            set => this.RaiseAndSetIfChanged(ref _wrongData, value);
        }
        public ReactiveCommand<Unit, Unit> SubmitCommand { get; }
        public string UserName
        {
            get => _username;
            set => this.RaiseAndSetIfChanged(ref _username, value);
        }

        public string Password
        {
            get => _password;
            set => this.RaiseAndSetIfChanged(ref _password, value);
        }

        public ViewModelActivator Activator { get; }
    };
}
