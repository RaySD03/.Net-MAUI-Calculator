namespace MauiCalculator;

public partial class HistoryPage : ContentPage
{
	public HistoryPage()
	{
		InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);
        HistoryList.ItemsSource = MainPage.history;
    }
    private async void resize(object sender, EventArgs e)
    {
        if (Window.Width < 680)
        {
            HistoryList.WidthRequest = Window.Width * 0.90;
        }
        else 
        {
            await Navigation.PopAsync();
        }
    }
    private async void ClearHistory(object sender, EventArgs e)
    {
        MainPage.history.Clear();
    }
    private async void returnHome(object sender, EventArgs e)
	{
        HistoryList.ItemsSource = null;
        await Navigation.PopAsync();
    }
}