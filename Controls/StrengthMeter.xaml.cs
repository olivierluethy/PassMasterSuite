using System.Windows.Controls;
using System.Windows.Media;
using PassMasterSuite.Core;

namespace PassMasterSuite.Controls;

public partial class StrengthMeter : UserControl
{
    private Border[] _segments = Array.Empty<Border>();

    public StrengthMeter()
    {
        InitializeComponent();
        _segments = new[] { Seg0, Seg1, Seg2, Seg3, Seg4 };
    }

    public void Update(StrengthResult result)
    {
        var empty = Brush("Brush.Border");

        if (result.IsEmpty)
        {
            foreach (var seg in _segments) seg.Background = empty;
            Rating.Text = result.Rating;
            Rating.Foreground = Brush("Brush.TextMuted");
            return;
        }

        var fill = Brush($"Brush.Strength{result.Level}");
        for (int i = 0; i < _segments.Length; i++)
            _segments[i].Background = i <= result.Level ? fill : empty;

        Rating.Text = result.Rating;
        Rating.Foreground = fill;
    }

    private SolidColorBrush Brush(string key) =>
        (SolidColorBrush)(TryFindResource(key) ?? Brushes.Gray);
}
