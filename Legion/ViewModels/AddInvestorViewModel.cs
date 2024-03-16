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
using ReactiveUI.Validation.Extensions;
using Splat;

namespace Legion.ViewModels
{
    public class AddInvestorViewModel : ViewModelBase
    {
        private ApplicationDbContext _context;
        private Investor _investor = null!;
        private string _card = null!;

        public AddInvestorViewModel(Investor investor, ApplicationDbContext context, IScreen? hostScreen = null) : this(
            context, hostScreen)
        {
            Investor = investor;
            SubmitText = "Редактировать инвестора";
            SaveCommand = ReactiveCommand.Create(() =>
            {
                _context.Investors.Update(Investor);

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

        public AddInvestorViewModel(ApplicationDbContext context, IScreen? hostScreen = null)
        {
            HostScreen = hostScreen ?? Locator.Current.GetService<IScreen>()!;
            _context = context;
            Investor = new Investor();
            SubmitText = "Добавить инвестора";

            BackCommand = ReactiveCommand.Create(() =>
            {
                HostScreen.Router.NavigateBack.Execute();
            });

            SaveCommand = ReactiveCommand.Create(() =>
            {
                _context.Investors.Add(Investor);

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

           /* this.ValidationRule(
                x => x.Card,
                card =>
                {
                    if (string.IsNullOrEmpty(card))
                        return true;

                    if (Regex.IsMatch(card, "(\\d{4} ){4}.*"))
                        return true;

                    return false;
                },
                "Номер карты 16 или 18 цифр");*/
        }

        public string? Card
        {
            get => Investor.CardNumber;
            set
            {
                Investor.CardNumber = value;
                this.RaisePropertyChanged();
            }
        }

        public ReactiveCommand<Unit, Unit> BackCommand { get; }
        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
        public string SubmitText { get; protected set; }
        public Investor Investor { get => _investor; set => this.RaiseAndSetIfChanged(ref _investor, value); }

        public sealed override IScreen HostScreen { get; set; }
    }
}