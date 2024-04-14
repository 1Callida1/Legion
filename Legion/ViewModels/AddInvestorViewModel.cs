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
    public class AddInvestorViewModel : ViewModelBase
    {
        private ApplicationDbContext _context;
        private Investor _investor = null!;
        private DateTimeOffset _passportGivenDate;
        private DateTimeOffset _birthDate;

        public AddInvestorViewModel(Investor investor, ApplicationDbContext context, IScreen? hostScreen = null) : this(
            context, hostScreen)
        {
            Investor = investor;
            SubmitText = "Редактировать инвестора";
            //SaveCommand = ReactiveCommand.Create(UpdInvestor(ref investor, investor))
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

#if DEBUG
            City = Faker.Address.City();
            Email = Faker.Internet.Email();
            FirstName = Faker.Name.First();
            LastName = Faker.Name.Last();
            MiddleName = Faker.Name.Middle();
            PassportGiven = Faker.Address.City();
            PassportSeries = new Random().Next(1111, 9999).ToString();
            PassportNumber = new Random().Next(111111, 999999).ToString();
            Phone = Faker.Phone.Number();
#endif

            BackCommand = ReactiveCommand.Create(() =>
            {
                HostScreen.Router.NavigateBack.Execute();
            });

            SaveCommand = ReactiveCommand.Create(() =>
            {
                if (!string.IsNullOrWhiteSpace(Investor.CardNumber))
                    Investor.PayType = true;

                _context.Investors.Add(Investor);

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
                x => x.Card,
                card =>
                {
                    if (string.IsNullOrEmpty(card))
                        return true;

                    if (Regex.IsMatch(card, "(\\d{4} ){3}\\d{4}.*"))
                        return true;

                    return false;
                },
                "Номер карты 16 или 18 цифр");

            this.ValidationRule(
                x => x.FirstName,
                firstName => !string.IsNullOrWhiteSpace(firstName),
                "Некорректное имя");

            this.ValidationRule(
                x => x.LastName,
                lastName => !string.IsNullOrWhiteSpace(lastName),
                "Некорректная фамилия");

            this.ValidationRule(
                x => x.MiddleName,
                middleName => !string.IsNullOrWhiteSpace(middleName),
                "Некорректное отчество");

            this.ValidationRule(
                x => x.City,
                city => !string.IsNullOrWhiteSpace(city),
                "Некорректный город");

            this.ValidationRule(
                x => x.Phone,
                phone => !string.IsNullOrWhiteSpace(phone),
                "Некорректный телефон");

            this.ValidationRule(
                x => x.Email,
                firstName =>
                {
                    if (!string.IsNullOrWhiteSpace(firstName) && Regex.IsMatch(Email, ".*@.*\\..*"))
                        return true;

                    return false;
                },
                "Некорректный Email");

            this.ValidationRule(
                x => x.PassportSeries,
                pSeries =>
                {
                    if (!string.IsNullOrWhiteSpace(pSeries) && Regex.IsMatch(pSeries, "\\d{4}"))
                        return true;

                    return false;
                },
                "Некорректная серия");

            this.ValidationRule(
                x => x.PassportNumber,
                pNumber =>
                {
                    if (!string.IsNullOrWhiteSpace(pNumber) && Regex.IsMatch(pNumber, "\\d{6}"))
                        return true;

                    return false;
                },
                "Некорректный номер");

            this.ValidationRule(
                x => x.PassportGiven,
                pNumber => !string.IsNullOrWhiteSpace(pNumber),
                "Некорректное значение");
        }

        public DateTimeOffset PassportGivenDate
        {
            get => new(Investor.PassportDateGiven);
            set
            {
                if (value == null)
                    return;

                _passportGivenDate = value;
                Investor.PassportDateGiven = _passportGivenDate.Date;
                this.RaisePropertyChanged();
            }
        }

        public DateTimeOffset BirthDate
        {
            get => new(Investor.DateBirth);
            set
            {
                if (value == null)
                    return;

                _birthDate = value;
                Investor.DateBirth = _birthDate.Date;
                this.RaisePropertyChanged();
            }
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

        public string? FirstName
        {
            get => Investor.FirstName;
            set
            {
                if(value == null)
                    return;

                Investor.FirstName = value;
                this.RaisePropertyChanged();
            }
        }

        public string? LastName
        {
            get => Investor.LastName;
            set
            {
                if(value == null)
                    return;
                Investor.LastName = value;
                this.RaisePropertyChanged();
            }
        }

        public string? MiddleName
        {
            get => Investor.MiddleName;
            set
            {
                if (value == null)
                    return;

                Investor.MiddleName = value;
                this.RaisePropertyChanged();
            }
        }

        public string? City
        {
            get => Investor.City;
            set
            {
                if (value == null)
                    return;

                Investor.City = value;
                this.RaisePropertyChanged();
            }
        }

        public string? Phone
        {
            get => Investor.Phone;
            set
            {
                if (value == null)
                    return;

                Investor.Phone = value;
                this.RaisePropertyChanged();
            }
        }

        public string? Email
        {
            get => Investor.Email;
            set
            {
                if (value == null)
                    return;

                Investor.Email = value;
                this.RaisePropertyChanged();
            }
        }

        public string? PassportSeries
        {
            get => Investor.PassprotSeries;
            set
            {
                if (value == null)
                    return;

                Investor.PassprotSeries = value;
                this.RaisePropertyChanged();
            }
        }

        public string? PassportNumber
        {
            get => Investor.PassprotNumber;
            set
            {
                if (value == null)
                    return;

                Investor.PassprotNumber = value;
                this.RaisePropertyChanged();
            }
        }

        public string? PassportGiven
        {
            get => Investor.Given;
            set
            {
                if (value == null)
                    return;

                Investor.Given = value;
                this.RaisePropertyChanged();
            }
        }

        public List<string> Cities => _context.Investors.Select(i => i.City).Distinct().ToList();

        public ReactiveCommand<Unit, Unit> BackCommand { get; }
        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
        public string SubmitText { get; protected set; }
        public Investor Investor { get => _investor; set => this.RaiseAndSetIfChanged(ref _investor, value); }

        public sealed override IScreen HostScreen { get; set; }
    }
}