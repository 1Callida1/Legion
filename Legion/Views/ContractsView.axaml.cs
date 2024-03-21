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
                action(ViewModel!.ShowDialog.RegisterHandler(Locator.Current.GetService<MainWindow>()!.DoShowMoneyDialogAsync)));
        }
    }
}