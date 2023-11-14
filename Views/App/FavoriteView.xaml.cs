using CommunityToolkit.Maui.Views;
using System.Text.Json;
using tcc_mypet_app.Models.Dto;
using tcc_mypet_app.Services;

namespace tcc_mypet_app.Views.App;

public partial class FavoriteView : ContentPage
{
    private readonly ApiService _api = new ApiService();

    public FavoriteView()
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

        var pets = await _api.GetAsync<List<PetDTO>>($"Pets/favorites/{user.Id}");
        var petsReturn = CheckIfPetIsReports(pets);
        // Definir a fonte de dados para a CollectionView
        if(petsReturn.Count == 0)
        {
            LabelListNull.IsVisible = true;
        }
        PetsCollectionView.ItemsSource = petsReturn;

        //deley de 2 segundos para aparecer a tela
        await Task.Delay(2000);
        popup.Close();
    }
    private List<PetDTO> CheckIfPetIsReports(List<PetDTO> petDTOs)
    {
        var list = Preferences.Get("ListReport", string.Empty);
        var listDtoReturn = new List<PetDTO>();
        if (!string.IsNullOrEmpty(list))
        {
            var listArray = JsonSerializer.Deserialize<List<int>>(list);
            foreach (var item in petDTOs)
            {
                if (!listArray.Contains(item.Id))
                {
                    listDtoReturn.Add(item);
                }
            }
        }
        else
        {
            return petDTOs;
        }
        return listDtoReturn;
    }
    private async void StartView_OnPetUpdated()
    {
        // Atualize a lista de pets
        await LoadDataAsync();
    }

    private async void OnFrameTapped(object sender, EventArgs e)
    {
        if (sender is Frame frame && frame.BindingContext is PetDTO selectedPet)
        {
            var petViewComplete = new PetViewComplete(selectedPet);

            // Inscreva-se no evento
            petViewComplete.OnPetUpdated += StartView_OnPetUpdated;

            await Navigation.PushModalAsync(petViewComplete);

            // Alteração: Desinscrever-se no evento de desaparecimento
            petViewComplete.Disappearing += async (sender, e) =>
            {
                petViewComplete.OnPetUpdated -= StartView_OnPetUpdated;
                await LoadDataAsync();
            };
        }
    }



}