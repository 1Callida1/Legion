using Legion.Models;
using ReactiveUI;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Legion.Models.Internal;
using Splat;
using System.Reactive;
using Avalonia.Controls;
using Legion.Helpers;
using Legion.Views;

namespace Legion.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private string _templatesPath;
        private string _archievPath;

        public SettingsViewModel(IScreen? hostScreen = null)
        {
            _templatesPath = Settings.TemplatesFolder;
            _archievPath = Settings.ArchievFolder;
            HostScreen = hostScreen ?? Locator.Current.GetService<IScreen>()!;

            BackCommand = ReactiveCommand.Create(() =>
            {
                HostScreen.Router.NavigateBack.Execute();
            });

            OpenTemplateFolderCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var ofd = new OpenFolderDialog()
                {
                    Title = "Выберите папку с шаблонами"
                };
                TemplatesPath = await ofd.ShowAsync(Locator.Current.GetService<MainWindow>()!);
            });

            OpenArchievFolderCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var ofd = new OpenFolderDialog()
                {
                    Title = "Выберите папку с архивом"
                };
                ArchievPath = await ofd.ShowAsync(Locator.Current.GetService<MainWindow>()!);
            });

            SaveCommand = ReactiveCommand.Create( () =>
            {
                SettingsHelper.SaveSettings(Settings);
            });
        }

        public string TemplatesPath
        {
            get => _templatesPath;
            set
            {
                this.RaiseAndSetIfChanged(ref _templatesPath, value);
                Settings.TemplatesFolder = value;
            }
        }

        public string ArchievPath
        {
            get => _archievPath;
            set
            {
                this.RaiseAndSetIfChanged(ref _archievPath, value);
                Settings.ArchievFolder = value;
            }
        }

        public Settings Settings => Locator.Current.GetService<Settings>()!;
        
        public ReactiveCommand<Unit, Unit> BackCommand { get; } = null!;
        public ReactiveCommand<Unit, Unit> SaveCommand { get; } = null!;
        public ReactiveCommand<Unit, Unit> OpenTemplateFolderCommand { get; } = null!;
        public ReactiveCommand<Unit, Unit> OpenArchievFolderCommand { get; } = null!;

        public sealed override IScreen HostScreen { get; set; } = null!;
    }
}
