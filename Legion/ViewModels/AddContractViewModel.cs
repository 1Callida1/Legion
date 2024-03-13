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
using System.Diagnostics.Contracts;

namespace Legion.ViewModels
{
    public class AddContractViewModel : ViewModelBase
    {
        private ApplicationDbContext _context;
        private Legion.Models.Contract _contract;

        public AddContractViewModel(Legion.Models.Contract contract, IScreen hostScreen, ApplicationDbContext context) : this(
            hostScreen, context)
        {
            Contract = contract;
            SubmitText = "Редактировать договор";
            SaveCommand = ReactiveCommand.Create(() =>
            {
                _context.Contracts.Update(Contract);

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

        public AddContractViewModel(IScreen hostScreen, ApplicationDbContext context)
        {
            HostScreen = hostScreen;
            _context = context;
            Contract = new Legion.Models.Contract();
            SubmitText = "Добавить новый договор";

            BackCommand = ReactiveCommand.Create(() =>
            {
                hostScreen.Router.NavigateBack.Execute();
            });

            SaveCommand = ReactiveCommand.Create(() =>
            {
                _context.Contracts.Add(Contract);

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

        public ReactiveCommand<Unit, Unit> BackCommand { get; }
        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
        public string SubmitText { get; protected set; }
        public Legion.Models.Contract Contract { get => _contract; set => this.RaiseAndSetIfChanged(ref _contract, value); }

        public override IScreen HostScreen { get; }
    }
}