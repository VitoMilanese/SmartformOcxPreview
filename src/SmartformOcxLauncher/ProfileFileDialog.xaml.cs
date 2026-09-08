using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace SmartformOcxLauncher;

public partial class ProfileFileDialog : Window
{
    public string? SelectedFileName { get; private set; }

    public ProfileFileDialog()
    {
        InitializeComponent();
    }

    private void ProfileFileDialog_Loaded(object sender, RoutedEventArgs e)
    {
        var root = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        var lyDirectory = Path.Combine(root, "LY");

        if (!Directory.Exists(lyDirectory))
        {
            MessageBox.Show(
                this,
                $"The LY directory was not found.\n\nExpected path:\n{lyDirectory}",
                "LY directory not found",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            DialogResult = false;
            return;
        }

        try
        {
            FilesListBox.ItemsSource = Directory
                .EnumerateFiles(lyDirectory, "*.ly", SearchOption.TopDirectoryOnly)
                .Select(filePath => new FileInfo(filePath).Name)
                .OrderBy(fileName => fileName, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                this,
                ex.Message,
                "Unable to load profile files",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            DialogResult = false;
        }
    }

    private void FilesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (FilesListBox.SelectedItem is not string fileName)
        {
            return;
        }

        SelectedFileName = fileName;
        DialogResult = true;
    }
}
