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

        public int CheckStrength(string password)
        {
            int score = 0;

            // Check password length
            if (password.Length < 1)
                score = 0;
            else if (password.Length < 4)
                score++;
            else if (password.Length >= 6)
                score++;

            // Check if password contains more than 8 characters
            CheckEight.IsChecked = (password.Length >= 8);

            // Check if password contains more than 12 characters
            if (password.Length >= 12)
                score++;

            // Check if password contains at least one number
            if (Regex.IsMatch(password, @"\d", RegexOptions.ECMAScript))
            {
                score++;
                CheckOneNumber.IsChecked = true;
            }
            else
            {
                CheckOneNumber.IsChecked = false;
            }

            // Check if password contains both lowercase and uppercase letters
            if (Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z]).+$", RegexOptions.ECMAScript))
                score++;

            // Check if password contains special characters
            if (Regex.IsMatch(password, @"[!,@,#,$,%,^,&,*,?,_,~,-,£,(,)]", RegexOptions.ECMAScript))
            {
                CheckOSC.IsChecked = true;
                score++;
            }
            else
            {
                CheckOSC.IsChecked = false;
            }

            // Check if password contains at least one uppercase letter
            CheckUpper.IsChecked = Regex.IsMatch(password, @"[A-Z]", RegexOptions.ECMAScript);

            // Check if password contains at least one lowercase letter
            CheckLow.IsChecked = Regex.IsMatch(password, @"[a-z]", RegexOptions.ECMAScript);

            return score;
        }


        private void tbxPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            // Set visibility of buttons
            btnShow.Visibility = Visibility.Visible;
            btnHide.Visibility = Visibility.Hidden;

            // Get password input and strength
            string passwordInput = tbxPassword.Password.ToString();
            int passwordStrength = CheckStrength(passwordInput);

            // Determine password strength message
            string strengthMessage = GetPasswordStrengthMessage(passwordStrength);

            // Update strength label content
            lblStrength.Content = strengthMessage;
        }

        private void tbxPasswordText_TextChanged(object sender, TextChangedEventArgs e)
        {
            btnShow.Visibility = Visibility.Hidden;
            btnHide.Visibility = Visibility.Visible;

            string passwordInput = tbxPasswordText.Text.Trim(); // Trim whitespace characters
            int passwordStrength = CheckStrength(passwordInput);

            string strengthValue = GetPasswordStrengthMessage(passwordStrength);

            lblStrength.Content = strengthValue.ToString(); // Convert integer to string for label content
        }

        private string GetPasswordStrengthMessage(int strength)
        {
            switch (strength)
            {
                case 0:
                    return "The password is blank!";
                case 1:
                    return "The password is very weak!";
                case 2:
                    return "The password is weak!";
                case 3:
                    return "The password is medium!";
                case 4:
                    return "The password is strong!";
                case 5:
                    return "The password is very strong!";
                default:
                    return "Invalid password strength!";
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
    }
}