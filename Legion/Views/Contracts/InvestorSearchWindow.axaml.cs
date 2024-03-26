using System;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using Legion.ViewModels;
using ReactiveUI;

namespace Legion.Views
{
    public partial class InvestorSearchWindow : ReactiveWindow<InvestorSerachViewModel>
    {
        public InvestorSearchWindow()
        {
            InitializeComponent();

            this.WhenActivated(d => d(ViewModel!.BackCommand.Subscribe(Close)));
            this.WhenActivated(d => d(ViewModel!.SelectInvestor.Subscribe(Close)));
        }
    }
}
