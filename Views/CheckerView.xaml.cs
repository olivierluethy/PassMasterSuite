using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using PassMasterSuite.Core;

namespace PassMasterSuite.Views;

public partial class CheckerView : UserControl
{
    private bool _revealed;
    private bool _syncing;
    private readonly List<(AttackerPreset Preset, TextBlock Value)> _crackRows = new();

    public CheckerView()
    {
        InitializeComponent();
        BuildCrackRows();
        ApplyRevealGlyph();
        Evaluate("");
    }

    private void BuildCrackRows()
    {
        foreach (var preset in AttackerPreset.All)
        {
            var row = new Grid { Margin = new Thickness(0, 0, 0, 12) };
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            var left = new StackPanel();
            left.Children.Add(new TextBlock
            {
                Text = preset.Name,
                Style = (Style)FindResource("Text.Body"),
            });
            left.Children.Add(new TextBlock
            {
                Text = preset.Description,
                Style = (Style)FindResource("Text.Muted"),
                Margin = new Thickness(0, 2, 0, 0),
            });
            Grid.SetColumn(left, 0);

            var value = new TextBlock
            {
                Style = (Style)FindResource("Text.Mono"),
                FontSize = 14,
                VerticalAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Right,
                Text = "—",
            };
            var badge = new Border
            {
                CornerRadius = new CornerRadius(999),
                Background = Brush("Brush.SurfaceRaised"),
                BorderBrush = Brush("Brush.Border"),
                BorderThickness = new Thickness(1),
                Padding = new Thickness(12, 5, 12, 5),
                Margin = new Thickness(16, 0, 0, 0),
                VerticalAlignment = VerticalAlignment.Center,
                Child = value,
            };
            Grid.SetColumn(badge, 1);

            row.Children.Add(left);
            row.Children.Add(badge);
            CrackRows.Children.Add(row);
            _crackRows.Add((preset, value));
        }
    }

    private string CurrentText() => _revealed ? PwPlain.Text : PwSecure.Password;

    private void Secure_Changed(object sender, RoutedEventArgs e)
    {
        if (!_syncing) Evaluate(CurrentText());
    }

    private void Plain_Changed(object sender, TextChangedEventArgs e)
    {
        if (!_syncing) Evaluate(CurrentText());
    }

    private void Reveal_Click(object sender, RoutedEventArgs e)
    {
        _syncing = true;
        _revealed = !_revealed;
        if (_revealed)
        {
            PwPlain.Text = PwSecure.Password;
            PwSecure.Visibility = Visibility.Collapsed;
            PwPlain.Visibility = Visibility.Visible;
            PwPlain.CaretIndex = PwPlain.Text.Length;
            PwPlain.Focus();
        }
        else
        {
            PwSecure.Password = PwPlain.Text;
            PwPlain.Visibility = Visibility.Collapsed;
            PwSecure.Visibility = Visibility.Visible;
            PwSecure.Focus();
        }
        _syncing = false;
        ApplyRevealGlyph();
    }

    private void ApplyRevealGlyph()
    {
        BtnReveal.Content = (string)FindResource(_revealed ? "Glyph.Hide" : "Glyph.Show");
    }

    private void Evaluate(string password)
    {
        var result = PasswordStrength.Evaluate(password);
        Meter.Update(result);
        EntropyNote.Text = result.IsEmpty ? "" : $"{result.Length} chars · {result.EntropyBits:0} bits of entropy";

        SetIcon(IcoLen, result.HasLength);
        SetIcon(IcoLower, result.HasLower);
        SetIcon(IcoUpper, result.HasUpper);
        SetIcon(IcoDigit, result.HasDigit);
        SetIcon(IcoSymbol, result.HasSymbol);

        FillList(WeakList, WeakPanel, result.Warnings, "Brush.Danger", result.IsEmpty);
        FillList(TipList, TipPanel, result.Suggestions, "Brush.TextSecondary", result.IsEmpty);

        UpdateCrackTimes(password);
    }

    private void UpdateCrackTimes(string password)
    {
        if (string.IsNullOrEmpty(password))
        {
            foreach (var (_, value) in _crackRows) value.Text = "—";
            return;
        }

        var cs = Charset.Analyze(password);
        foreach (var (preset, value) in _crackRows)
        {
            double seconds = CrackTime.AverageSeconds(cs.Size, password.Length, preset.GuessesPerSecond);
            value.Text = DurationFormat.Headline(seconds);
        }
    }

    private void SetIcon(TextBlock icon, bool met)
    {
        icon.Text = (string)FindResource(met ? "Glyph.Check" : "Glyph.Cancel");
        icon.Foreground = met ? Brush("Brush.Success") : Brush("Brush.TextMuted");
    }

    private void FillList(StackPanel list, StackPanel panel, IReadOnlyList<string> lines, string colorKey, bool isEmpty)
    {
        list.Children.Clear();
        if (isEmpty || lines.Count == 0)
        {
            panel.Visibility = Visibility.Collapsed;
            return;
        }

        foreach (string line in lines)
        {
            var row = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 6) };
            row.Children.Add(new TextBlock
            {
                Text = "•",
                Foreground = Brush(colorKey),
                Margin = new Thickness(0, 0, 8, 0),
                FontSize = 14,
            });
            row.Children.Add(new TextBlock
            {
                Text = line,
                Style = (Style)FindResource("Text.Small"),
                MaxWidth = 640,
            });
            list.Children.Add(row);
        }
        panel.Visibility = Visibility.Visible;
    }

    private SolidColorBrush Brush(string key) => (SolidColorBrush)(TryFindResource(key) ?? Brushes.Gray);
}
