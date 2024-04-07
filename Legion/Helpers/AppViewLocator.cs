using Legion.Views;
using Legion.ViewModels;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Legion.Helpers
{
    public class AppViewLocator : IViewLocator
    {
        public IViewFor ResolveView<T>(T? viewModel, string? contract = null) => viewModel switch
        {
            AddContractViewModel context => new AddContractView() { DataContext = context },
            AddInvestorViewModel context => new AddInvestorView() { DataContext = context },
            ContractsViewModel context => new ContractsView() { DataContext = context },
            InvestorSerachViewModel context => new InvestorSerachView() { DataContext = context },
            InvestorsViewModel context => new InvestorsView() { DataContext = context },
            LoginViewModel context => new LoginView() { DataContext = context },
            MainMenuViewModel context => new MainMenuView() { DataContext = context },
            MainWindowViewModel context => new MainWindow() { DataContext = context },
            ReferralViewModel context => new ReferralView() { DataContext = context },
            ReportsViewModel context => new ReportsView() { DataContext = context },
            UserViewModel context => new UserView() { DataContext = context },
            AddUserViewModel context => new AddUserView() { DataContext = context },
            AddIntegerViewModel context => new AddIntegerWindow() {DataContext = context},
            ExpiringContractViewModel context => new ExpiringContractView() { DataContext = context },
            AdditionalPaymentsHistoryViewModel context => new AdditionalPaymentsHistoryWindow() { DataContext = context },
            PaymentsHistoryViewModel context => new PaymentsHistoryWindow() { DataContext = context },
            SettingsViewModel context => new SettingsView() { DataContext = context },
            _ => throw new ArgumentOutOfRangeException(nameof(viewModel))
        };
        //{
        //    // Find view's by chopping of the 'Model' on the view model name
        //    // MyApp.ShellViewModel => MyApp.ShellView
        //    var viewModelName = viewModel.GetType().Name;
        //    var viewTypeName = "Legion.Views." + viewModelName.TrimEnd("Model".ToCharArray());
        //    Type.GetType(viewTypeName) context => new FirstView { DataContext = context },
        //    _ => throw new ArgumentOutOfRangeException(nameof(viewModel))
        //    try
        //    {
        //        var viewType = Type.GetType(viewTypeName);
        //        if (viewType == null)
        //        {
        //            this.Log().Error($"Could not find the view {viewTypeName} for view model {viewModelName}.");
        //            return null;
        //        }
        //        return Activator.CreateInstance(viewType) as IViewFor;
        //    }
        //    catch (Exception)
        //    {
        //        this.Log().Error($"Could not instantiate view {viewTypeName}.");
        //        throw;
        //    }
        //}
    }
}
