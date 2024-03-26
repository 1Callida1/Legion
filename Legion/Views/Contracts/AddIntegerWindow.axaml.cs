using System;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using Legion.Models;
using Legion.ViewModels;
using ReactiveUI;


namespace Legion.Views
{
    public partial class AddIntegerWindow : ReactiveWindow<AddIntegerViewModel>
    {
        public AddIntegerWindow()
        {
            InitializeComponent();

            this.WhenActivated(d => d(ViewModel!.SetCommand.Subscribe(Close)));
            this.WhenActivated(d => d(ViewModel!.CancelCommand.Subscribe(Close)));
        }
    }
}
