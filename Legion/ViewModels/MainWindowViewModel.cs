using Avalonia.Controls.ApplicationLifetimes;
using Legion.Views;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;

namespace Legion.ViewModels
{
    public class MainWindowViewModel : ReactiveObject, IScreen
    {
        private int _mainWindowState;
        private int _lastSizeState;
        private bool _isInternalStateChanging;

        public RoutingState Router { get; } = new RoutingState();

        // The command that navigates a user to first view model.
        public ReactiveCommand<Unit, IRoutableViewModel> GoNext { get; }

        // The command that navigates a user back.
        public ReactiveCommand<Unit, IRoutableViewModel> GoBack => Router.NavigateBack!;

        public MainWindowViewModel(ApplicationDbContext context)
        {
            _lastSizeState = 0; // normal
            _isInternalStateChanging = false;
            // Manage the routing state. Use the Router.Navigate.Execute
            // command to navigate to different view models. 
            //
            // Note, that the Navigate.Execute method accepts an instance 
            // of a view model, this allows you to pass parameters to 
            // your view models, or to reuse existing view models.
            //
            ExitCommand = ReactiveCommand.Create(() =>
            {
                Locator.Current.GetService<IClassicDesktopStyleApplicationLifetime>()!.Shutdown();
            });

            ResizeCommand = ReactiveCommand.Create(() =>
            {
                if (MainWindowState == 3)
                {
                    _isInternalStateChanging = true;
                    MainWindowState = 0; //normal
                    _isInternalStateChanging = false;
                }
                else
                {
                    MainWindowState = 3; //fullscreen
                }
            });

            HideCommand = ReactiveCommand.Create(() =>
            {
                MainWindowState = 1;
            });


#if DEBUG
            Locator.CurrentMutable.RegisterConstant(context.Users.First()!);
            GoNext = ReactiveCommand.CreateFromObservable(
                () => Router.Navigate.Execute(new MainMenuViewModel(context))
            );
            
#else

            GoNext = ReactiveCommand.CreateFromObservable(
                () => Router.Navigate.Execute(new LoginViewModel(context))
            );
#endif
        }

        public int MainWindowState
        {
            get => _mainWindowState;
            set
            {
                var toSet = value;

                //saving resize state after hide window
                switch (value)
                {
                    case 1:
                        _lastSizeState = MainWindowState;
                        break;
                    case 0:
                        toSet = _isInternalStateChanging ? value : _lastSizeState;
                        break;
                }
                this.RaiseAndSetIfChanged(ref _mainWindowState, toSet);
            }
        }
        public ReactiveCommand<Unit, Unit> ExitCommand { get; }
        public ReactiveCommand<Unit, Unit> ResizeCommand { get; }
        public ReactiveCommand<Unit, Unit> HideCommand { get; }
    }
}
