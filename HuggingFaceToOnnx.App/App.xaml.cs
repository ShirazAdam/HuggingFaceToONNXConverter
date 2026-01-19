using Microsoft.Extensions.DependencyInjection;
using HuggingFaceToOnnx.App.Services;
using HuggingFaceToOnnx.App.ViewModels;
using System.Windows;

namespace HuggingFaceToOnnx.App
{
    public partial class App
    {
        public IServiceProvider Services { get; }

        public App()
        {
            Services = ConfigureServices();
        }

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // Services
                        services.AddSingleton<IConverterService, ConverterService>();
            services.AddSingleton<IFileService, FileService>();

            // ViewModels
            services.AddTransient<MainViewModel>();

            // Views
            services.AddTransient<MainWindow>();

            return services.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var mainWindow = Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
    }
}

