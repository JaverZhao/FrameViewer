using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace SequenceFrameViewer.Views;

public partial class AboutWindow : Window
{
    public AboutWindow()
    {
        InitializeComponent();
        var iconPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Icons", "app.ico");
        if (System.IO.File.Exists(iconPath))
            Icon = new BitmapImage(new Uri(iconPath));
    }

    private void OnOkClick(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
