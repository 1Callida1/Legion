using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Legion.ViewModels;
using ReactiveUI;
using System;

namespace Legion.Views
{
    public partial class InvestorSerachView : ReactiveUserControl<InvestorSerachViewModel>
    {
        public InvestorSerachView()
        {
            this.WhenActivated(disposables => { });
            AvaloniaXamlLoader.Load(this);
        }
    }
}