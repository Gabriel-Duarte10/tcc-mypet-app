namespace tcc_mypet_app.Views.Auth;

public partial class LoginView : ContentPage
{
	public LoginView()
	{
		InitializeComponent();
	}
    private async void OnCreateButtonClicked(object sender, EventArgs e)
    {
        // Navega para a página LoginView
        await Navigation.PushModalAsync(new CreateUserView());
    }
    private async void OnForgetPasswordButtonClicked(object sender, EventArgs e)
    {
        // Navega para a página LoginView
        await Navigation.PushModalAsync(new ForgotPasswordView());
    }
}