using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Legion.ViewModels;
using ReactiveUI;
using System;

namespace Legion.Views
{
    public partial class InvestorsView : ReactiveUserControl<InvestorsViewModel>
    {
        public InvestorsView()
        {
            this.WhenActivated(disposables => { });
            AvaloniaXamlLoader.Load(this);
        }
    }
}