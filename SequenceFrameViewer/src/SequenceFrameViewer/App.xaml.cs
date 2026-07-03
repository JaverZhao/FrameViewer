using System;
using System.Windows;
using System.Windows.Threading;
using SequenceFrameViewer.Services;

namespace SequenceFrameViewer;

public partial class App : Application
{
    public App()
    {
        DispatcherUnhandledException += OnDispatcherUnhandledException;
        AppDomain.CurrentDomain.UnhandledException += OnAppDomainUnhandledException;
    }

    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        LogService.Error("UI thread exception", e.Exception);
        MessageBox.Show(
            $"发生未处理的异常:\n{e.Exception.Message}",
            "FrameView - 错误",
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
