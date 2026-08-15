using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Threading;
using PassMasterSuite.Core;

namespace PassMasterSuite.Views;

public partial class CrackerView : UserControl
{
    private enum RunState { Idle, Running, Cracked, Stopped }

    private const int Frames = 150;       // animation length for a realizable crack (~5s at 33ms)
    private const int TickerEvery = 3;    // add a ticker row every N frames
    private const int MaxTickerRows = 12;

    private readonly DispatcherTimer _timer;
    private readonly ToggleButton[] _chips;
    private readonly AttackerPreset[] _presets;
    private int _presetIndex = 2;         // Offline (fast hash)

    private RunState _state = RunState.Idle;
    private CrackModel? _model;
    private int _frame;

    public CrackerView()
    {
        InitializeComponent();

        _chips = new[] { Chip0, Chip1, Chip2, Chip3 };
        _presets = AttackerPreset.All.ToArray();

        _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(33) };
        _timer.Tick += Tick;

        Unloaded += (_, _) => { if (_state == RunState.Running) StopRun(RunState.Stopped); };

        SyncPresetSelection();
        UpdateEstimate();
    }

    private AttackerPreset Preset => _presets[_presetIndex];

    // ── Input & preset ─────────────────────────────────────────
    private void Input_Changed(object sender, TextChangedEventArgs e)
    {
        InputError.Visibility = Visibility.Collapsed;
        if (_state != RunState.Running) UpdateEstimate();
    }

    private void Preset_Click(object sender, RoutedEventArgs e)
    {
        int idx = Array.IndexOf(_chips, sender);
        if (idx >= 0) _presetIndex = idx;
        SyncPresetSelection();
        if (_state != RunState.Running) UpdateEstimate();
    }

    private void SyncPresetSelection()
    {
        for (int i = 0; i < _chips.Length; i++)
            _chips[i].IsChecked = i == _presetIndex;
        PresetDesc.Text = Preset.Description;
    }

    private void UpdateEstimate()
    {
        string pw = PwInput.Text;
        if (string.IsNullOrEmpty(pw))
        {
            EstHeadline.Text = "—";
            EstMeta.Text = "";
            return;
        }

        var cs = Charset.Analyze(pw);
        double avg = CrackTime.AverageSeconds(cs.Size, pw.Length, Preset.GuessesPerSecond);
        EstHeadline.Text = DurationFormat.Headline(avg);
        EstMeta.Text = $"{cs.Size}-symbol alphabet · {pw.Length} characters · {Preset.Name}";
    }

    // ── Run control ────────────────────────────────────────────
    private void Start_Click(object sender, RoutedEventArgs e)
    {
        string pw = PwInput.Text;
        if (string.IsNullOrEmpty(pw))
        {
            InputError.Visibility = Visibility.Visible;
            return;
        }

        _model = CrackModel.Build(pw, Preset);
        _frame = 0;
        _state = RunState.Running;
        Ticker.Children.Clear();

        BtnStart.Visibility = Visibility.Collapsed;
        BtnStop.Visibility = Visibility.Visible;
        PwInput.IsEnabled = false;
        RunPanel.Visibility = Visibility.Visible;
        Prog.Value = 0;

        CandidateBox.BorderBrush = Brush("Brush.Accent");
        CandidateBox.Effect = FindEffect("Effect.AccentGlow");
        CurrentCandidate.Foreground = Brush("Brush.Accent");
        StatusText.Foreground = Brush("Brush.TextSecondary");
        StatusText.Text = _model.Realizable
            ? "Guessing every combination in order until it matches…"
            : "This password is strong — showing how the guessing would look while the clock runs.";
        CountdownText.Text = DurationFormat.Countdown(_model.AverageSeconds);

        _timer.Start();
    }

    private void Stop_Click(object sender, RoutedEventArgs e) => StopRun(RunState.Stopped);

    private void Tick(object? sender, EventArgs e)
    {
        if (_model is null) { StopRun(RunState.Stopped); return; }

        _frame++;

        if (_model.Realizable)
        {
            double t = Math.Min(1.0, (double)_frame / Frames);
            BigInteger index = _model.TargetIndex * _frame / Frames;
            string candidate = _frame >= Frames ? _model.Target : _model.CandidateAt(index);

            CurrentCandidate.Text = candidate;
            Prog.Value = t * 100;
            CountdownText.Text = DurationFormat.Countdown(_model.AverageSeconds * (1 - t));

            if (_frame % TickerEvery == 0) AddTickerRow(candidate, false);

            if (_frame >= Frames) OnCracked();
        }
        else
        {
            string candidate = _model.RandomCandidate();
            CurrentCandidate.Text = candidate;
            Prog.Value = _frame % 100;                     // a moving activity sweep
            CountdownText.Text = DurationFormat.Headline(_model.AverageSeconds);
            if (_frame % TickerEvery == 0) AddTickerRow(candidate, false);
        }
    }

    private void OnCracked()
    {
        _timer.Stop();
        _state = RunState.Cracked;

        RunPanel.Visibility = Visibility.Collapsed;   // progress + countdown disappear the instant it ends
        BtnStop.Visibility = Visibility.Collapsed;
        BtnStart.Visibility = Visibility.Visible;
        PwInput.IsEnabled = true;

        CurrentCandidate.Text = _model!.Target;
        CurrentCandidate.Foreground = Brush("Brush.Success");
        CandidateBox.BorderBrush = Brush("Brush.Success");
        CandidateBox.Effect = FindEffect("Effect.SuccessGlow");

        StatusText.Foreground = Brush("Brush.Success");
        StatusText.Text = $"Cracked! The password “{_model.Target}” was found by brute force.";
        AddTickerRow(_model.Target, true);
    }

    private void StopRun(RunState state)
    {
        _timer.Stop();
        _state = state;

        RunPanel.Visibility = Visibility.Collapsed;
        BtnStop.Visibility = Visibility.Collapsed;
        BtnStart.Visibility = Visibility.Visible;
        PwInput.IsEnabled = true;

        CandidateBox.BorderBrush = Brush("Brush.Border");
        CandidateBox.Effect = null;
        CurrentCandidate.Foreground = Brush("Brush.Accent");

        if (state == RunState.Stopped)
        {
            StatusText.Foreground = Brush("Brush.TextSecondary");
            StatusText.Text = "Stopped. Adjust the password or attacker speed and start again.";
        }
    }

    // ── Ticker ─────────────────────────────────────────────────
    private void AddTickerRow(string candidate, bool success)
    {
        // Dim existing rows so only the newest stands out.
        foreach (var child in Ticker.Children)
            if (child is TextBlock old)
                old.Foreground = Brush("Brush.TextMuted");

        var row = new TextBlock
        {
            Text = candidate,
            FontFamily = new FontFamily("Consolas"),
            FontSize = 14,
            Margin = new Thickness(0, 2, 0, 2),
            Foreground = success ? Brush("Brush.Success") : Brush("Brush.Accent"),
            TextTrimming = TextTrimming.CharacterEllipsis,
        };
        Ticker.Children.Insert(0, row);

        while (Ticker.Children.Count > MaxTickerRows)
            Ticker.Children.RemoveAt(Ticker.Children.Count - 1);

        row.BeginAnimation(OpacityProperty, new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(140)));
    }

    // ── Helpers ────────────────────────────────────────────────
    private SolidColorBrush Brush(string key) => (SolidColorBrush)(TryFindResource(key) ?? Brushes.Gray);
    private Effect? FindEffect(string key) => TryFindResource(key) as Effect;
}
