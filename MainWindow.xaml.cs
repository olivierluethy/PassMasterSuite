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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Password_Checker__Generator_and_Cracker
{
  /// <summary>
  /// Interaktionslogik für MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();
    }

    private void btnPSC_Click(object sender, RoutedEventArgs e)
    {
      this.Hide();
      Password_Strength_Checker f1 = new Password_Strength_Checker();
      f1.ShowDialog();
      this.Close();
    }

    private void btnPG_Click(object sender, RoutedEventArgs e)
    {
      this.Hide();
      Password_Generator f1 = new Password_Generator();
      f1.ShowDialog();
      this.Close();
    }

    private void btnPC_Click(object sender, RoutedEventArgs e)
    {
      this.Hide();
      Password_Cracker f1 = new Password_Cracker();
      f1.ShowDialog();
      this.Close();
    }
  }
}
