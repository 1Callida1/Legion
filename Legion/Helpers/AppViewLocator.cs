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
        public IViewFor ResolveView<T>(T viewModel, string contract = null)
        {
            // Find view's by chopping of the 'Model' on the view model name
            // MyApp.ShellViewModel => MyApp.ShellView
            var viewModelName = viewModel.GetType().Name;
            var viewTypeName = "Legion.Views." + viewModelName.TrimEnd("Model".ToCharArray());

            try
            {
                var viewType = Type.GetType(viewTypeName);
                if (viewType == null)
                {
                    this.Log().Error($"Could not find the view {viewTypeName} for view model {viewModelName}.");
                    return null;
                }
                return Activator.CreateInstance(viewType) as IViewFor;
            }
            catch (Exception)
            {
                this.Log().Error($"Could not instantiate view {viewTypeName}.");
                throw;
            }
        }
    }
}
