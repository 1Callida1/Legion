using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Legion.ViewModels
{
    public abstract class ViewModelBase : ReactiveObject, IRoutableViewModel
    {
        // Reference to IScreen that owns the routable view model.
        public abstract IScreen HostScreen { get; }

        // Unique identifier for the routable view model.
        public string UrlPathSegment { get; } = Guid.NewGuid().ToString().Substring(0, 5);
    }
}
