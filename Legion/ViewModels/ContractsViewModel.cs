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
using Legion.Views;
using System.Diagnostics.Contracts;

namespace Legion.ViewModels
{
    public class ContractsViewModel : ViewModelBase
    {
        private ApplicationDbContext _context;
        private bool _isPaneOpen;

        public ContractsViewModel()
        {
            _context = new ApplicationDbContext(new DbContextOptions<ApplicationDbContext>());
        }

        public ContractsViewModel(IScreen hostScreen, ApplicationDbContext context)
        {
            _context = context;
            _isPaneOpen = false;
            _context.Contracts.Load();

            PaneCommand = ReactiveCommand.Create(() =>
            {
                IsPaneOpen = !IsPaneOpen;
            });

            NewContractCommand = ReactiveCommand.Create(() =>
            {
                hostScreen.Router.Navigate.Execute(new AddContractViewModel(hostScreen, context));
            });

            DataGridEditActionCommand = ReactiveCommand.Create((Legion.Models.Contract ctr) =>
            {
                hostScreen.Router.Navigate.Execute(new AddContractViewModel(ctr, hostScreen, context));
            });


            DataGridPrintActionCommand = ReactiveCommand.Create((Legion.Models.Contract ctr) =>
            {
                Debug.WriteLine(ctr.ToString());
            });

            DataGridRemoveActionCommand = ReactiveCommand.Create((Legion.Models.Contract ctr) =>
            {
                Debug.WriteLine(ctr.Id.ToString() + "to remove");
                _context.Contracts.Remove(ctr);
                _context.SaveChangesAsync();
                _context.Investors.LoadAsync();
            });

            BackCommand = ReactiveCommand.Create(() =>
            {
                hostScreen.Router.NavigateBack.Execute();
            });
        }

        public ObservableCollection<Legion.Models.Contract> Contracts => _context.Contracts.Local.ToObservableCollection();
        public override IScreen HostScreen => throw new NotImplementedException();
        public bool IsPaneOpen
        {
            get => _isPaneOpen;
            set => this.RaiseAndSetIfChanged(ref _isPaneOpen, value);
        }

        public ReactiveCommand<Unit, Unit> BackCommand { get; }

        public ReactiveCommand<Unit, Unit> PaneCommand { get; }
        public ReactiveCommand<Unit, Unit> NewContractCommand { get; }
        public ReactiveCommand<Legion.Models.Contract, Unit> DataGridPrintActionCommand { get; set; }
        public ReactiveCommand<Legion.Models.Contract, Unit> DataGridEditActionCommand { get; set; }
        public ReactiveCommand<Legion.Models.Contract, Unit> DataGridRemoveActionCommand { get; set; }
    }
}
