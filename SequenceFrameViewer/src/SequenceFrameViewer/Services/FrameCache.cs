using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace SequenceFrameViewer.Services;

public class FrameCache
{
    private readonly int _maxItems;
    private readonly Dictionary<string, LinkedListNode<CacheItem>> _map;
    private readonly LinkedList<CacheItem> _list;

    public FrameCache(int maxItems = 60)
    {
        _maxItems = maxItems;
        _map = new Dictionary<string, LinkedListNode<CacheItem>>(StringComparer.OrdinalIgnoreCase);
        _list = new LinkedList<CacheItem>();
    }

    public BitmapSource? Get(string key)
    {
        if (_map.TryGetValue(key, out var node))
        {
            _list.Remove(node);
            _list.AddFirst(node);
            return node.Value.Image;
        }

        return null;
    }

    public void Add(string key, BitmapSource image)
    {
        if (_map.TryGetValue(key, out var existingNode))
        {
            _list.Remove(existingNode);
            _list.AddFirst(existingNode);
            existingNode.Value.Image = image;
            return;
        }

        while (_map.Count >= _maxItems)
        {
            var last = _list.Last;
            if (last != null)
            {
                _map.Remove(last.Value.Key);
                _list.RemoveLast();
            }
        }

        var item = new CacheItem { Key = key, Image = image };
        var newNode = _list.AddFirst(item);
        _map[key] = newNode;
    }

    public void Remove(string key)
    {
        if (_map.TryGetValue(key, out var node))
        {
            _list.Remove(node);
            _map.Remove(key);
        }
    }

    public void Clear()
    {
        _map.Clear();
        _list.Clear();
    }

    public int Count => _map.Count;

    private class CacheItem
    {
        public string Key { get; set; } = string.Empty;
        public BitmapSource? Image { get; set; }
    }
}
