using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace FilepathFooter.Editor
{
    /// <summary>
    /// A thin WPF margin rendered below the editor's horizontal scroll bar.
    /// It shows the folder path of the active document and copies it to the
    /// clipboard when the user clicks it, then briefly confirms the action.
    /// </summary>
    internal sealed class FilepathFooterMargin : Grid, IWpfTextViewMargin
    {
        public const string MarginName = "FilepathFooterMargin";

        private const double MarginHeight = 24;
        private const int CopiedFeedbackMs = 2000;

        private readonly TextBlock _pathTextBlock;
        private readonly DispatcherTimer _resetTimer;
        private string _filePath = string.Empty;
        private bool _isDisposed;

        public FilepathFooterMargin(
            IWpfTextView textView,
            ITextDocumentFactoryService textDocumentFactoryService)
        {
            Height = MarginHeight;

            // ── Background: matches the VS status-bar colour, adapts to theme ──
            SetResourceReference(BackgroundProperty, EnvironmentColors.StatusBarDefaultBrushKey);

            // ── Thin top separator ──
            var separator = new Border
            {
                Height = 1,
                VerticalAlignment = VerticalAlignment.Top,
            };
            separator.SetResourceReference(
                Border.BackgroundProperty,
                EnvironmentColors.CommandBarMenuBorderBrushKey);
            Children.Add(separator);

            // ── Path label ──
            _pathTextBlock = new TextBlock
            {
                VerticalAlignment = VerticalAlignment.Center,
                Margin            = new Thickness(8, 0, 8, 0),
                FontSize          = 11,
                Cursor            = Cursors.Hand,
                ToolTip           = "Click to copy file path to clipboard",
                TextTrimming      = TextTrimming.CharacterEllipsis,
            };
            _pathTextBlock.SetResourceReference(
                TextBlock.ForegroundProperty,
                EnvironmentColors.StatusBarDefaultTextBrushKey);
            Children.Add(_pathTextBlock);

            // ── Timer that reverts the "copied" feedback ──
            _resetTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(CopiedFeedbackMs),
            };
            _resetTimer.Tick += OnResetTimerTick;

            // ── Populate from the current document ──
            if (textDocumentFactoryService.TryGetTextDocument(
                    textView.TextBuffer, out var document))
            {
                SetFolderPath(document.FilePath);

                // Keep in sync when the file is saved-as or renamed.
                document.FileActionOccurred += (_, e) =>
                    Dispatcher.BeginInvoke(
                        new Action(() => SetFolderPath(e.FilePath)));
            }

            _pathTextBlock.MouseLeftButtonUp += OnPathClicked;
        }

        // ────────────────────────────────────────────────────────────────────
        //  Helpers
        // ────────────────────────────────────────────────────────────────────

        private void SetFolderPath(string? filePath)
        {
            _filePath = string.IsNullOrEmpty(filePath)
                ? string.Empty
                : filePath;

            ShowNormalState();
        }

        private void ShowNormalState()
        {
            _pathTextBlock.Text = _filePath;
            _pathTextBlock.SetResourceReference(
                TextBlock.ForegroundProperty,
                EnvironmentColors.StatusBarDefaultTextBrushKey);
        }

        private void ShowCopiedState()
        {
            _pathTextBlock.Text     = "✓  File path copied to clipboard";
            _pathTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(0x4E, 0xC9, 0x4E)); // green
        }

        // ────────────────────────────────────────────────────────────────────
        //  Event handlers
        // ────────────────────────────────────────────────────────────────────

        private void OnPathClicked(object sender, MouseButtonEventArgs e)
        {
            if (string.IsNullOrEmpty(_filePath))
                return;

            try
            {
                Clipboard.SetText(_filePath);
                _resetTimer.Stop();
                ShowCopiedState();
                _resetTimer.Start();
            }
            catch (Exception)
            {
                // Clipboard operations can fail if another process owns the lock.
            }
        }

        private void OnResetTimerTick(object? sender, EventArgs e)
        {
            _resetTimer.Stop();
            ShowNormalState();
        }

        // ────────────────────────────────────────────────────────────────────
        //  IWpfTextViewMargin
        // ────────────────────────────────────────────────────────────────────

        public FrameworkElement VisualElement => this;

        public double MarginSize => MarginHeight;

        public bool Enabled => true;

        public ITextViewMargin? GetTextViewMargin(string marginName)
            => marginName == MarginName ? this : null;

        public void Dispose()
        {
            if (_isDisposed)
                return;

            _isDisposed = true;
            _resetTimer.Stop();
            _resetTimer.Tick            -= OnResetTimerTick;
            _pathTextBlock.MouseLeftButtonUp -= OnPathClicked;
        }
    }
}
