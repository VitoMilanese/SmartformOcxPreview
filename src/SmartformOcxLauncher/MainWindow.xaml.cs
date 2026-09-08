using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Printing;
using System.Reflection;
using System.Text;
using System.Windows;
using Microsoft.Win32;

namespace SmartformOcxLauncher;

public partial class MainWindow : Window
{
    private const string ProcessName = "SmartformOcxPreview";
    private const string ExecutableName = "SmartformOcxPreview.exe";
    private const string StartInfoFileName = "start.info";

    public MainWindow()
    {
        InitializeComponent();
        LoadPrinters();

        var root = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        PhotoTextBox.Text = Path.Combine(root, "Foto", "2C1B060229A914F2A90A8C7E3BE6EB40");
        SignatureTextBox.Text = Path.Combine(root, "Foto", "7AF0130FEF08718CF8CE259B2D82C7E0");
        TscEvdDatePicker.SelectedDate = DateTime.Today;

        var startInfoFileName = Path.Combine(root, StartInfoFileName);
        if (File.Exists(startInfoFileName))
        {
            try
            {
                var file = File.ReadLines(startInfoFileName).ToArray();

                ProfileFileNameTextBox.Text = file[5];
                SaleDeviceTextBox.Text = file[6];
                ProfileIdTextBox.Text = file[7];
                ProfileShortNameTextBox.Text = file[8];
                ProfileLongNameTextBox.Text = file[9];
                UserIdTextBox.Text = file[10];
                HolderNameTextBox.Text = file[11];
                HolderSurnameTextBox.Text = file[12];
                SerialNumberTextBox.Text = file[16];
                PhotoTextBox.Text = file[17];
                SignatureTextBox.Text = file[18];
                AddressTextBox.Text = file[19];
                ZipTextBox.Text = file[20];
                TownTextBox.Text = file[21];
                ProvinceTextBox.Text = file[22];
                Str1TextBox.Text = file[23];
                Str2TextBox.Text = file[24];
                Str3TextBox.Text = file[25];
                String1TextBox.Text = file[26];
                ProfileNoteTextBox.Text = file[27];
                RequestIdTextBox.Text = file[28];
                PrintingTypeTextBox.Text = file[29];
                PageSelectTextBox.Text = file[30];
                PrinterNameComboBox.Text = file[31];

                try
                {
                    BirthdayDatePicker.SelectedDate = DateTime.ParseExact(file[13], "dd-MM-yyyy", DateTimeFormatInfo.InvariantInfo);
                }
                catch { }
                try
                {
                    IssuingDateDatePicker.SelectedDate = DateTime.ParseExact(file[14], "dd-MM-yyyy", DateTimeFormatInfo.InvariantInfo);
                }
                catch { }
                try
                {
                    TscEvdDatePicker.SelectedDate = DateTime.ParseExact(file[15], "dd-MM-yyyy", DateTimeFormatInfo.InvariantInfo);
                }
                catch { }
            }
            catch { }
        }
    }

    private void LoadPrinters()
    {
        try
        {
            using var printServer = new LocalPrintServer();
            var queues = printServer.GetPrintQueues();

            foreach (var queue in queues.OrderBy(q => q.Name))
            {
                PrinterNameComboBox.Items.Add(queue.Name);
            }

            PrinterNameComboBox.Text = "XPS Card Printer";
        }
        catch (Exception ex)
        {
            StatusTextBlock.Text = $"Unable to load printers: {ex.Message}";
        }
    }

    private void BrowseProfileFile_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new ProfileFileDialog
        {
            Owner = this
        };

