using System;
using System.IO;

namespace SequenceFrameViewer.Services;

public static class LogService
{
    private static readonly string LogDir = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "SequenceFrameViewer",
        "logs");

    private static readonly object _lock = new();

    static LogService()
    {
        try
        {
            if (!Directory.Exists(LogDir))
                Directory.CreateDirectory(LogDir);
        }
        catch
        {
            // Cannot create log directory - skip logging
        }
    }

    public static void Info(string message)
    {
        Write("INFO", message);
    }

    public static void Warning(string message)
    {
        Write("WARN", message);
    }

    public static void Error(string message, Exception? ex = null)
    {
        var msg = ex != null ? $"{message} | {ex.GetType().Name}: {ex.Message}" : message;
        Write("ERROR", msg);
    }

    private static void Write(string level, string message)
    {
        try
        {
            var logFile = Path.Combine(LogDir, $"log-{DateTime.Now:yyyy-MM-dd}.txt");
            lock (_lock)
            {
                File.AppendAllText(logFile, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}{Environment.NewLine}");
            }
        }
        catch
        {
            // Silent fail on logging errors
        }
    }
}
