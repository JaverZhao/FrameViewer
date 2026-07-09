using System;
using System.Reflection;
using System.Windows;
using System.Windows.Media.Imaging;
using SequenceFrameViewer.Resources;

namespace SequenceFrameViewer.Views;

public partial class AboutWindow : Window
{
    public AboutWindow()
    {
        InitializeComponent();

        var appVersion = Assembly.GetExecutingAssembly()
            .GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version ?? "1.0.0";
        VersionText.Text = string.Format(LocalizationService.Default.VersionFormat, appVersion);

        var iconPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Icons", "app.ico");
        if (System.IO.File.Exists(iconPath))
            Icon = new BitmapImage(new Uri(iconPath));
    }

    private void OnOkClick(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