        if (dialog.ShowDialog() == true &&
            !string.IsNullOrWhiteSpace(dialog.SelectedFileName))
        {
            ProfileFileNameTextBox.Text = dialog.SelectedFileName;
        }
    }

    private void BrowsePhoto_Click(object sender, RoutedEventArgs e)
    {
        string? path = SelectImageFile();

        if (path != null)
        {
            PhotoTextBox.Text = Path.GetFileName(path);
        }
    }

    private void BrowseSignature_Click(object sender, RoutedEventArgs e)
    {
        string? path = SelectImageFile();

        if (path != null)
        {
            SignatureTextBox.Text = Path.GetFileName(path);
        }
    }

    private static string? SelectImageFile()
    {
        var dialog = new OpenFileDialog
        {
            Title = "Select image",
            Filter = "All files|*.*|Image files|*.jpg;*.jpeg;*.png;*.bmp;*.gif",
            CheckFileExists = true
        };

        return dialog.ShowDialog() == true
            ? dialog.FileName
            : null;
    }

    private void StartSmartform_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            string executablePath = Path.Combine(
                AppContext.BaseDirectory,
                ExecutableName);

            if (!File.Exists(executablePath))
            {
                MessageBox.Show(
                    $"{ExecutableName} was not found.\n\nExpected path:\n{executablePath}",
                    "Executable not found",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                StatusTextBlock.Text = $"Executable not found: {executablePath}";
                return;
            }

            var root = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
            var prtFile = Path.Combine(root, "SmartFormOcxPrint");

            if (File.Exists(prtFile))
            {
                File.Delete(prtFile);
            }

            var left = SystemParameters.PrimaryScreenWidth / 2 + 30;
            var top = SystemParameters.PrimaryScreenHeight / 2 - 150;
            var r = 201;
            var g = 99;
            var b = 18;

            var lyFile = ProfileFileNameTextBox.Text;
            if (!Path.HasExtension(lyFile))
            {
                lyFile += ".ly";
            }

            var startInfoFile = new[]
            {
                root,
                $"{left}",
                $"{top}",
                $"{r.ToString().PadLeft(3, '0')}{g.ToString().PadLeft(3, '0')}{b.ToString().PadLeft(3, '0')}",
                "false",
                lyFile,
                SaleDeviceTextBox.Text,
                ProfileIdTextBox.Text,
                ProfileShortNameTextBox.Text,
                ProfileLongNameTextBox.Text,
                UserIdTextBox.Text,
                HolderNameTextBox.Text,
                HolderSurnameTextBox.Text,
                BirthdayDatePicker.SelectedDate?.ToString("dd-MM-yyyy") ?? string.Empty,
                IssuingDateDatePicker.SelectedDate?.ToString("dd-MM-yyyy") ?? string.Empty,
                TscEvdDatePicker.SelectedDate ?.ToString("dd-MM-yyyy") ?? string.Empty,
                SerialNumberTextBox.Text,
                PhotoTextBox.Text,
                SignatureTextBox.Text,
                AddressTextBox.Text,
                ZipTextBox.Text,
                TownTextBox.Text,
                ProvinceTextBox.Text,
                Str1TextBox.Text,
                Str2TextBox.Text,
                Str3TextBox.Text,
                String1TextBox.Text,
                ProfileNoteTextBox.Text,
                RequestIdTextBox.Text,
                PrintingTypeTextBox.Text,
                PageSelectTextBox.Text,
                PrinterNameComboBox.Text
            };
            var startInfoFileName = Path.Combine(root, StartInfoFileName);
            File.WriteAllLines(startInfoFileName, startInfoFile);

            var processStartInfo = new ProcessStartInfo
            {
                WorkingDirectory = root,
                FileName = executablePath,
                Arguments = $"\"{startInfoFileName}\"",
                UseShellExecute = true,
                WindowStyle = ProcessWindowStyle.Normal
            };

            var process = Process.Start(processStartInfo);

            if (process == null)
            {
                throw new InvalidOperationException(
                    "The SmartformOcxPreview process could not be started.");
            }

            StatusTextBlock.Text =
                $"SmartformOcxPreview started. PID: {process.Id}.";
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                ex.Message,
                "Start error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            StatusTextBlock.Text = $"Start failed: {ex.Message}";
        }
    }

    private void KillSmartform_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Process[] processes = Process.GetProcessesByName(ProcessName);

            if (processes.Length == 0)
            {
                StatusTextBlock.Text =
                    "No SmartformOcxPreview processes are running.";
                return;
            }

            int killedCount = 0;
            var errors = new List<string>();

            foreach (Process process in processes)
            {
                using (process)
                {
                    try
                    {
                        //process.Kill(entireProcessTree: true);
                        process.Kill();
                        process.WaitForExit(5000);
                        killedCount++;
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"PID {process.Id}: {ex.Message}");
                    }
                }
            }

            StatusTextBlock.Text = errors.Count == 0
                ? $"Killed {killedCount} SmartformOcxPreview process(es)."
                : $"Killed {killedCount} process(es). Errors: {string.Join(" | ", errors)}";
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                ex.Message,
                "Kill error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            StatusTextBlock.Text = $"Kill failed: {ex.Message}";
        }
    }

    private static string FormatDate(DateTime? value)
    {
        return value?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
            ?? string.Empty;
    }

    private static void AppendArgument(
        StringBuilder builder,
        string name,
        string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return;
        }

        if (builder.Length > 0)
        {
            builder.Append(' ');
        }

        builder.Append("--");
        builder.Append(name);
        builder.Append(' ');
        builder.Append('"');
        builder.Append(EscapeArgument(value));
        builder.Append('"');
    }

    private static string EscapeArgument(string value)
    {
        return value.Replace("\\", "\\\\")
                    .Replace("\"", "\\\"")
                    .Replace("\r", " ")
                    .Replace("\n", " ");
    }

    private void Print_Click(object sender, RoutedEventArgs e)
    {
        var root = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        var prtFile = Path.Combine(root, "SmartFormOcxPrint");
        File.WriteAllText(prtFile, string.Empty);

        Task.Delay(TimeSpan.FromSeconds(3)).Wait();

        KillSmartform_Click(sender, e);
    }
}
