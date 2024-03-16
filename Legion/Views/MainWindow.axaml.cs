using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Legion.Models;
using Legion.ViewModels;
using ReactiveUI;
using Splat;
using System.Threading.Tasks;

namespace Legion.Views
{
    public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        public MainWindow()
        {
            // ViewModel's WhenActivated block will also get called.
            this.WhenActivated(disposables => { /* Handle view activation etc. */ });
            AvaloniaXamlLoader.Load(this);
        }

        public async Task DoShowDialogAsync(InteractionContext<InvestorSerachViewModel,
            Investor?> interaction)
        {
            var dialog = new InvestorSearchWindow();
            dialog.DataContext = interaction.Input;

            var result = await dialog.ShowDialog<Investor?>(this);
            interaction.SetOutput(result);
        }
    }
}

