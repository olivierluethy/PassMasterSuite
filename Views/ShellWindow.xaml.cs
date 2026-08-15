using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace PassMasterSuite.Views;

public partial class ShellWindow : Window
{
    private readonly HomeView _home;
    private readonly GeneratorView _generator;
    private readonly CheckerView _checker;
    private readonly CrackerView _cracker;

    public ShellWindow()
    {
        InitializeComponent();

        _home = new HomeView { OnNavigate = GoTo };
        _generator = new GeneratorView();
        _checker = new CheckerView();
        _cracker = new CrackerView();

        Navigate(AppView.Home);
    }

    /// <summary>Selects a nav item programmatically (used by the Home cards).</summary>
    public void GoTo(AppView view)
    {
        RadioButton target = view switch
        {
            AppView.Generator => NavGenerator,
            AppView.Checker => NavChecker,
            AppView.Cracker => NavCracker,
            _ => NavHome,
        };
        target.IsChecked = true; // fires Nav_Checked → Navigate
    }

    private void Nav_Checked(object sender, RoutedEventArgs e)
    {
        if (sender is not RadioButton rb || rb.Tag is not string tag) return;
        if (Host is null) return; // guard during initial parse
        if (Enum.TryParse(tag, out AppView view))
            Navigate(view);
    }

    private void Navigate(AppView view)
    {
        Host.Content = view switch
        {
            AppView.Generator => _generator,
            AppView.Checker => _checker,
            AppView.Cracker => _cracker,
            _ => _home,
        };
        PlayTransition();
    }

    private void PlayTransition()
    {
        var slide = new TranslateTransform(0, 14);
        Host.RenderTransform = slide;

        Host.BeginAnimation(OpacityProperty,
            new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(200)));

        slide.BeginAnimation(TranslateTransform.YProperty,
            new DoubleAnimation(14, 0, TimeSpan.FromMilliseconds(220))
            {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
            });
    }
}
