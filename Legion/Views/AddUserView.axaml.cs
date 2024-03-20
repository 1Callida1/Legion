using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Legion.ViewModels;
using ReactiveUI;
using System;

namespace Legion.Views
{
    public partial class AddUserView : ReactiveUserControl<AddUserViewModel>
    {
        public AddUserView()
        {
            this.WhenActivated(disposables => { });
            AvaloniaXamlLoader.Load(this);
        }
    }
}