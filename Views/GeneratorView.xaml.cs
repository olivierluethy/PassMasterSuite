using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using PassMasterSuite.Core;

namespace PassMasterSuite.Views;

public partial class GeneratorView : UserControl
{
    private bool _ready;
    private bool _revealed = true;
    private string _password = "";

    public GeneratorView()
    {
        InitializeComponent();
        _ready = true;
        ApplyRevealGlyph();
        Regenerate();
    }

    private void Length_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (_ready) Regenerate();
    }

    private void Option_Changed(object sender, RoutedEventArgs e)
    {
        if (!_ready) return;

        // Never allow every class to be off — keep the last one on.
        if (NoClassSelected())
        {
            if (sender is ToggleButton tb)
            {
                _ready = false;
                tb.IsChecked = true;
                _ready = true;
            }
            return;
        }
        Regenerate();
    }

    private void Regen_Click(object sender, RoutedEventArgs e) => Regenerate();

    private void Regenerate()
    {
        var options = new GeneratorOptions
        {
            Length = (int)Math.Round(SlLength.Value),
            UseLower = ChipLower.IsChecked == true,
            UseUpper = ChipUpper.IsChecked == true,
            UseDigits = ChipDigits.IsChecked == true,
            UseSymbols = ChipSymbols.IsChecked == true,
        };

        _password = PasswordGenerator.Generate(options);
        UpdateDisplay();
        UpdateStrength();
    }

    private void UpdateDisplay()
    {
        PwBox.Text = _revealed ? _password : new string('•', _password.Length);
    }

    private void UpdateStrength()
    {
        var result = PasswordStrength.Evaluate(_password);
        Meter.Update(result);
        EntropyNote.Text = result.IsEmpty
            ? ""
            : $"{result.Length} chars · {result.EntropyBits:0} bits of entropy";
    }

    private void Reveal_Click(object sender, RoutedEventArgs e)
    {
        _revealed = !_revealed;
        ApplyRevealGlyph();
        UpdateDisplay();
    }

    private void ApplyRevealGlyph()
    {
        BtnReveal.Content = (string)FindResource(_revealed ? "Glyph.Hide" : "Glyph.Show");
    }

    private void Copy_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(_password)) return;
        try
        {
            Clipboard.SetText(_password);
            FlashCopied();
        }
        catch
        {
            // Clipboard can be briefly locked by another process; ignore silently.
        }
    }

    private void FlashCopied()
    {
        CopiedNote.BeginAnimation(OpacityProperty, new DoubleAnimation
        {
            From = 1,
            To = 0,
            Duration = TimeSpan.FromMilliseconds(1500),
            BeginTime = TimeSpan.FromMilliseconds(200),
        });
    }

    private bool NoClassSelected() =>
        ChipLower.IsChecked != true && ChipUpper.IsChecked != true &&
        ChipDigits.IsChecked != true && ChipSymbols.IsChecked != true;
}
