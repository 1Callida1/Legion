using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Legion.Models;
using ReactiveUI;
using Splat;

namespace Legion.ViewModels
{
    public class AddIntegerViewModel : ViewModelBase
    {
        private object? _amount = null;
        private string _helpText;
        private string _buttonText;

        public AddIntegerViewModel(string helpText, string buttonText)
        {
            IsValidated = this.WhenAnyValue(
                x => x.Amount,
                (text) =>
                {
                    if (string.IsNullOrWhiteSpace((string)text))
                        return false;

                    if (Regex.Count((string)text, "\\D") > 0)
                        return false;

                    return true;
                }
                    
            );
            SetCommand = ReactiveCommand.Create(() => Amount, IsValidated);
            CancelCommand = ReactiveCommand.Create(() =>
            {
                int? a = null;
                object? b = a;
                return b;
            })!;
            _helpText = helpText;
            _buttonText = buttonText;
        }

        public sealed override IScreen HostScreen { get; set; } = null!;

        public string HelpText
        {
            get => _helpText;
            set => this.RaiseAndSetIfChanged(ref _helpText, value);
        }

        public string ButtonText
        {
            get => _buttonText;
            set => this.RaiseAndSetIfChanged(ref _buttonText, value);
        }

        public object? Amount
        {
            get => _amount;
            set => this.RaiseAndSetIfChanged(ref _amount, value);
        }


        public IObservable<bool> IsValidated { get; } = null!;
        public ReactiveCommand<Unit, object?> SetCommand { get; }
        public ReactiveCommand<Unit, object?> CancelCommand { get; }

    }
}
