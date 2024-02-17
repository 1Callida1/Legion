using Avalonia;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Legion.ViewModels
{
    internal class LoginWindowViewModel : ReactiveObject
    {
        private ApplicationDbContext _context;

        public LoginWindowViewModel(ApplicationDbContext context, InvestorsView investorsView, LoginWindow mainHandler)
        {
            _context = context;

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
                if (_context.Users.FirstOrDefault(user => user.UserName == UserName && user.Password == Password) == null)
                {
                    WrongData = true;
                    return;
                }
                mainHandler.Hide();
                investorsView.DataContext = new InvestorsViewModel(context);
                investorsView.Show();

            }, IsInputValid);


            if (_context.Users.FirstOrDefault(user => user.UserName == "admin") == null)
            {
                _context.Users.Add(new Model.User() { Password = "123", UserName = "admin" });
                _context.SaveChanges();
            }
            else
            {
                Debug.WriteLine($"Finded user {_context.Users.FirstOrDefault().UserName} in database");
            }
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
    }
}
