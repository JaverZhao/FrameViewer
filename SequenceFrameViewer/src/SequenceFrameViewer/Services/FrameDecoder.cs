using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SequenceFrameViewer.Services;

public class FrameDecoder
{
    private readonly FrameCache? _cache;

    public FrameDecoder(FrameCache? cache = null)
    {
        _cache = cache;
    }

    public BitmapSource? Decode(string filePath)
    {
        if (_cache != null)
        {
            var cached = _cache.Get(filePath);
            if (cached != null)
                return cached;
        }

        try
        {
            if (!File.Exists(filePath))
                return null;

            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            bitmap.UriSource = new Uri(filePath);
            bitmap.EndInit();
            bitmap.Freeze();

            _cache?.Add(filePath, bitmap);
            return bitmap;
        }
        catch
        {
            return null;
        }
    }

    public async Task<BitmapSource?> DecodeAsync(string filePath)
    {
        return await Task.Run(() => Decode(filePath));
    }
}
