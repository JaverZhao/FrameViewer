using System;
using System.IO;
using System.Text.Json;
using SequenceFrameViewer.Models;

namespace SequenceFrameViewer.Services;

public class SettingsService
{
    private static readonly string SettingsDir = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "SequenceFrameViewer");

    private static readonly string SettingsPath = Path.Combine(SettingsDir, "settings.json");

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    private AppSettings _settings = new();

    public AppSettings Load()
    {
        try
        {
            if (!File.Exists(SettingsPath))
                return _settings = new AppSettings();

            var json = File.ReadAllText(SettingsPath);
            _settings = JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
        }
        catch
        {
            _settings = new AppSettings();
        }

        return _settings;
    }

    public void Save(AppSettings settings)
    {
        try
        {
            if (!Directory.Exists(SettingsDir))
                Directory.CreateDirectory(SettingsDir);

            _settings = settings;
            var json = JsonSerializer.Serialize(_settings, JsonOptions);
            File.WriteAllText(SettingsPath, json);
        }
        catch
        {
            // Silently fail - settings save should not crash the app
        }
    }
}
