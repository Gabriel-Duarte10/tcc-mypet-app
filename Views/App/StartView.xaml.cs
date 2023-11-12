using CommunityToolkit.Maui.Views;
using System.Text.Json;
using tcc_mypet_app.Models.Dto;
using tcc_mypet_app.Services;

namespace tcc_mypet_app.Views.App;

public partial class StartView : ContentPage
{
    private readonly ApiService _api = new ApiService();

    public StartView()
    {
        InitializeComponent();
        LoadDataAsync(); // Adicionar esta linha
    }

    private async Task LoadDataAsync()
    {
        var popup = new SpinnerPopup();
        this.ShowPopup(popup);
        string jsonString = Preferences.Get("User", string.Empty);
        var user = JsonSerializer.Deserialize<UserDto>(jsonString);

        var pets = await _api.GetAsync<List<PetDTO>>($"Pets/not-user/{user.Id}");
        // Limpar se houver dados
        
        // Definir a fonte de dados para a CollectionView
        PetsCollectionView.ItemsSource = pets;

        //deley de 2 segundos para aparecer a tela
        await Task.Delay(2000);
        popup.Close();
    }

    private async void OnPetFormViewClicked(object sender, EventArgs e)
    {
        var petFormView = new PetFormView();
        await Navigation.PushModalAsync(petFormView);

    }

    private async void OnFrameTapped(object sender, EventArgs e)
    {
        if (sender is Frame frame && frame.BindingContext is PetDTO selectedPet)
        {
            var petViewComplete = new PetViewComplete(selectedPet);
            await Navigation.PushModalAsync(petViewComplete);
        }
    }


}