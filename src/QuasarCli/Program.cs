using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Quasar.Core;
using Quasar.ElasticSearch;
using System;
using System.IO;

namespace QuasarCli
{
    class Program
    {
        static void Main(string[] args)
        {
            // create service collection
            var serviceCollection = new ServiceCollection();
            var serviceProvider = ConfigureServices(serviceCollection);

            // entry to run app
            serviceProvider.GetService<App>().Run();

            Console.ReadKey();
        }

        private static ServiceProvider ConfigureServices(IServiceCollection services)
        {
            // Setup custom classes
            services.AddTransient<App>();
            services.AddTransient<PackageManager>();
            services.AddTransient<DeploymentRunner>();
            services.AddTransient<PowershellRunner>();
            services.AddTransient<ProcessRunner>();
            services.AddTransient<IDeploymentPublisher, ElasticSearchDeploymentPublisher>();

            // Configure Logging
            services.AddSingleton<ILoggerFactory, LoggerFactory>();
            services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
            services.AddLogging((builder) => builder.SetMinimumLevel(LogLevel.Trace));

            var serviceProvider = services.BuildServiceProvider();

            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

            // Configure NLog
            loggerFactory.AddNLog(new NLogProviderOptions { CaptureMessageTemplates = true, CaptureMessageProperties = true });
            NLog.LogManager.LoadConfiguration(string.Format("..{0}..{0}conf{0}nlog.config",Path.DirectorySeparatorChar));

            return serviceProvider;
        }
    }
}
