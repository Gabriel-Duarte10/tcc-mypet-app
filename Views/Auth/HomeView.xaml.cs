namespace tcc_mypet_app.Views.Auth;

public partial class HomeView : ContentPage
{
    public HomeView()
    {
        InitializeComponent();
    }
    private async void OnStartButtonClicked(object sender, EventArgs e)
    {
        // Navega para a p�gina LoginView
        await Navigation.PushModalAsync(new LoginView());
    }
}
