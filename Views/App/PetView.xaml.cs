using CommunityToolkit.Maui.Views;
using System.Text.Json;
using tcc_mypet_app.Models.Dto;
using tcc_mypet_app.Services;

namespace tcc_mypet_app.Views.App;

public partial class PetView : ContentPage
{
    private readonly ApiService _api = new ApiService();

    public PetView()
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

        var pets = await _api.GetAsync<List<PetDTO>>($"Pets/user/{user.Id}");
        var petsReturn = CheckIfPetIsReports(pets);
        // Definir a fonte de dados para a CollectionView 
        if (petsReturn.Count == 0)
        {
            LabelListNull.IsVisible = true;
        }
        else
        {
            LabelListNull.IsVisible = false;
        }
        //limpar a collection view
        PetsCollectionView.ItemsSource = null;
        PetsCollectionView.ItemsSource = petsReturn;
        popup.Close();
    }
    private List<PetDTO> CheckIfPetIsReports(List<PetDTO> petDTOs)
    {
        var list = Preferences.Get("ListReport", string.Empty);
        var listDtoReturn = new List<PetDTO>();
        if (!string.IsNullOrEmpty(list))
        {
            foreach (var item in petDTOs)
            {
                if(!list.Contains(item.Id.ToString()))
                {
                    listDtoReturn.Add(item);
                }
            }
        }else
        {
            listDtoReturn = petDTOs;
        }
        return listDtoReturn;
    }

    private async void OnPetFormViewClicked(object sender, EventArgs e)
    {
        var petFormView = new PetFormView();
        // Inscreva-se no evento
        petFormView.OnPetUpdated += PetFormView_OnPetUpdated;
        await Navigation.PushModalAsync(petFormView);

        // Desinscrever-se do evento após o fechamento do PetFormView
        petFormView.Disappearing += (sender, e) =>
        {
            petFormView.OnPetUpdated -= PetFormView_OnPetUpdated;
        };
    }

    private async void PetFormView_OnPetUpdated()
    {
        // Atualize a lista de pets
        await LoadDataAsync();
    }
    private async void OnFrameTapped(object sender, EventArgs e)
    {
        if (sender is Frame frame && frame.BindingContext is PetDTO selectedPet)
        {
            var petFormView = new PetFormView(selectedPet);

            // Inscreva-se no evento OnPetUpdated
            petFormView.OnPetUpdated += PetFormView_OnPetUpdated;

            await Navigation.PushModalAsync(petFormView);

            // Desinscrever-se do evento após o fechamento do PetFormView
            petFormView.Disappearing += (sender, e) =>
            {
                petFormView.OnPetUpdated -= PetFormView_OnPetUpdated;
            };
        }
    }

}