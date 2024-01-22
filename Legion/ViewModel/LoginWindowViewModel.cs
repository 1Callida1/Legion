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

namespace Legion.ViewModel
{
    internal class LoginWindowViewModel : ReactiveObject
    {
        public LoginWindowViewModel()
        {
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
                if (!(UserName == "admin" && Password == "123"))
                    WrongData = true;

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
    }
}
