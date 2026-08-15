using System.Windows.Controls;

namespace PassMasterSuite.Views;

public partial class HomeView : UserControl
{
    /// <summary>Set by the shell; invoked when a tool card is chosen.</summary>
    public Action<AppView>? OnNavigate { get; set; }

    public HomeView()
    {
        InitializeComponent();
    }

    private void CardGenerator_Click(object sender, System.Windows.RoutedEventArgs e) => OnNavigate?.Invoke(AppView.Generator);
    private void CardChecker_Click(object sender, System.Windows.RoutedEventArgs e) => OnNavigate?.Invoke(AppView.Checker);
    private void CardCracker_Click(object sender, System.Windows.RoutedEventArgs e) => OnNavigate?.Invoke(AppView.Cracker);
}
