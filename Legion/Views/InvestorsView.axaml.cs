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

    //private void AddNewInvestor(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    //{
    //    SPAddNewInvestor.IsVisible = true;
    //    BtnBack.IsVisible = true;
    //    BtnAddNew.IsVisible = false;
    //    SPInvestorDG.IsVisible = false;
    //    SVLeft.IsPaneOpen = false;
    //    SerachBox.IsVisible = false;
    //    SerachBtn.IsVisible = false;
    //    SpTriggerBtn.IsVisible = false;
    //}

    //private void BackBtnClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    //{
    //    SPAddNewInvestor.IsVisible = false;
    //    BtnBack.IsVisible = false;
    //    BtnAddNew.IsVisible = true;
    //    SPInvestorDG.IsVisible = true;
    //    SerachBox.IsVisible = true;
    //    SerachBtn.IsVisible = true;
    //    SpTriggerBtn.IsVisible = true;
    //}

    //private void TriggerPaneCommand(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    //{
    //    SVLeft.IsPaneOpen = !SVLeft.IsPaneOpen;
    //}
}