using tcc_mypet_app.Models.Dto;
using System.Text.Json;
using tcc_mypet_app.Services;
using CommunityToolkit.Maui.Views;
using tcc_mypet_app.Views.Auth;

namespace tcc_mypet_app.Views.App;

public partial class ProfileView : ContentPage
{
    public UserDto _User;
    public ProfileView()
	{
		InitializeComponent();
        LoadDataAsync();

    }
    private async Task LoadDataAsync()
    {
        string jsonString = Preferences.Get("User", string.Empty);
        _User = JsonSerializer.Deserialize<UserDto>(jsonString);

        ImageUser.Source = _User.UserImages[0].Image64;
        LabelName.Text = _User.Name;
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Chama a função para carregar os dados
        LoadDataAsync();
    }
    private async void OnEditProfileClicked(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new EditProfileView());
    }
    private async void OnPetViewClicked(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new PetView());
    }
    private async void OnFavoritePetViewClicked(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new FavoriteView());
    }
    private void OnLogoutClicked(object sender, EventArgs e)
    {
        var popup = new SpinnerPopup();
        this.ShowPopup(popup);

        Preferences.Default.Remove("Token");
        Preferences.Default.Remove("User");
        Application.Current.MainPage = new NavigationPage(new LoginView());

        popup.Close();
    }
}