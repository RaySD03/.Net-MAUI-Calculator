using MauiCalculator;

namespace MauiCalculator;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
		MainPage = new NavigationPage(new MainPage());
    }
    protected override Window CreateWindow(IActivationState activationState)
    {
        var window = base.CreateWindow(activationState);
        window.Width = 700;
        window.Height = 510;
        return window;
    }
}
