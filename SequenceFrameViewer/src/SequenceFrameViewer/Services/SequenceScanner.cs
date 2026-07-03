using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SequenceFrameViewer.Helpers;
using SequenceFrameViewer.Models;

namespace SequenceFrameViewer.Services;

public class SequenceScanner
{
    private static readonly HashSet<string> SupportedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".png",
        ".jpg",
        ".jpeg",
        ".webp",
        ".bmp"
    };

    private static readonly NaturalSortComparer Sorter = new();

    public FrameSequence Scan(string folderPath)
    {
        var sequence = new FrameSequence { FolderPath = folderPath };

        if (!Directory.Exists(folderPath))
            return sequence;

        try
        {
            var files = Directory.EnumerateFiles(folderPath)
                .Where(f => SupportedExtensions.Contains(Path.GetExtension(f)))
                .OrderBy(f => f, Sorter)
                .ToList();

            for (int i = 0; i < files.Count; i++)
            {
                var filePath = files[i];
                try
                {
                    var fileInfo = new FileInfo(filePath);
                    sequence.Frames.Add(new FrameItem
                    {
                        Index = i,
                        FilePath = filePath,
                        FileName = fileInfo.Name,
                        FileSize = fileInfo.Length
                    });
                }
                catch
                {
                    // Skip files that can't be accessed
                }
            }
        }
        catch
        {
            // Return empty sequence on error
        }

        return sequence;
    }
}
