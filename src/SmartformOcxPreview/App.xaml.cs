using System.IO;
using System.Reflection;
using System.Windows;
using AFCS.TOM.APP.BgServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Logging.EventLog;

namespace SmartformOcxPreview
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            //if (!(e.Args?.Any() ?? false))
            //{
            //    var msg = "Started without parameters";
            //    Console.WriteLine(msg);
            //    LogHelper.LogError(msg);
            //    Shutdown(-1);
            //    return;
            //}

            //if (!File.Exists(e.Args![0]))
            //{
            //    var msg = "Initialization file not found";
            //    Console.WriteLine(msg);
            //    LogHelper.LogError(msg);
            //    Shutdown(-2);
            //    return;
            //}

            var ocxFileName = "smartformnet.ocx";
            var paths = ComHelper.FindRegisteredOcx(ocxFileName);
            var registered = paths.Count > 0;

            if (!registered)
            {
                var root = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;

                var ocxPath = Path.Combine(root, ocxFileName);

                if (File.Exists(ocxPath))
                {
                    try
                    {
                        registered = OcxRegistration.EnsureRegistered(ocxPath);
                        Console.WriteLine(registered ? "OCX is registered." : "OCX registration failed.");

                        if (registered)
                        {
                            paths = ComHelper.FindRegisteredOcx("smartformnet.ocx");
                            registered = paths.Count > 0;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }

            if (registered)
            {
                foreach (var path in paths)
                {
                    Console.WriteLine(path);
                    LogHelper.LogError(path);
                }
            }
            else
            {
                var msg = "OCX is not registered.";
                Console.WriteLine(msg);
                LogHelper.LogError(msg);
                Shutdown(-3);
                return;
            }

            //var lines = File.ReadAllLines(e.Args![0]);
            var lines = File.ReadAllLines("C:\\Src\\Local\\SmartformOcxPreview\\SmartformOcxLauncher\\bin\\Debug\\net6.0-windows\\start.info");
            SmartformOcxPreview.MainWindow.StartupArgs = lines;
            LogHelper.LogInfo($"Started with {lines.Length} parameter(s)");

            //var lines = File.ReadAllLines("start.info");
            //SmartformOcxPreview.MainWindow.StartupArgs = lines;

            IHostBuilder builder = Host.CreateDefaultBuilder(e.Args)
                .UseWindowsService(options =>
                {
                    options.ServiceName = "SmartformOcxPreviewService";
                })
                .ConfigureServices((context, services) =>
                {
                    LoggerProviderOptions.RegisterProviderOptions<EventLogSettings, EventLogLoggerProvider>(services);
                    services.AddHostedService<CommBgService>();
                });

            var host = builder.Build();
            _ = Task.Run(host.Run);
        }
    }
}
