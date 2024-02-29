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

    // Falls die Anzahl an Digits, Capitals und Symbols die Länge überschreiten, dass man deren Wert zurücksetzen
    // kann.
    int ValueOfDigitsBefore = 0, 
      ValueOfCapitalsBefore = 0, 
      ValueOfSymbolsBefore = 0;

    public void RandomPasswordGenerator()
    {
      int len = (byte)slLength.Value;
      int digits = (byte)slDigits.Value;
      int capitals = (byte)slCapitals.Value;
      int symbols = (byte)slSymbols.Value;

      ValueOfDigitsBefore = digits;
      ValueOfCapitalsBefore = capitals;
      ValueOfSymbolsBefore = symbols;

      // Kleinbuchstaben
      const string ValidAbcLow = "abcdefghijklmnopqrstuvwxyz";

      // Zahlen
      const string ValidDigits = "1234567890";

      // Grossbuchstaben
      const string ValidCapitals = "ABZDEFGHIJKLMNOPQRSTUVWXYZ";

      // Symbolen
      const string ValidSymbols = "^'=)(/&%ç*+°¦@#°§¬|¢´~!èà£[]{}öé-_.:,;";

      if (slLength.Value > 25)
      {/*
        btnRefresh.Margin = new Thickness(337, 91, 325, 68);
        tbxPassword.Margin = new Thickness(125, -10, 122, 119);
        btnCheckPass.Margin = new Thickness(306, 162, 0, 0);*/
        tbxPassword.Margin = new Thickness(170, 0, 20, 0);

      }
      else
      {
        tbxPassword.Margin = new Thickness(170, 0, 40, 51);
        btnRefresh.Margin = new Thickness(0, 0, 167, 51);
      }

      // Random Zahlen generieren
      StringBuilder resultDigits = new StringBuilder();
      Random rand = new Random();
      for (int i = 0; i < digits; i++)
      {
        resultDigits.Append(ValidDigits[rand.Next(ValidDigits.Length)]);
      }
      //while (0 < digits--)
      //{
      //  resultDigits.Append(ValidDigits[rand.Next(ValidDigits.Length)]);
      //}

      // Random Kleinbuchstaben generieren
      StringBuilder resultLow = new StringBuilder();
      len = len - digits - capitals - symbols;
      for (int i = 0; i < len; i++)
      {
        resultLow.Append(ValidAbcLow[rand.Next(ValidAbcLow.Length)]);
      }

      len = (byte)slLength.Value;

      // Random Grossbuchstaben generieren
      StringBuilder resultCapitals = new StringBuilder();
      for (int i = 0; i < capitals; i++)
      {
        resultCapitals.Append(ValidCapitals[rand.Next(ValidCapitals.Length)]);
      }

      // Random Symbole generieren
      StringBuilder resultSymbols = new StringBuilder();
      for (int i = 0; i < symbols; i++)
      {
        resultSymbols.Append(ValidSymbols[rand.Next(ValidSymbols.Length)]);
      }

      string finalresult = resultDigits.ToString() + resultCapitals.ToString() + resultSymbols.ToString() + resultLow.ToString();

      // Alles geht noch einmal ins Random
      StringBuilder result4 = new StringBuilder();

      for (int i = 0; i < len; i++)
      {
        result4.Append(finalresult[rand.Next(finalresult.Length)]);
      }
      try
      {
        tbxPassword.Text = result4.ToString();
        string password = result4.ToString();

        // Will copy the password
        Clipboard.SetText(result4.ToString());
        lblCopy.Visibility = Visibility.Visible;
      }
      catch
      {
        MessageBox.Show("Bitte nicht zu schnell verschieben!");
      }
    }

    private void ColorSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
      double cnt = slDigits.Value + slCapitals.Value + slSymbols.Value;
      if (cnt > slLength.Value)
      {
        // Falls die Länge auf 50 und der Rest mehr ist
        if (slLength.Value == 50 && cnt > 50)
        {
          MessageBox.Show("Die Anzahl Grossbuchstaben, Symbolen und Zahlen darf nicht die Länge überschreiten! ");
        }
        else
        {
          slLength.Value += (cnt - slLength.Value);
        }
      }
      else
      {
        RandomPasswordGenerator();
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
