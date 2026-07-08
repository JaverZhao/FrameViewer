using System;
using System.Windows.Threading;
using SequenceFrameViewer.Models;

namespace SequenceFrameViewer.Services;

public enum PlaybackState
{
    Idle,
    Playing,
    Paused
}

public class PlaybackEngine
{
    private readonly DispatcherTimer _timer;
    private FrameSequence? _sequence;
    private int _currentIndex;
    private double _fps = 24;
    private bool _loop;

    public event Action<int>? FrameChanged;
    public event Action? PlaybackEnded;

    public PlaybackState State { get; private set; } = PlaybackState.Idle;
    public int CurrentIndex => _currentIndex;
    public int TotalFrames => _sequence?.TotalFrames ?? 0;

    public double Fps
    {
        get => _fps;
        set
        {
            _fps = Math.Clamp(value, 1, 120);
            if (State == PlaybackState.Playing)
                UpdateTimerInterval();
        }
    }

    public bool Loop
    {
        get => _loop;
        set => _loop = value;
    }

    public PlaybackEngine()
    {
        _timer = new DispatcherTimer();
        _timer.Tick += OnTimerTick;
        UpdateTimerInterval();
    }

    public void LoadSequence(FrameSequence sequence)
    {
        Stop();
        _sequence = sequence;
        _currentIndex = 0;
    }

    public void Play()
    {
        if (_sequence == null || _sequence.IsEmpty)
            return;

        State = PlaybackState.Playing;
        UpdateTimerInterval();
        _timer.Start();
    }

    public void Pause()
    {
        if (State != PlaybackState.Playing)
            return;

        State = PlaybackState.Paused;
        _timer.Stop();
    }

    public void Stop()
    {
        State = PlaybackState.Idle;
        _timer.Stop();
    }

    public void TogglePlayPause()
    {
        if (State == PlaybackState.Playing)
            Pause();
        else
            Play();
    }

    public void GoToFrame(int index)
    {
        if (_sequence == null || _sequence.IsEmpty)
            return;

        _currentIndex = Math.Clamp(index, 0, _sequence.TotalFrames - 1);
        FrameChanged?.Invoke(_currentIndex);
    }

    public void NextFrame()
    {
        if (_sequence == null || _sequence.IsEmpty)
            return;

        _currentIndex = (_currentIndex + 1) % _sequence.TotalFrames;
        FrameChanged?.Invoke(_currentIndex);
    }

    public void PreviousFrame()
    {
        if (_sequence == null || _sequence.IsEmpty)
            return;

        _currentIndex--;
        if (_currentIndex < 0)
            _currentIndex = _sequence.TotalFrames - 1;

        FrameChanged?.Invoke(_currentIndex);
    }

    private void OnTimerTick(object? sender, EventArgs e)
    {
        if (_sequence == null || _sequence.IsEmpty)
        {
            Stop();
            return;
        }

        _currentIndex++;

        if (_currentIndex >= _sequence.TotalFrames)
        {
            if (_loop)
            {
                _currentIndex = 0;
            }
            else
            {
                _currentIndex = _sequence.TotalFrames - 1;
                Stop();
                FrameChanged?.Invoke(_currentIndex);
                PlaybackEnded?.Invoke();
                return;
            }
        }

        FrameChanged?.Invoke(_currentIndex);
    }

    private void UpdateTimerInterval()
    {
        _timer.Interval = TimeSpan.FromMilliseconds(1000.0 / _fps);
    }
}
