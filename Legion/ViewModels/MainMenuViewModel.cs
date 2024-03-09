using Avalonia;
using Legion.Models;
using Legion.ViewModels;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Legion.ViewModels
{
    public class MainMenuViewModel : ViewModelBase
    {

        public MainMenuViewModel()
        {
            
        }

        public MainMenuViewModel(IScreen hostScreen, ApplicationDbContext context)
        {

        }


        public override IScreen HostScreen => throw new NotImplementedException();
    }
}
