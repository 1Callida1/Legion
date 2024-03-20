﻿using Avalonia;
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
using Splat;
using System.Diagnostics.Contracts;

namespace Legion.ViewModels
{
    public class AddUserViewModel : ViewModelBase
    {
        private readonly ApplicationDbContext _context;

        public AddUserViewModel(ApplicationDbContext context, IScreen? hostScreen = null)
        {
            _context = context;
            HostScreen = hostScreen ?? Locator.Current.GetService<IScreen>()!;

            BackCommand = ReactiveCommand.Create(() =>
            {
                HostScreen.Router.NavigateBack.Execute();
            });
        }

        public ReactiveCommand<Unit, Unit> BackCommand { get; } = null!;
        public sealed override IScreen HostScreen { get; set; }
    }
}