using GenericHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HostingExample
{
    public class Program
    {
        private static async Task Main()
        {
            var logger = LogManager.GetCurrentClassLogger();
            try
            {
                var hostBuilder = new HostBuilder()
                    .UseNLog()
                    .ConfigureAppConfiguration((hostingContext, config) =>
                    {
                        config.AddJsonFile("appsettings.json", optional: true);
                        config.AddEnvironmentVariables();
                    })
                    .ConfigureServices((hostContext, services) =>
                    {
                        services.Configure<AppConfig>(hostContext.Configuration.GetSection("AppConfig"));
                        services.AddSingleton<IMqttService, MqttService>();
                        services.AddSingleton<ITcpService, TcpService>();
                        services.AddHostedService<ConsoleHostedService>();
                
                    });

                // Build and run the host in one go; .RCA is specialized for running it in a console.
                // It registers SIGTERM(Ctrl-C) to the CancellationTokenSource that's shared with all services in the container.
                hostBuilder.Start();

                Console.WriteLine("The host container has terminated. Press ANY key to exit the console.");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                // NLog: catch setup errors (exceptions thrown inside of any containers may not necessarily be caught)
                logger.Fatal(ex, "Stopped program because of exception");
                throw;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                LogManager.Shutdown();
            }
        }


    }
}