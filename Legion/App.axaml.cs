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
using Legion.Helpers;
using Legion.Models.Internal;
using Legion.Views;
using ReactiveUI;

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

        public override void OnFrameworkInitializationCompleted()
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

                Log.Logger.Information("Application Starting");

                IHost host = Host.CreateDefaultBuilder() // Initializing the Host 
                   .ConfigureServices((context, services) => { // Adding the DI container for configuration
                       //services.AddSingleton(Configuration);

                       services.AddSingleton<MainWindow>();
                       services.AddSingleton<LoginView>();
                       services.AddSingleton<InvestorsView>();
                       services.AddSingleton<InvestorsViewModel>();
                       services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));
                   })
                   .UseSerilog() // Add Serilog
                   .Build(); // Build the Host

                // Db init
                var _context = host.Services.GetRequiredService<ApplicationDbContext>();
                _context.Database.EnsureCreated();

                if (_context.Users.FirstOrDefault(user => user.UserName == "admin") == null)
                {
                    _context.UserRoles.Add(new UserRole() { Role = "Àäìèí"});
                    _context.UserRoles.Add(new UserRole() { Role = "Áîññ êà÷àëêè" });
                    _context.SaveChanges();
                    _context.Users.Add(new User() { Password = "123", UserName = "admin", EmployerFirstName = "Aboba", UserRole = _context.UserRoles.First(role => role.Role.Contains("Àäìèí")) });
                    _context.Users.Add(new User() { Password = "321", UserName = "loh", EmployerFirstName = "Biba", UserRole = _context.UserRoles.First(role => role.Role.Contains("Áîññ êà÷àëêè")) });
                    _context.ContractTypes.Add(new ContractType() { ContractIdFormat = "id/yy", Bet = 2, Formula = "x*p", Period = 6, TypeName = "Íàêîïèòåëüíûé", CanAddMoney = true });
                    _context.ContractTypes.Add(new ContractType() { ContractIdFormat = "id/yy", Bet = 4, Formula = "x*p", Period = 12, TypeName = "Ãîäîâîé", CanAddMoney = false });
                    _context.ContractTypes.Add(new ContractType() { ContractIdFormat = "id/yyyy/yy", Bet = 6, Formula = "x*p", Period = 36, TypeName = "Òðåõãîäîâîé", CanAddMoney = false });
                    _context.ContractTypes.Add(new ContractType() { ContractIdFormat = "id/yyyy/yy", Bet = 7, Formula = "x*p", Period = 18, TypeName = "Äîõîäíûé", CanAddMoney = false });
                    _context.ContractStatuses.Add(new ContractStatus() { Status = "Îòêðûò" });
                    _context.ContractStatuses.Add(new ContractStatus() { Status = "Çàêðûò" });
                    _context.ContractStatuses.Add(new ContractStatus() { Status = "Ïðèîñòàíîâëåí" });
                    _context.SaveChanges();
                }
                else
                {
                    Log.Logger.Information($"Finded user {_context.Users.FirstOrDefault()?.UserName} in database");
                }

                _context.SaveChanges();

                Locator.CurrentMutable.RegisterConstant<IScreen>(new MainWindowViewModel(_context));
                Locator.CurrentMutable.RegisterConstant(new MainWindow());
                Locator.CurrentMutable.RegisterConstant(desktop);
                Locator.CurrentMutable.RegisterConstant(SettingsHelper.LoadSettings() ?? new Settings());

                desktop.MainWindow = Locator.Current.GetService<MainWindow>();
                desktop.MainWindow!.DataContext = Locator.Current.GetService<IScreen>();
                ((MainWindowViewModel)desktop.MainWindow.DataContext!).GoNext.Execute();
            }

            base.OnFrameworkInitializationCompleted();
            Locator.CurrentMutable.RegisterLazySingleton(() => new AppViewLocator(), typeof(IViewLocator));
        }
    }
}