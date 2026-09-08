using System.Diagnostics;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Hosting;
using SmartformOcxPreview;

namespace AFCS.TOM.APP.BgServices
{
    internal class CommBgService : BackgroundService
    {
        public static EventHandler Hide { get; set; }
        public static EventHandler Print { get; set; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                LogHelper.LogInfo($"CommBgService started");
                while (!stoppingToken.IsCancellationRequested)
                {
                    DeleateStartInfoFile();
                    CheckAndKill();
                    CheckHideCommand();
                    CheckPrintCommand();
                    await Task.Delay(100, stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex);
            }
        }

        private void DeleateStartInfoFile()
        {
            if (MainWindow.StartupArgs?.Any() ?? false)
            {
                if (File.Exists(MainWindow.StartupArgs[0]))
                {
                    try
                    {
                        File.Delete(MainWindow.StartupArgs[0]);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.LogException(ex);
                    }
                }
            }
        }

        private void CheckAndKill()
        {
#if DEBUG
            return;
#endif
            var allowedViews = "smartform_allowed_views.txt";
            var currentViewFileName = "current_view.txt";
            if (!File.Exists(allowedViews) || !File.Exists(currentViewFileName)) return;
            try
            {
                var content = File.ReadAllText(currentViewFileName).Trim();
                var views = File.ReadAllLines(allowedViews);
                if (!views.Any(p => p.Equals(content, StringComparison.InvariantCultureIgnoreCase)))
                {
                    LogHelper.LogInfo($"SmartformOcxPreview closed on {content}");
                    Process.GetCurrentProcess().Kill();
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex);
            }
        }

        private void CheckHideCommand()
        {
            var fileName = "SmartFormOcxHide";
            if (!File.Exists(fileName)) return;
            
            try
            {
                File.Delete(fileName);
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex);
            }
            
            Hide?.Invoke(this, EventArgs.Empty);
        }

        private void CheckPrintCommand()
        {
            var root = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
            var prtFile = Path.Combine(root, "SmartFormOcxPrint");

            if (!File.Exists(prtFile)) return;
            
            try
            {
                File.Delete(prtFile);
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex);
            }

            var resFile = Path.Combine(root, "SmartFormOcxPrintResult");
            if (File.Exists(resFile))
            {
                try
                {
                    File.Delete(resFile);
                }
                catch (Exception ex)
                {
                    LogHelper.LogException(ex);
                }
            }

            Print?.Invoke(this, EventArgs.Empty);
        }
    }
}
