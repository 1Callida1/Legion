using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Legion.ViewModels;
using ReactiveUI;

namespace Legion.Views
{
    public partial class ContractView : ReactiveUserControl<ContractViewModel>
    {
        public ContractView()
        {
            this.WhenActivated(disposables => { });
            AvaloniaXamlLoader.Load(this);

        }
    }
}

