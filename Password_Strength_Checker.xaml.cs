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
using System.Text.RegularExpressions;

namespace Password_Checker__Generator_and_Cracker
{
  /// <summary>
  /// Interaktionslogik für Password_Strength_Checker.xaml
  /// </summary>
  public partial class Password_Strength_Checker : Window
  {
    public Password_Strength_Checker()
    {
      InitializeComponent();
    }

    public int CheckStrength(string password) // Diese Funktion überprüft das eingegebene Passwort
    {
      int score = 0;

      if (password.Length < 1)
        score = 0;
      if (password.Length < 4)
        score++;
      if (password.Length >= 6)
        score++;

      if (tbxPassword.Password.ToString() == "")
      {
        CheckEight.IsChecked = false;
      }

      // Falls das Passwort mehr als 8 Zeichen beinhaltet, soll ein Häkchen an der richten Stelle gesetzt werden
      if (password.Length >= 8)
      {
        CheckEight.IsChecked = true;
      }
      else
      {
        CheckEight.IsChecked = false;
      }

      if (password.Length >= 12)
        score++;

      // Falls das Passwort Zahlen und Kommastellen beinhaltet //number only //"^\d+$" if you need to match more than one digit.
      if (Regex.IsMatch(password, @"[0-9]+(\.[0-9][0-9]?)?", RegexOptions.ECMAScript))
      {
        score++;
        CheckOneNumber.IsChecked = true;
      }
      else
      {
        CheckOneNumber.IsChecked = false;
      }

      // Falls das Passwort Gross- oder Kleinbuchstaben beinhaltet
      if (Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z]).+$", RegexOptions.ECMAScript)) //both, lower and upper case
        score++;

      // Falls das Passwort diese Sonderzeichen beinhaltet
      if (Regex.IsMatch(password, @"[!,@,#,$,%,^,&,*,?,_,~,-,£,(,)]", RegexOptions.ECMAScript)) // Check if password contains the following characters
      {
        CheckOSC.IsChecked = true;
        score++;
      }
      else
      {
        CheckOSC.IsChecked = false;
      }

      if (Regex.IsMatch(password, @"[A-Z]", RegexOptions.ECMAScript)) // Check if password has one or more uppercase characters
      {
        CheckUpper.IsChecked = true;
      }
      else
      {
        CheckUpper.IsChecked = false;
      }
      if (Regex.IsMatch(password, @"[a-z]", RegexOptions.ECMAScript)) // Check if password has one or more uppercase characters
      {
        CheckLow.IsChecked = true;
      }
      else
      {
        CheckLow.IsChecked = false;
      }

      return score;
    }

    private void tbxPassword_PasswordChanged(object sender, RoutedEventArgs e)
    {
      btnShow.Visibility = Visibility.Visible;
      btnHide.Visibility = Visibility.Hidden;

      string input = "";
      int points = 0;
      input = tbxPassword.Password.ToString();
      points = CheckStrength(input);
      int length = input.Length;

      if (points == 0)
      {
        lblStrength.Content = "The password is blank!";
      }
      else if (points == 1)
      {
        lblStrength.Content = "The password is very weak!";
      }
      else if (points == 2)
      {
        lblStrength.Content = "The password is weak!";
      }
      else if (points == 3)
      {
        lblStrength.Content = "The password is medium!";
      }
      else if (points == 4)
      {
        lblStrength.Content = "The password is strong!";
      }
      else if (points == 5)
      {
        lblStrength.Content = "The password is very strong!";
      }
    }

    private void btnShow_Click(object sender, RoutedEventArgs e)
    {
      tbxPasswordText.Text = tbxPassword.Password;
      tbxPasswordText.CaretIndex = tbxPasswordText.Text.Length;

      tbxPasswordText.Visibility = Visibility.Visible;
      tbxPassword.Visibility = Visibility.Hidden;

      btnShow.Visibility = Visibility.Hidden;
      btnHide.Visibility = Visibility.Visible;

      tbxPasswordText.Focus();
    }

    private void btnHide_Click(object sender, RoutedEventArgs e)
    {
      tbxPassword.Password = tbxPasswordText.Text;

      // set the cursor position to 2...
      SetSelection(tbxPassword, tbxPassword.Password.Length, 0);

      tbxPasswordText.Visibility = Visibility.Hidden;
      tbxPassword.Visibility = Visibility.Visible;

      btnHide.Visibility = Visibility.Hidden;
      btnShow.Visibility = Visibility.Visible;

      tbxPassword.Focus();
    }

    private void SetSelection(PasswordBox passwordBox, int start, int length)
    {
      passwordBox.GetType().GetMethod("Select", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).Invoke(passwordBox, new object[] { start, length });
    }

    private void btnHome_Click(object sender, RoutedEventArgs e)
    {
      this.Hide();
      MainWindow f1 = new MainWindow();
      f1.ShowDialog();
      this.Close();
    }

    private void tbxPasswordText_TextChanged(object sender, TextChangedEventArgs e)
    {
      btnShow.Visibility = Visibility.Hidden;
      btnHide.Visibility = Visibility.Visible;

      string input = "";
      int points = 0;
      input = tbxPasswordText.Text.ToString();
      points = CheckStrength(input);
      int length = input.Length;

      if (points == 0)
      {
        lblStrength.Content = "The password is blank!";
      }
      else if (points == 1)
      {
        lblStrength.Content = "The password is very weak!";
      }
      else if (points == 2)
      {
        lblStrength.Content = "The password is weak!";
      }
      else if (points == 3)
      {
        lblStrength.Content = "The password is medium!";
      }
      else if (points == 4)
      {
        lblStrength.Content = "The password is strong!";
      }
      else if (points == 5)
      {
        lblStrength.Content = "The password is very strong!";
      }
    }
  }
}
