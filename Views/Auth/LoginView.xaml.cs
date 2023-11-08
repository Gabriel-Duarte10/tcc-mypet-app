using CommunityToolkit.Maui.Views;
using System.Text.Json;
using tcc_mypet_app.Models.Dto;
using tcc_mypet_app.Models.Request;
using tcc_mypet_app.Services;
using tcc_mypet_app.Views.App;

namespace tcc_mypet_app.Views.Auth;

public partial class LoginView : ContentPage
{
    private readonly ApiService Api = new ApiService();
    public LoginView()
	{
		InitializeComponent();
	}
    private async void OnLoginButtonClicked(object sender, EventArgs e)
    {
        var popup = new SpinnerPopup();
        this.ShowPopup(popup);
        var loginRequest = new LoginRequest
        {
            Login = EntryLogin.Text,
            Password = EntrySenha.Text
        };
        var token = await Api.PostAsync<LoginRequest, LoginDto>("Auth/LoginUser", loginRequest);
        if (token != null)
        {
            Preferences.Default.Set("Token", token.Token);
            var nameId = TokenService.GetClaimValue(token.Token, "nameid");
            var userDto = await Api.GetAsync<UserDto>($"User/{nameId}");
            popup.Close();  // Feche o popup aqui, antes de mudar a página principal.
            if (userDto != null)
            {
                string jsonString = JsonSerializer.Serialize(userDto);
                Preferences.Default.Set("User", jsonString);
                Application.Current.MainPage = new NavigationPage(new AppView());
            }
        }
        else
        {
            popup.Close();
        } 
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