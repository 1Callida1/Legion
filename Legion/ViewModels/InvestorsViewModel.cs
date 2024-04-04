﻿using Avalonia;
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
using DynamicData.Binding;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using Splat;
using DynamicData;
using Avalonia.Controls.ApplicationLifetimes;

namespace Legion.ViewModels
{
    public class InvestorsViewModel : ViewModelBase
    {
        private ApplicationDbContext _context;
        private bool _isPaneOpen;
        private string _searchText = null!;
        private ObservableCollection<Investor> _investors = null!;
        private double _viewHeight = 780;
        private double _menuHeight = 0;
        private double _dataGridHeight = 0;
        private string _invCitySearchText = null!;
        private string _invRegistrationSearchText = null!;
        private DateTimeOffset _searchDateTime = new(DateTime.Now);
        private bool _filterState = false;
        private int _filterDateState = 0;

        public InvestorsViewModel()
        {
            _context = new ApplicationDbContext(new DbContextOptions<ApplicationDbContext>());
        }

        public InvestorsViewModel(ApplicationDbContext context, IScreen? hostScreen = null)
        {
            ViewHeight = 780;
            _context = context;
            _isPaneOpen = false;
            _context.Investors.Load();
            Investors = _context.Investors.Local.ToObservableCollection();
            var a = Locator.Current.GetService<IClassicDesktopStyleApplicationLifetime>();

            if (a is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow.WhenAnyValue(s => s.Height).Subscribe((x) => ViewHeight = x - 20);
            }


            HostScreen = hostScreen ?? Locator.Current.GetService<IScreen>()!;

            IsSearchTextExist = this.WhenAnyValue(
                x => x.SearchText,
                (text) =>
                    !string.IsNullOrWhiteSpace(text)
            );

            SearchCommand = ReactiveCommand.Create(() =>
            {
                Investors = new ObservableCollection<Investor>();
                SearchText.Split(' ').ToList().ForEach(word => Investors.Add(_context.Investors.Where(inv => inv.LastName.ToLower().Contains(word.ToLower()) || inv.FirstName.ToLower().Contains(word.ToLower()) || inv.MiddleName.ToLower().Contains(word.ToLower()) || inv.Email.ToLower().Contains(word.ToLower()) || inv.Phone.ToLower().Contains(word.ToLower()) || inv.City.ToLower().Contains(word.ToLower()) || inv.DateBirth.ToString().Contains(word.ToLower()))));
                Investors = new ObservableCollection<Investor>(Investors.Distinct());
            }, IsSearchTextExist);

            PaneCommand = ReactiveCommand.Create(() =>
            {
                IsPaneOpen = !IsPaneOpen;
            });

            NewInvestorCommand = ReactiveCommand.Create(() =>
            {
                HostScreen.Router.Navigate.Execute(new AddInvestorViewModel(context));
            });

            DataGridEditActionCommand = ReactiveCommand.Create((Investor inv) =>
            {
                HostScreen.Router.Navigate.Execute(new AddInvestorViewModel(inv, context));
            });


            DataGridPrintActionCommand = ReactiveCommand.Create((Investor inv) =>
            {
                Debug.WriteLine(inv.ToString());
            });

            DataGridRemoveActionCommand = ReactiveCommand.Create((Investor inv) =>
            {
                Debug.WriteLine(inv.Id.ToString() + "to remove");
                _context.Investors.Remove(inv);
                _context.SaveChangesAsync();
                _context.Investors.LoadAsync();
            });

            BackCommand = ReactiveCommand.Create(() =>
            {
                HostScreen.Router.NavigateBack.Execute();
            });

            FilterCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                if(!FilterState)
                {
                    Investors = _context.Investors.Local.ToObservableCollection();
                    return;
                }

                Investors = new ObservableCollection<Investor>();

                if (!string.IsNullOrWhiteSpace(InvCitySearchText))
                {
                    if(!string.IsNullOrWhiteSpace(InvRegistrationSearchText))
                    {

                        switch (FilterDateState)
                        {
                            case 0:
                                _context.Investors
                                    .Where(i => i.PassportRegistration != null && i.City.Contains(InvCitySearchText) &&
                                                i.PassportRegistration.Contains(InvRegistrationSearchText)).ToList()
                                    .ForEach(i => Investors.Add(i));
                                break;
                            case 1:
                                _context.Investors
                                    .Where(i => i.PassportRegistration != null && i.City.Contains(InvCitySearchText) &&
                                                i.PassportRegistration.Contains(InvRegistrationSearchText) &&
                                                i.DateBirth > SearchDateTime).ToList()
                                    .ForEach(i => Investors.Add(i));
                                break;
                            case 2:
                                _context.Investors
                                    .Where(i => i.PassportRegistration != null && i.City.Contains(InvCitySearchText) &&
                                                i.PassportRegistration.Contains(InvRegistrationSearchText) &&
                                                i.DateBirth < SearchDateTime).ToList()
                                    .ForEach(i => Investors.Add(i));
                                break;
                            case 3:
                                _context.Investors
                                    .Where(i => i.PassportRegistration != null && i.City.Contains(InvCitySearchText) &&
                                                i.PassportRegistration.Contains(InvRegistrationSearchText) &&
                                                i.DateBirth == SearchDateTime).ToList()
                                    .ForEach(i => Investors.Add(i));
                                break;

                        }
                    }
                }
            });
        }

        public ObservableCollection<Investor> Investors
        {
            get => _investors;
            set => this.RaiseAndSetIfChanged(ref _investors, value);
        }

        public sealed override IScreen HostScreen { get; set; } = null!;

        public bool IsPaneOpen
        {
            get => _isPaneOpen;
            set => this.RaiseAndSetIfChanged(ref _isPaneOpen, value);
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (value.Length == 0)
                {
                    Investors = _context.Investors.Local.ToObservableCollection();
                }
                this.RaiseAndSetIfChanged(ref _searchText, value);
                
            }
        }

        public string InvCitySearchText
        {
            get => _invCitySearchText;
            set => this.RaiseAndSetIfChanged(ref _invCitySearchText, value);
        }

        public string InvRegistrationSearchText
        {
            get => _invRegistrationSearchText;
            set => this.RaiseAndSetIfChanged(ref _invRegistrationSearchText, value);
        }

        public DateTimeOffset SearchDateTime
        {
            get => _searchDateTime;
            set
            {
                if (value == null)
                    return;

                this.RaiseAndSetIfChanged(ref _searchDateTime, value);
            }
        }

        public double ViewHeight {
            get => _viewHeight;
            set
            {
                this.RaiseAndSetIfChanged(ref _viewHeight, value);
                MenuHeight = value - 50;
                DataGridHeight = value - 100;
            }
        }

        public double DataGridHeight
        {
            get => _dataGridHeight;
            set => this.RaiseAndSetIfChanged(ref _dataGridHeight, value);
        }

        public bool FilterState
        {
            get => _filterState;
            set => this.RaiseAndSetIfChanged(ref _filterState, value);
        }

        public double MenuHeight { get => _menuHeight; set => this.RaiseAndSetIfChanged(ref _menuHeight, value); }

        public ReactiveCommand<Unit, Unit> BackCommand { get; } = null!;
        public IObservable<bool> IsSearchTextExist { get; } = null!;
        public DateOnly InvDobDPDate { get; }
        public int FilterDateState { get => _filterDateState;
            set => this.RaiseAndSetIfChanged(ref _filterDateState, value);
        }
        public ReactiveCommand<Investor, Unit> SerachCommand { get; set; } = null!;
        public ReactiveCommand<Unit, Unit> PaneCommand { get;} = null!;
        public ReactiveCommand<Unit, Unit> NewInvestorCommand { get; } = null!;
        public ReactiveCommand<Unit, Unit> SearchCommand { get; } = null!;
        public ReactiveCommand<Investor, Unit> DataGridPrintActionCommand { get; set; } = null!;
        public ReactiveCommand<Investor, Unit> DataGridEditActionCommand { get; set; } = null!;
        public ReactiveCommand<Investor, Unit> DataGridRemoveActionCommand { get; set; } = null!;
        public ReactiveCommand<Unit, Unit> FilterCommand { get; set; } = null!;
    }
}
