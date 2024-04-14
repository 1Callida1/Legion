using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Legion.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Splat;
using System;
using System.IO;
using Serilog.Events;
using Microsoft.EntityFrameworkCore;
using Legion.Models;
using SkiaSharp;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Legion.Helpers;
using Legion.Models.Internal;
using Legion.Views;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using ReactiveUI;
using ILogger = Serilog.ILogger;

namespace Legion
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public IServiceProvider? ServiceProvider { get; private set; }

        public IConfiguration? Configuration { get; private set; }

        public override async void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                IConfigurationBuilder builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

                Configuration = builder.Build();

                // Specifying the configuration for serilog
                Log.Logger = new LoggerConfiguration() // initiate the logger configuration
                                                       //.ReadFrom.Configuration(Configuration) // connect serilog to our configuration folder
                    .Enrich.FromLogContext() //Adds more information to our logs from built in Serilog 
                    .WriteTo.Debug()
                    .WriteTo.File("Logs/debug.txt", rollingInterval: RollingInterval.Day,
                        outputTemplate:
                        "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}")
                    .CreateLogger(); //initialise the logger

                ILogger _log = Log.Logger.ForContext<App>();

                _log.Information("Application Starting");

                IHost host = Host.CreateDefaultBuilder() // Initializing the Host 
                   .ConfigureServices((context, services) => { // Adding the DI container for configuration
                       //services.AddSingleton(Configuration);

                       services.AddSingleton<MainWindow>();
                       services.AddSingleton<LoginView>();
                       services.AddSingleton<InvestorsView>();
                       services.AddSingleton<InvestorsViewModel>();
                       services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"), npgsqlDbContextOptionsBuilder => npgsqlDbContextOptionsBuilder.EnableRetryOnFailure()));
                   })
                   .UseSerilog(Log.Logger) // Add Serilog
                   .Build(); // Build the Host

                // Db init
                var context = host.Services.GetRequiredService<ApplicationDbContext>();

                Locator.CurrentMutable.RegisterConstant(new MainWindow());
                Locator.CurrentMutable.RegisterConstant<IScreen>(new MainWindowViewModel(context));
                Locator.CurrentMutable.RegisterConstant(desktop);
                Locator.CurrentMutable.RegisterConstant(SettingsHelper.LoadSettings() ?? new Settings());

                desktop.MainWindow = Locator.Current.GetService<MainWindow>();
                desktop.MainWindow!.DataContext = Locator.Current.GetService<IScreen>();
                ((MainWindowViewModel)desktop.MainWindow.DataContext!).GoNext.Execute();

                desktop.MainWindow.Show();

                if (context.Database.CanConnect())
                {
                    _log.Information("Database can connect!");
                    context.Database.EnsureCreated();

                    if (!context.Users.Any())
                    {
                        await context.UserRoles.AddAsync(new UserRole() { Role = "Admin", CanAddContracts = true, CanDeleteData = true, CanManageUsers = true, CanSeeHiddenData = true, CanViewReports = true });
                        await context.SaveChangesAsync();
                        await context.Users.AddAsync(new User() { Password = "123", UserName = "admin", EmployerFirstName = "Admin", UserRole = context.UserRoles.First(role => role.Role.Contains(Role.Admin.ToString())) });

                        await context.ContractTypes.AddAsync(new ContractType() { ContractIdFormat = "id-E/yy", Bet = 2, Formula = "x*p", Period = 6, TypeName = "Накопительный", CanAddMoney = true });
                        await context.ContractTypes.AddAsync(new ContractType() { ContractIdFormat = "id/yy", Bet = 4, Formula = "x*p", Period = 12, TypeName = "Инвестиционный", CanAddMoney = false });
                        await context.ContractTypes.AddAsync(new ContractType() { ContractIdFormat = "id-36/yy", Bet = 6, Formula = "x*p", Period = 36, TypeName = "Трехгодовой", CanAddMoney = false });
                        await context.ContractTypes.AddAsync(new ContractType() { ContractIdFormat = "id-18/yy", Bet = 7, Formula = "x*p", Period = 18, TypeName = "Полуторагодовой", CanAddMoney = false });
                        await context.ContractTypes.AddAsync(new ContractType() { ContractIdFormat = "id/yy", Bet = 3, Formula = "x*p", Period = 12, TypeName = "ТАНАКА инвестиционный", CanAddMoney = false });
                        await context.ContractTypes.AddAsync(new ContractType() { ContractIdFormat = "idK/yy", Bet = 3, Formula = "x*p", Period = 24, TypeName = "ТАНАКА накопительный", CanAddMoney = false });
                        await context.ContractStatuses.AddAsync(new ContractStatus() { Status = "Открыт" });
                        await context.ContractStatuses.AddAsync(new ContractStatus() { Status = "Закрыт" });
                        await context.ContractStatuses.AddAsync(new ContractStatus() { Status = "Приостановлен" });
                        await context.SaveChangesAsync();
                    }
                    else
                    {
                        _log.Information($"Found user {context.Users.FirstOrDefault()?.UserName} in database");
                    }

                    await context.SaveChangesAsync();
                }
                else
                {
                    _log.Fatal("Database connection error!");

                    var box = MessageBoxManager
                        .GetMessageBoxStandard("Ошибка!", "Невозможно установить соединение с базой данных! Проверьте подключение к интернету.",
                            ButtonEnum.Ok, Icon.Error);

                    await box.ShowAsPopupAsync(desktop.MainWindow);

                    desktop.Shutdown(1);
                }
            }

            base.OnFrameworkInitializationCompleted();
            Locator.CurrentMutable.RegisterLazySingleton(() => new AppViewLocator(), typeof(IViewLocator));
        }
    }
}