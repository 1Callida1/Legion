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

namespace Legion
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public IServiceProvider ServiceProvider { get; private set; }

        public IConfiguration Configuration { get; private set; }

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
                    .ReadFrom.Configuration(Configuration) // connect serilog to our configuration folder
                    .Enrich.FromLogContext() //Adds more information to our logs from built in Serilog 
                    .WriteTo.Debug()
                    .WriteTo.File("Logs/debug.txt", rollingInterval: RollingInterval.Day,
                        outputTemplate:
                        "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}")
                    .CreateLogger(); //initialise the logger

                Log.Logger.Information("Application Starting");

                IHost host = Host.CreateDefaultBuilder() // Initializing the Host 
                   .ConfigureServices((context, services) => { // Adding the DI container for configuration
                       services.AddSingleton(Configuration);

                       services.AddSingleton<LoginWindow>();
                       services.AddSingleton<LoginWindowViewModel>();
                       services.AddSingleton<InvestorsView>();
                       services.AddSingleton<InvestorsViewModel>();

                       services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite("name=ConnectionStrings:DefaultConnection"));
                   })
                   .UseSerilog() // Add Serilog
                   .Build(); // Build the Host

                host.Services.GetRequiredService<ApplicationDbContext>().Database.EnsureCreated();
                host.Services.GetRequiredService<ApplicationDbContext>().SaveChanges();

                desktop.MainWindow = host.Services.GetRequiredService<LoginWindow>();
                desktop.MainWindow.DataContext = host.Services.GetRequiredService<LoginWindowViewModel>();


                //desktop.MainWindow = new InvestorsView()
                //{
                //    DataContext = host.Services.GetRequiredService<InvestorsViewModel>()
                //};
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}