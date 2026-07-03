using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using SequenceFrameViewer.ViewModels;

namespace SequenceFrameViewer;

public partial class MainWindow : Window
{
    private Point _dragStart;
    private bool _isDragging;
    private double _scale = 1.0;
    private double _offsetX;
    private double _offsetY;
    public MainWindow()
    {
        InitializeComponent();
        AllowDrop = true;
        AddHandler(DropEvent, new DragEventHandler(OnDrop));

        PreviewBorder.MouseWheel += OnPreviewMouseWheel;
        PreviewBorder.MouseDown += OnPreviewMouseDown;
        PreviewBorder.MouseMove += OnPreviewMouseMove;
        PreviewBorder.MouseUp += OnPreviewMouseUp;

        FrameSlider.AddHandler(Thumb.DragCompletedEvent, new DragCompletedEventHandler(OnSliderDragCompleted));

        ViewModel.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(MainViewModel.IsFitToWindow))
                ApplyTransform();
            else if (e.PropertyName == nameof(MainViewModel.TotalFrames))
                FrameSlider.Maximum = ViewModel.TotalFrames > 0 ? ViewModel.TotalFrames - 1 : 1;
        };

        PreviewKeyDown += OnPreviewKeyDown;
    }

    private MainViewModel ViewModel => (MainViewModel)DataContext;

    private async void OnRecentFolderSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count > 0 && e.AddedItems[0] is string folder)
        {
            ((System.Windows.Controls.ComboBox)sender).SelectedIndex = -1;
            if (Directory.Exists(folder))
                await ViewModel.LoadFolder(folder);
        }
    }

    private async void OnDrop(object sender, DragEventArgs e)
    {
        if (!e.Data.GetDataPresent(DataFormats.FileDrop))
            return;

        var files = (string[])e.Data.GetData(DataFormats.FileDrop);
        if (files.Length > 0 && Directory.Exists(files[0]))
            await ViewModel.LoadFolder(files[0]);
    }

    private void OnSliderDragCompleted(object sender, DragCompletedEventArgs e)
    {
        ViewModel.SeekToFrame((int)FrameSlider.Value);
    }

    private void OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (FpsTextBox.IsFocused)
            return;

        switch (e.Key)
        {
            case Key.Space:
                ViewModel.PlayPauseCommand.Execute(null);
                e.Handled = true;
                break;
            case Key.Left:
                ViewModel.PreviousFrameCommand.Execute(null);
                e.Handled = true;
                break;
            case Key.Right:
                ViewModel.NextFrameCommand.Execute(null);
                e.Handled = true;
                break;
            case Key.Home:
                ViewModel.GoToFirstFrameCommand.Execute(null);
                e.Handled = true;
                break;
            case Key.End:
                ViewModel.GoToLastFrameCommand.Execute(null);
                e.Handled = true;
                break;
            case Key.F:
                ViewModel.FitWindowCommand.Execute(null);
                e.Handled = true;
                break;
            case Key.D1:
                ViewModel.ZoomOriginalCommand.Execute(null);
                e.Handled = true;
                break;
        }

        if (e.KeyboardDevice.Modifiers == ModifierKeys.Control)
        {
            switch (e.Key)
            {
                case Key.O:
                    ViewModel.OpenFolderCommand.Execute(null);
                    e.Handled = true;
                    break;
                case Key.R:
                    ViewModel.ReloadCommand.Execute(null);
                    e.Handled = true;
                    break;
            }
        }
    }

    private void ApplyTransform()
    {
        if (ViewModel.IsFitToWindow)
        {
            _scale = 1.0;
            _offsetX = 0;
            _offsetY = 0;
            PreviewImage.Stretch = Stretch.Uniform;
        }
        else
        {
            _scale = ViewModel.ZoomLevel;
            PreviewImage.Stretch = Stretch.None;
        }

        ScaleTransform.ScaleX = _scale;
        ScaleTransform.ScaleY = _scale;
        TranslateTransform.X = _offsetX;
        TranslateTransform.Y = _offsetY;
    }

    private void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (ViewModel.IsEmpty || ViewModel.CurrentImage == null)
            return;

        var mousePos = e.GetPosition(PreviewContainer);
        double newScale = e.Delta > 0 ? _scale * 1.1 : _scale / 1.1;
        newScale = Math.Clamp(newScale, 0.1, 10.0);

        double ratio = newScale / _scale;
        _offsetX = mousePos.X - ratio * (mousePos.X - _offsetX);
        _offsetY = mousePos.Y - ratio * (mousePos.Y - _offsetY);
        _scale = newScale;

        ViewModel.SetZoom(_scale);
        PreviewImage.Stretch = Stretch.None;

        ScaleTransform.ScaleX = _scale;
        ScaleTransform.ScaleY = _scale;
        TranslateTransform.X = _offsetX;
        TranslateTransform.Y = _offsetY;
    }

    private void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left && !ViewModel.IsFitToWindow)
        {
            _dragStart = e.GetPosition(PreviewBorder);
            _isDragging = true;
            PreviewBorder.Cursor = Cursors.Hand;
            PreviewBorder.CaptureMouse();
        }
    }

    private void OnPreviewMouseMove(object sender, MouseEventArgs e)
    {
        if (!_isDragging)
            return;

        var pos = e.GetPosition(PreviewBorder);
        _offsetX += pos.X - _dragStart.X;
        _offsetY += pos.Y - _dragStart.Y;
        _dragStart = pos;

        TranslateTransform.X = _offsetX;
        TranslateTransform.Y = _offsetY;
    }

    private void OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
    {
        if (!_isDragging)
            return;

        _isDragging = false;
        PreviewBorder.Cursor = Cursors.Arrow;
        PreviewBorder.ReleaseMouseCapture();
    }

    protected override void OnClosed(EventArgs e)
    {
        ViewModel.SaveSettings();
        base.OnClosed(e);
    }
}
