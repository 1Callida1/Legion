using System;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using Legion.ViewModels;
using ReactiveUI;


namespace Legion.Views
{
    public partial class AdditionalPaymentsHistoryWindow : ReactiveWindow<AdditionalPaymentsHistoryViewModel>
    {
        public AdditionalPaymentsHistoryWindow()
        {
            InitializeComponent();

            this.WhenActivated(d => d(ViewModel!.BackCommand.Subscribe(Close)));
        }
    }
}
