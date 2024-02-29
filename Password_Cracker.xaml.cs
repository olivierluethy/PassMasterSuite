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
  /// Interaktionslogik für Password_Cracker.xaml
  /// </summary>
  public partial class Password_Cracker : Window
  {
    public Password_Cracker()
    {
      InitializeComponent();
    }

    public void Cracker(string[] characters)
    {
      btnSeeCrack.Visibility = Visibility.Hidden;

      listPossible.Items.Clear();

      // Das eingegebene Passwort
      string password = tbxEntPassword.Text.ToString();

      // Anzahl Versuche
      string attempt = "";

      int first = 0, second = 0, third = 0, fourth = 0, fifth = 0, sixth = 0, seventh = 0, eigth = 0, cracks = 0;

      while (!attempt.Equals(password))
      {
        if (first == characters.Length)
        {
          second++;
          first = 0;
        }
        if (second == characters.Length)
        {
          third++;
          second = 0;
        }
        if (third == characters.Length)
        {
          fourth++;
          third = 0;
        }
        if (fourth == characters.Length)
        {
          fifth++;
          fourth = 0;
        }
        if (fifth == characters.Length)
        {
          sixth++;
          fifth = 0;
        }
        if (sixth == characters.Length)
        {
          seventh++;
          sixth = 0;
        }
        if (seventh == characters.Length)
        {
          eigth++;
          seventh = 0;
        }
        if (eigth == characters.Length)
        {
          break;
        }

        attempt = characters[eigth] + characters[seventh] + characters[sixth] + characters[fifth] + characters[fourth] + characters[third] + characters[second] + characters[first];

        if (attempt == password)
        {
          listPossible.Items.Add("Final Crack: " + attempt.ToString());
        }else
        {
          listPossible.Items.Add(attempt.ToString());
        }
        first++;
        cracks++;
      }
      lblAttemps.Content = "Attemps to crack: " + cracks;
      btnSeeCrack.Visibility = Visibility.Visible;
    }

    public void CheckInput(string password)
    {
      // Only Numbers
      if (Regex.IsMatch(password, @"^[0-9]+$", RegexOptions.ECMAScript))
      {
        string[] characters = new string[] { "", "1","2","3","4","5",
        "6","7","8","9","0"};

        Cracker(characters);
      }

      // Only small letters
      else if (Regex.IsMatch(password, @"^[a-z]+$", RegexOptions.ECMAScript))
      {
        string[] characters = new string[] { "","a", "b", "c", "d", "e", "f", "g", "h", "i", "j",
        "k", "l", "m", "n", "o", "p", "q", "r", "s", "t",
        "u", "v", "w", "x", "y", "z"};

        Cracker(characters);
      }

      // Only big letters
      else if (Regex.IsMatch(password, @"^[A-Z]+$", RegexOptions.ECMAScript))
      {
        string[] characters = new string[] { "", "A","B","C","D","E",
        "F","G","H","I","J","K","L","M","N","O","P","Q","R",
        "S","T","U","V","W","X","Y","Z"};

        Cracker(characters);
      }

      // Only letters for big and small
      else if (Regex.IsMatch(password, @"^[a-zA-Z]+$", RegexOptions.ECMAScript))
      {
        string[] characters = new string[] { "","a", "b", "c", "d", "e", "f", "g", "h", "i", "j",
        "k", "l", "m", "n", "o", "p", "q", "r", "s", "t",
        "u", "v", "w", "x", "y", "z","A","B","C","D","E",
        "F","G","H","I","J","K","L","M","N","O","P","Q","R",
        "S","T","U","V","W","X","Y","Z"};

        Cracker(characters);
      }

      // Only symbols
      else if (Regex.IsMatch(password, @"^\W+$", RegexOptions.ECMAScript))
      {
        string[] characters = new string[] { "", "!", "@", "#", "$", "%", "^", "&", "*", "?", "_",
          "~", "-", "£", "(", ")" };

        Cracker(characters);
      }

      // Only small letters and numbers
      else if (Regex.IsMatch(password, @"^[a-z0-9]+$", RegexOptions.ECMAScript))
      {
        string[] characters = new string[] { "", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j",
        "k", "l", "m", "n", "o", "p", "q", "r", "s", "t",
        "u", "v", "w", "x", "y", "z","1","2","3","4","5",
        "6","7","8","9","0"};

        Cracker(characters);
      }

      // Only big letters and numbers
      else if (Regex.IsMatch(password, @"^[A-Z0-9]+$", RegexOptions.ECMAScript))
      {
        string[] characters = new string[] { "", "A","B","C","D","E",
        "F","G","H","I","J","K","L","M","N","O","P","Q","R",
        "S","T","U","V","W","X","Y","Z", "1","2","3","4","5",
        "6","7","8","9","0"};

        Cracker(characters);
      }

      // Only big and small letters with numbers
      else if (Regex.IsMatch(password, @"^[a-zA-Z0-9]+$", RegexOptions.ECMAScript))
      {
        string[] characters = new string[] { "", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j",
        "k", "l", "m", "n", "o", "p", "q", "r", "s", "t",
        "u", "v", "w", "x", "y", "z","A","B","C","D","E",
        "F","G","H","I","J","K","L","M","N","O","P","Q","R",
        "S","T","U","V","W","X","Y","Z","1","2","3","4","5",
        "6","7","8","9","0"};

        Cracker(characters);
      }

      // Only big letters with symbols
      else if (Regex.IsMatch(password, @"^[-!@$%^&£^#*()_+|~=`{}\[\]:;'<>?,./ A-Z]+$", RegexOptions.ECMAScript))
      {
        string[] characters = new string[] { "", "A","B","C","D","E",
        "F","G","H","I","J","K","L","M","N","O","P","Q","R",
        "S","T","U","V","W","X","Y","Z", "-", "!", "@", "$", "%", "^", "&", "£", "^", "#", "*", "(",
        ")", "_", "+", "|", "~", "=", "`", "{", "}", "[", "]", ":", ";", "'", "<", ">",
          "?", ",", ".", "/"};

        Cracker(characters);
      }

      // Only small letters with symbols
      else if (Regex.IsMatch(password, @"^[-!@$%^&£^#*()_+|~=`{}\[\]:;'<>?,./ a-z]+$", RegexOptions.ECMAScript))
      {
        string[] characters = new string[] { "", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j",
        "k", "l", "m", "n", "o", "p", "q", "r", "s", "t",
        "u", "v", "w", "x", "y", "z", "-", "!", "@", "$", "%", "^", "&", "£", "^", "#", "*", "(",
        ")", "_", "+", "|", "~", "=", "`", "{", "}", "[", "]", ":", ";", "'", "<", ">",
          "?", ",", ".", "/"};

        Cracker(characters);
      }

      // Falls alles enthalten
      else
      {
        string[] characters = new string[] { "", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j",
        "k", "l", "m", "n", "o", "p", "q", "r", "s", "t",
        "u", "v", "w", "x", "y", "z","A","B","C","D","E",
        "F","G","H","I","J","K","L","M","N","O","P","Q","R",
        "S","T","U","V","W","X","Y","Z","1","2","3","4","5",
        "6","7","8","9","0", "!", "@", "#", "$", "%", "^", "&", "*", "?", "_",
          "~", "-", "£", "(", ")"};

        Cracker(characters);
      }
    }

    private void btnSeeCrack_Click(object sender, RoutedEventArgs e)
    {
      listPossible.Visibility = Visibility.Visible;
      btnSeeCrack.Visibility = Visibility.Hidden;
      btnHideCrack.Visibility = Visibility.Visible;
    }

    private void btnHideCrack_Click(object sender, RoutedEventArgs e)
    {
      listPossible.Visibility = Visibility.Hidden;
      btnSeeCrack.Visibility = Visibility.Visible;
      btnHideCrack.Visibility = Visibility.Hidden;
    }

    // Main section
    private void btnStart_Click(object sender, RoutedEventArgs e)
    {
      string password = tbxEntPassword.Text.ToString();
      int length = tbxEntPassword.Text.Length;

      if (length >= 4)
      {
        MessageBox.Show("Sie haben vier oder mehr Zeichen eingegeben! Das heisst, " +
          "das es länger dauert, bis das Programm das Password gecrackt hat. Daher kann es sein, " +
          "dass bei zu wenig Leistung ihres Computers der Computer abstürzen kann!");

        if (MessageBox.Show("Wollen sie fortfahren?", "", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
        {

        }
        else
        {
          CheckInput(password);
        }
      }else if(length == 0)
      {
        MessageBox.Show("Bitte geben sie ein Passwort ein befor sie mit dem Crackingvorgang starten");
      }
      else
      {
        CheckInput(password);
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
