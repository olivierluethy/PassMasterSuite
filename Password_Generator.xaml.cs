using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Password_Checker__Generator_and_Cracker
{
  /// <summary>
  /// Interaktionslogik für Password_Generator.xaml
  /// </summary>
  public partial class Password_Generator : Window
  {
    public Password_Generator()
    {
      InitializeComponent();
    }

    public void RandomPasswordGenerator()
    {
        int len = (byte)slLength.Value;
        int digits = (byte)slDigits.Value;
        int capitals = (byte)slCapitals.Value;
        int symbols = (byte)slSymbols.Value;

        // Überprüfen, ob die Summe der Anforderungen die Passwortlänge überschreitet
        int sum = digits + capitals + symbols;
        if (sum > len)
        {
            // Passen Sie die Länge entsprechend an, um Platz für alle Anforderungen zu schaffen
            len = sum;
            slLength.Value = len;
        }

        const string ValidAbcLow = "abcdefghijklmnopqrstuvwxyz";
        const string ValidDigits = "1234567890";
        const string ValidCapitals = "ABZDEFGHIJKLMNOPQRSTUVWXYZ";
        const string ValidSymbols = "^'=)(/&%ç*+°¦@#°§¬|¢´~!èà£[]{}öé-_.:,;";

            if (slLength.Value > 25)
            {
                tbxPassword.Margin = new Thickness(170, 0, 20, 0);

            }
            else
            {
                tbxPassword.Margin = new Thickness(170, 0, 40, 51);
                btnRefresh.Margin = new Thickness(0, 0, 167, 51);
            }

            StringBuilder resultDigits = new StringBuilder();
        StringBuilder resultLow = new StringBuilder();
        StringBuilder resultCapitals = new StringBuilder();
        StringBuilder resultSymbols = new StringBuilder();

        Random rand = new Random();

        for (int i = 0; i < digits; i++)
        {
            resultDigits.Append(ValidDigits[rand.Next(ValidDigits.Length)]);
        }

        for (int i = 0; i < capitals; i++)
        {
            resultCapitals.Append(ValidCapitals[rand.Next(ValidCapitals.Length)]);
        }

        for (int i = 0; i < symbols; i++)
        {
            resultSymbols.Append(ValidSymbols[rand.Next(ValidSymbols.Length)]);
        }

        // Berechnen Sie die Anzahl der Kleinbuchstaben, die benötigt werden
        int remainingChars = len - (digits + capitals + symbols);

        for (int i = 0; i < remainingChars; i++)
        {
            resultLow.Append(ValidAbcLow[rand.Next(ValidAbcLow.Length)]);
        }

        string finalresult = resultDigits.ToString() + resultCapitals.ToString() + resultSymbols.ToString() + resultLow.ToString();

        // Mischen Sie das Ergebnis, um die Reihenfolge zufällig zu machen
        string shuffledResult = new string(finalresult.OrderBy(c => rand.Next()).ToArray());

        tbxPassword.Text = shuffledResult;

        // Kopieren Sie das Passwort in die Zwischenablage
        Clipboard.SetText(shuffledResult);
        lblCopy.Visibility = Visibility.Visible;
    }

        private void ColorSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider changedSlider = sender as Slider;

            double totalCharacterCount = slDigits.Value + slCapitals.Value + slSymbols.Value;

            if (totalCharacterCount > slLength.Value)
            {
                // If the length is fixed at 50 and the total characters exceed it
                if (slLength.Value == 50 && totalCharacterCount > 50)
                {
                    MessageBox.Show("The total count of digits, capitals, and symbols cannot exceed the length of 50!");
                    // Adjust the sliders back to ensure they don't exceed 50
                    AdjustSlidersToMaxLength(changedSlider);
                }
                else
                {
                    // Adjust the length to accommodate the total character count
                    slLength.ValueChanged -= ColorSlider_ValueChanged; // Temporarily remove event handler
                    slLength.Value = totalCharacterCount; // Set the value without triggering event
                    slLength.ValueChanged += ColorSlider_ValueChanged; // Re-attach event handler
                }
            }
            else
            {
                // Regenerate the random password since the total count is within the length limit
                RandomPasswordGenerator();
            }

        }

        private void AdjustSlidersToMaxLength(Slider changedSlider)
        {
            slLength.Value = 50;

            // Create a dictionary of sliders and their names
            Dictionary<string, Slider> sliders = new Dictionary<string, Slider>
    {
        { "slDigits", slDigits },
        { "slCapitals", slCapitals },
        { "slSymbols", slSymbols }
    };

            // Sort the dictionary by slider value in descending order
            var sortedSliders = sliders.OrderByDescending(slider => slider.Value.Value);

            // Get the name and value of the slider with the biggest value
            var maxSlider = sortedSliders.First();

            // Adjust other sliders based on the slider with the maximum value
            if (maxSlider.Value != changedSlider)
            {
                if (maxSlider.Value == slDigits)
                {
                    AdjustSliderValue(slCapitals, slDigits.Value);
                    AdjustSliderValue(slSymbols, slDigits.Value);
                }
                else if (maxSlider.Value == slCapitals)
                {
                    AdjustSliderValue(slDigits, slCapitals.Value);
                    AdjustSliderValue(slSymbols, slCapitals.Value);
                }
                else if (maxSlider.Value == slSymbols)
                {
                    AdjustSliderValue(slDigits, slSymbols.Value);
                    AdjustSliderValue(slCapitals, slSymbols.Value);
                }
            }
        }


        private void AdjustSliderValue(Slider slider, double subtractValue)
        {
            if (slider.Value > 0)
            {
                slider.Value -= subtractValue;
            }
        }


        public void btnRefresh_Click(object sender, RoutedEventArgs e)
    {
      if (slLength.Value == 0)
      {
        MessageBox.Show("Bitte geben Sie eine Länge an!");
      }
      else
      {
        RandomPasswordGenerator();
      }
    }

    private void LengthSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
      double cnt = slDigits.Value + slCapitals.Value + slSymbols.Value;
      if (cnt > slLength.Value)
      {
        MessageBox.Show("Die Länge darf nicht kleiner sein!");
        slLength.Value = slDigits.Value + slCapitals.Value + slSymbols.Value;
      }
      else
      {
        RandomPasswordGenerator();
      }
    }

    private void btnHome_Click(object sender, RoutedEventArgs e)
    {
      this.Hide();
      MainWindow f1 = new MainWindow();
      f1.ShowDialog();
      this.Close();
    }
  }
}
