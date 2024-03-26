using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Legion.ViewModels;
using ReactiveUI;
using Splat;
using System;

namespace Legion.Views
{
    public partial class ContractsView : ReactiveUserControl<ContractsViewModel>
    {
        public ContractsView()
        {
            AvaloniaXamlLoader.Load(this);

            this.WhenActivated(action =>
                action(ViewModel!.ShowDialog.RegisterHandler(Locator.Current.GetService<MainWindow>()!.DoShowMoneyDialogAsync!)));

            this.WhenActivated(action =>
                action(ViewModel!.ShowAdditionalPaymentsDialog.RegisterHandler(Locator.Current.GetService<MainWindow>()!.DoShowAdditionalPaymentsDialogAsync!)));

            this.WhenActivated(action =>
                action(ViewModel!.ShowPaymentsDialog.RegisterHandler(Locator.Current.GetService<MainWindow>()!.DoShowPaymentsDialogAsync!)));
        }
    }
}