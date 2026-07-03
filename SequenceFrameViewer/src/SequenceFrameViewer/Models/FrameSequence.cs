using System.Collections.Generic;

namespace SequenceFrameViewer.Models;

public class FrameSequence
{
    public string FolderPath { get; set; } = string.Empty;
    public List<FrameItem> Frames { get; set; } = new();
    public int TotalFrames => Frames.Count;
    public bool IsEmpty => Frames.Count == 0;
}
