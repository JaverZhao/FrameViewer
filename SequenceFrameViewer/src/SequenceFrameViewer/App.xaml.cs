using System;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Threading;
using SequenceFrameViewer.Models;
using SequenceFrameViewer.Resources;
using SequenceFrameViewer.Services;

namespace SequenceFrameViewer;

public partial class App : Application
{
    public App()
    {
        DispatcherUnhandledException += OnDispatcherUnhandledException;
        AppDomain.CurrentDomain.UnhandledException += OnAppDomainUnhandledException;
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        try
        {
            var settingsPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "SequenceFrameViewer", "settings.json");
            if (File.Exists(settingsPath))
            {
                var json = File.ReadAllText(settingsPath);
                var settings = JsonSerializer.Deserialize<AppSettings>(json);
                if (settings?.Language == "en")
                    LocalizationService.Default.SetCulture("en");
                else
                    LocalizationService.Default.SetCulture("zh");
            }
        }
        catch { }

        base.OnStartup(e);
    }

    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        LogService.Error("UI thread exception", e.Exception);
        MessageBox.Show(
            string.Format(LocalizationService.Default.UnhandledErrorFormat, e.Exception.Message),
                LocalizationService.Default.ErrorTitle,
            MessageBoxButton.OK,
            MessageBoxImage.Error);
        e.Handled = true;
    }

    private void OnAppDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is Exception ex)
        {
            LogService.Error("AppDomain unhandled exception", ex);
        }
    }
}
