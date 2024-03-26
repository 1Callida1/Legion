using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Legion.Models;
using Legion.ViewModels;
using ReactiveUI;
using Splat;

namespace Legion.Views
{
    public partial class AddContractView : ReactiveUserControl<AddContractViewModel>
    {
        public AddContractView()
        {
            AvaloniaXamlLoader.Load(this);

            this.WhenActivated(action =>
                action(ViewModel!.ShowDialog.RegisterHandler(Locator.Current.GetService<MainWindow>()!.DoShowDialogAsync)));
        }

        
    }
}
