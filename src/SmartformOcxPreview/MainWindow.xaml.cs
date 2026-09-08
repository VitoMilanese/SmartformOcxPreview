using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Forms.Integration;
using System.Windows.Media;
using AFCS.TOM.APP.BgServices;
using AxSmartFormNETDLL;
using NLog;

namespace SmartformOcxPreview
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string[] StartupArgs { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            CommBgService.Hide = OnHide;
            CommBgService.Print = OnPrint;
        }

        private static string IniFilePath { get; } = @"SmartFormLayoutCampo.ini";
        private static string LyFilesPath { get; } = @"LY";
        private static string LySfondoDirectoryFoto { get; } = @"Foto";
        private static short Zoom { get; set; } = 100;

        private WindowsFormsHost? _host { get; set; }

        private AxSFLayoutPrint? _axSF { get; set; }
        private AxSFLayoutPrint? AxSF
        {
            get => _axSF;
            set
            {
                _axSF = value;
                if (_host != null) _host.Child = _axSF;
            }
        }
        
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            return;
            const short ZoomMin = 100;
            const short ZoomMax = 300;
            const int WSteps = 8;
            const int HSteps = 8;
            const int WMin = 750;
            const int HMin = 400;

            var WStep = WMin / WSteps;
            var HStep = HMin / HSteps;
            var w = e.NewSize.Width;
            var h = e.NewSize.Height;

            var zoomW = (short)(ZoomMin / WSteps * Math.Max(WSteps, (int)(w / WStep)));
            var zoomH = (short)(ZoomMin / HSteps * Math.Max(HSteps, (int)(h / HStep)));
            var zoom = Math.Max(ZoomMin, Math.Min(ZoomMax, Math.Min(zoomW, zoomH)));

            Zoom = zoom;
            if (AxSF != null)
            {
                LoadLayout();
            }
        }

        public void Dispose()
        {
            LogHelper.LogInfo("Disposing");
            try
            {
                _axSF?.ResizeToZeroPb();
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex);
            }
            try
            {
                _axSF?.Dispose();
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex);
            }
            try
            {
                _host?.Dispose();
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex);
            }
            _axSF = null;
            _host = null;
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LogHelper.LogInfo("UserControl loaded");
            Loaded -= UserControl_Loaded;

            try
            {
                var rgb = (StartupArgs?.Length ?? 0) > 3 ? StartupArgs![3] ?? string.Empty : string.Empty;
                if (!string.IsNullOrWhiteSpace(rgb))
                {
                    var rr = rgb.Substring(0, 3);
                    var gg = rgb.Substring(3, 3);
                    var bb = rgb.Substring(6, 3);
                    byte.TryParse(rr, out var r);
                    byte.TryParse(gg, out var g);
                    byte.TryParse(bb, out var b);
                    var color = System.Windows.Media.Color.FromRgb(r, g, b);
                    Background = new SolidColorBrush(color);
                }

                if ((StartupArgs?.Length ?? 0) > 1 && double.TryParse(StartupArgs![1], out var left))
                {
                    Left = left - (Width / 2) - 30;
                }
                else
                {
                    Left = (SystemParameters.PrimaryScreenWidth - Width) / 2 - 30;
                }
                LogHelper.LogInfo($"Left: {Left}");

                if ((StartupArgs?.Length ?? 0) > 2 && double.TryParse(StartupArgs![2], out var top))
                {
                    Top = top - (Height / 2);
                }
                else
                {
                    Top = (SystemParameters.PrimaryScreenHeight - Height) / 2 - 100;
                }
                LogHelper.LogInfo($"Top: {Top}");

                LogHelper.LogInfo("Creating Host");
                _host = new WindowsFormsHost
                {
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Background = System.Windows.Media.Brushes.LightGray,
                    Child = AxSF
                };
                HostRoot.Child = _host;
                HostRoot.Width = 0;
                HostRoot.Height = 0;

                LoadLayout();
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex);
            }
        }

        private void LoadLayout()
        {
            LogHelper.LogInfo("Loading layout");
            try
            {
                var root = (StartupArgs?.Length ?? 0) > 0 ? StartupArgs![0] ?? string.Empty : string.Empty;
                var layoutName = (StartupArgs?.Length ?? 0) > 5 ? StartupArgs![5] ?? string.Empty : string.Empty;

                var lyFileName = Path.Combine(root, LyFilesPath, layoutName);
                if (!File.Exists(lyFileName))
                {
                    LogHelper.LogError($"Layout file not found: {lyFileName}");
                    throw new FileNotFoundException(lyFileName);
                }

                AxSF?.Dispose();
                AxSF = new AxSFLayoutPrint();
                AxSF.Init(Path.Combine(root, IniFilePath));

                HostRoot.Width = int.MaxValue;
                HostRoot.Height = int.MaxValue;

                AxSF.SetLayout(lyFileName);

                SetFields();

                AxSF.PreviewLayout(0, (short)(Zoom * 1.5));

                HostRoot.BorderThickness = new Thickness(1, 1, 1, 1);
                HostRoot.Width = Math.Max(AxSF.Width + HostRoot.BorderThickness.Left, 420);
                HostRoot.Height = Math.Max(AxSF.Height + HostRoot.BorderThickness.Top, 300);
                LogHelper.LogInfo("Layout loaded");
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex);
            }
        }

        private void SetField(string key, string value)
        {
            LayoutFields.SetValue(key, value);
            AxSF?.SetField($"%{key}%", value);
        }

        private void SetFields()
        {
            LogHelper.LogInfo("Setting fields");
            var root               = (StartupArgs?.Length ?? 0) > 0  ? StartupArgs![0]  ?? string.Empty : string.Empty;
            var SALE_DEVICE        = (StartupArgs?.Length ?? 0) > 6  ? StartupArgs![6]  ?? string.Empty : string.Empty;
            var PROFILE_IDENTIFIER = (StartupArgs?.Length ?? 0) > 7  ? StartupArgs![7]  ?? string.Empty : string.Empty;
            var PROFILE_SHORT_NAME = (StartupArgs?.Length ?? 0) > 8  ? StartupArgs![8]  ?? string.Empty : string.Empty;
            var PROFILE_LONG_NAME  = (StartupArgs?.Length ?? 0) > 9  ? StartupArgs![9]  ?? string.Empty : string.Empty;
            var HOLDER_USER_ID     = (StartupArgs?.Length ?? 0) > 10 ? StartupArgs![10] ?? string.Empty : string.Empty;
            var HOLDER_FIRST_NAME  = (StartupArgs?.Length ?? 0) > 11 ? StartupArgs![11] ?? string.Empty : string.Empty;
            var HOLDER_LAST_NAME   = (StartupArgs?.Length ?? 0) > 12 ? StartupArgs![12] ?? string.Empty : string.Empty;
            var HOLDER_BIRTH_DAY   = (StartupArgs?.Length ?? 0) > 13 ? StartupArgs![13] ?? string.Empty : string.Empty;
            var HOLDER_BIRTH_DATE  = (StartupArgs?.Length ?? 0) > 13 ? StartupArgs![13] ?? string.Empty : string.Empty;
            var ISSUING_DATE       = (StartupArgs?.Length ?? 0) > 14 ? StartupArgs![14] ?? string.Empty : string.Empty;
            var TSC_VAL_END_DATE   = (StartupArgs?.Length ?? 0) > 15 ? StartupArgs![15] ?? string.Empty : string.Empty;
            var SERIAL_NUMBER      = (StartupArgs?.Length ?? 0) > 16 ? StartupArgs![16] ?? string.Empty : string.Empty;
            var HOLDER_PHOTO       = (StartupArgs?.Length ?? 0) > 17 ? StartupArgs![17] ?? string.Empty : string.Empty;
            var HOLDER_SIGNATURE   = (StartupArgs?.Length ?? 0) > 18 ? StartupArgs![18] ?? string.Empty : string.Empty;
            var HOLDER_ADDRESS     = (StartupArgs?.Length ?? 0) > 19 ? StartupArgs![19] ?? string.Empty : string.Empty;
            var HOLDER_ZIP_CODE    = (StartupArgs?.Length ?? 0) > 20 ? StartupArgs![20] ?? string.Empty : string.Empty;
            var HOLDER_TOWN        = (StartupArgs?.Length ?? 0) > 21 ? StartupArgs![21] ?? string.Empty : string.Empty;
            var HOLDER_PROV        = (StartupArgs?.Length ?? 0) > 22 ? StartupArgs![22] ?? string.Empty : string.Empty;
            var STR_1              = (StartupArgs?.Length ?? 0) > 23 ? StartupArgs![23] ?? string.Empty : string.Empty;
            var STR_2              = (StartupArgs?.Length ?? 0) > 24 ? StartupArgs![24] ?? string.Empty : string.Empty;
            var STR_3              = (StartupArgs?.Length ?? 0) > 25 ? StartupArgs![25] ?? string.Empty : string.Empty;
            var STRING_1           = (StartupArgs?.Length ?? 0) > 26 ? StartupArgs![26] ?? string.Empty : string.Empty;
            var PROFILE_NOTE       = (StartupArgs?.Length ?? 0) > 27 ? StartupArgs![27] ?? string.Empty : string.Empty;
            var RICHIESTA_ID       = (StartupArgs?.Length ?? 0) > 28 ? StartupArgs![28] ?? string.Empty : string.Empty;
            SetField("SALE_DEVICE", SALE_DEVICE);
            SetField("PROFILE_IDENTIFIER", PROFILE_IDENTIFIER);
            SetField("PROFILE_SHORT_NAME", PROFILE_SHORT_NAME);
            SetField("PROFILE_LONG_NAME", PROFILE_LONG_NAME);
            SetField("HOLDER_USER_ID", HOLDER_USER_ID);
            SetField("HOLDER_FIRST_NAME", HOLDER_FIRST_NAME);
            SetField("HOLDER_LAST_NAME", HOLDER_LAST_NAME);
            SetField("HOLDER_BIRTH_DAY", HOLDER_BIRTH_DAY);
            SetField("HOLDER_BIRTH_DATE", HOLDER_BIRTH_DATE);
            SetField("ISSUING_DATE", ISSUING_DATE);
            SetField("TSC_VAL_END_DATE", TSC_VAL_END_DATE);
            SetField("SERIAL_NUMBER", SERIAL_NUMBER);
            SetField("HOLDER_ADDRESS", HOLDER_ADDRESS);
            SetField("HOLDER_ZIP_CODE", HOLDER_ZIP_CODE);
            SetField("HOLDER_TOWN", HOLDER_TOWN);
            SetField("HOLDER_PROV", HOLDER_PROV);
            SetField("STR_1", STR_1);
            SetField("STR_2", STR_2);
            SetField("STR_3", STR_3);
            SetField("STRING_1", STRING_1);
            SetField("PROFILE_NOTE", PROFILE_NOTE);
            SetField("RICHIESTA_ID", RICHIESTA_ID);
            if (string.IsNullOrWhiteSpace(HOLDER_PHOTO))
            {
                var file = Directory.GetFiles(Path.Combine(root, LySfondoDirectoryFoto), "fake.*", SearchOption.TopDirectoryOnly)?.FirstOrDefault();
                if (file != null)
                {
                    var path = Path.Combine(root, LySfondoDirectoryFoto, Path.GetFileName(file));
                    LogHelper.LogInfo(path);
                    SetField("HOLDER_PHOTO", path);
                }
            }
            else
            {
                LogHelper.LogInfo(HOLDER_PHOTO);
                SetField("HOLDER_PHOTO", HOLDER_PHOTO);
            }
            if (!string.IsNullOrWhiteSpace(HOLDER_SIGNATURE))
            {
                LogHelper.LogInfo(HOLDER_SIGNATURE);
                SetField("HOLDER_SIGNATURE", HOLDER_SIGNATURE);
            }
        }

        public static string CapitalizeWords(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return text;
            var previous = text[0];
            for (var i = 1; i < text.Length; ++i)
            {
                if (previous == ' ' && text[i] != ' ')
                {
                    var letter = $"{text[i]}".ToUpper();
                    text = text.Remove(i, 1).Insert(i, letter);
                    previous = letter[0];
                    continue;
                }
                if (previous == ' ') continue;
                previous = text[i];
            }
            return text;
        }

        private void OnHide(object sender, EventArgs e)
        {
            try
            {
                Dispatcher?.Invoke(Hide);
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex);
            }
        }

        private void OnPrint(object sender, EventArgs e)
        {
            if (File.Exists("SmartFormOcxPrintDelay"))
            {
                Task.Delay(TimeSpan.FromSeconds(15)).Wait();
            }
            try
            {
                short.TryParse((StartupArgs.Length > 29 ? StartupArgs[29] : "1"), out var printingType);
                short.TryParse((StartupArgs.Length > 30? StartupArgs[30] : "0"), out var pageSelect);

                AxSF.SetPrinter(StartupArgs![31]);
                LogHelper.LogDebug("Printing layout");
                var printOk = AxSF.PrintLayout(printingType, pageSelect, 100);
                LogHelper.LogDebug($"Print result: {printOk}");
                var fileName = "SmartFormOcxPrintResult";
                File.WriteAllText(fileName, printOk.ToString());
                if (File.Exists("SmartFormOcxPrintLogExit"))
                {
                    if (File.Exists(fileName))
                    {
                        LogHelper.LogDebug($"Print result saved to file");
                    }
                    else
                    {
                        LogHelper.LogDebug($"Print result NOT saved to file");
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex);
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            var cancel = false;
            if ((StartupArgs?.Length ?? 0) > 4 && bool.TryParse(StartupArgs![4], out cancel))
            e.Cancel = cancel;
        }
    }
}
