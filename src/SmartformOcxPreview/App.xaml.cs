using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using AFCS.TOM.APP.BgServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Logging.EventLog;
using NLog;

namespace SmartformOcxPreview
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        public App()
        {
            DispatcherUnhandledException += Application_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            try
            {
                LogHelper.LogInfo("Application startup started.");
                LogHelper.LogInfo($"Base directory: {AppContext.BaseDirectory}");
                LogHelper.LogInfo($"Working directory: {Environment.CurrentDirectory}");
                LogHelper.LogInfo($"Process ID: {Environment.ProcessId}; architecture: {(Environment.Is64BitProcess ? "x64" : "x86")}");
                LogHelper.LogInfo($"Startup argument count: {e.Args.Length}");

                if (e.Args.Length == 0 || string.IsNullOrWhiteSpace(e.Args[0]))
                {
                    FailStartup("Started without the start.info parameter.", -1);
                    return;
                }

                var startInfoFileName = Path.GetFullPath(e.Args[0]);
                LogHelper.LogInfo($"Initialization file: {startInfoFileName}");

                if (!File.Exists(startInfoFileName))
                {
                    FailStartup($"Initialization file not found: {startInfoFileName}", -2);
                    return;
                }

                var ocxFileName = "smartformnet.ocx";
                LogHelper.LogInfo($"Checking COM registration for {ocxFileName}.");

                var paths = ComHelper.FindRegisteredOcx(ocxFileName);
                var registered = paths.Count > 0;

                if (!registered)
                {
                    var root = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
                    var ocxPath = Path.Combine(root, ocxFileName);
                    LogHelper.LogInfo($"OCX is not registered. Local OCX path: {ocxPath}");

                    if (File.Exists(ocxPath))
                    {
                        try
                        {
                            LogHelper.LogInfo("Attempting OCX registration.");
                            registered = OcxRegistration.EnsureRegistered(ocxPath);
                            LogHelper.LogInfo(registered
                                ? "OCX registration call succeeded."
                                : "OCX registration call returned false.");

                            if (registered)
                            {
                                paths = ComHelper.FindRegisteredOcx(ocxFileName);
                                registered = paths.Count > 0;
                                LogHelper.LogInfo($"OCX registration verification result: {registered}.");
                            }
                        }
                        catch (Exception ex)
                        {
                            LogHelper.LogException(ex, "OCX registration threw an exception.");
                        }
                    }
                    else
                    {
                        LogHelper.LogError($"Local OCX file not found: {ocxPath}");
                    }
                }

                if (registered)
                {
                    foreach (var path in paths)
                    {
                        LogHelper.LogInfo($"Registered OCX path: {path}");
                    }
                }
                else
                {
                    FailStartup("OCX is not registered.", -3);
                    return;
                }

                string[] lines;
                try
                {
                    LogHelper.LogInfo("Reading initialization file.");
                    lines = File.ReadAllLines(startInfoFileName);
                }
                catch (Exception ex)
                {
                    LogHelper.LogException(ex, $"Unable to read initialization file: {startInfoFileName}");
                    FailStartup("Unable to read the initialization file.", -4);
                    return;
                }

                if (lines.Length < 6)
                {
                    FailStartup($"Initialization file contains only {lines.Length} line(s); at least 6 are required.", -5);
                    return;
                }

                SmartformOcxPreview.MainWindow.StartupArgs = lines;
                LogHelper.LogInfo($"Initialization file loaded. Line count: {lines.Length}.");
                LogHelper.LogInfo($"Application root from start.info: {lines[0]}");
                LogHelper.LogInfo($"Layout from start.info: {lines[5]}");

                IHostBuilder builder = Host.CreateDefaultBuilder()
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
                _ = Task.Run(async () =>
                {
                    try
                    {
                        LogHelper.LogInfo("Background host starting.");
                        await host.RunAsync();
                        LogHelper.LogInfo("Background host stopped.");
                    }
                    catch (Exception ex)
                    {
                        LogHelper.LogException(ex, "Background host terminated unexpectedly.");
                    }
                });

                LogHelper.LogInfo("Application startup completed.");
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex, "Unhandled exception during application startup.");
                FailStartup("Unhandled exception during application startup.", -10);
            }
        }

        private static void FailStartup(string message, int exitCode)
        {
            LogHelper.LogError($"Fatal startup error ({exitCode}): {message}");
            LogManager.Flush(TimeSpan.FromSeconds(2));
            Current.Shutdown(exitCode);
        }

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            LogHelper.LogException(e.Exception, "Unhandled WPF dispatcher exception.");
            LogManager.Flush(TimeSpan.FromSeconds(2));
        }

        private static void CurrentDomain_UnhandledException(object? sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
            {
                LogHelper.LogException(ex, $"Unhandled AppDomain exception. IsTerminating={e.IsTerminating}.");
            }
            else
            {
                LogHelper.LogError($"Unhandled AppDomain exception object: {e.ExceptionObject}. IsTerminating={e.IsTerminating}.");
            }

            LogManager.Flush(TimeSpan.FromSeconds(2));
        }

        private static void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            LogHelper.LogException(e.Exception, "Unobserved task exception.");
            e.SetObserved();
            LogManager.Flush(TimeSpan.FromSeconds(2));
        }

        protected override void OnExit(ExitEventArgs e)
        {
            LogHelper.LogInfo($"Application exiting with code {e.ApplicationExitCode}.");
            LogManager.Flush(TimeSpan.FromSeconds(2));
            LogManager.Shutdown();
            base.OnExit(e);
        }
    }
}
