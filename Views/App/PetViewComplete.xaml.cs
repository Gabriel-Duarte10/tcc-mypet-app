
using CommunityToolkit.Maui.Views;
using System.Text.Json;
using tcc_mypet_app.Models.Dto;
using tcc_mypet_app.Models.Request;
using tcc_mypet_app.Services;

namespace tcc_mypet_app.Views.App;

public partial class PetViewComplete : ContentPage
{
    private readonly ApiService _api = new ApiService();
    public readonly int idPet;
    public readonly int idUserPet;
    public event Action OnPetUpdated;
    public PetViewComplete(PetDTO selectedPet = null)
    {
        InitializeComponent();
        this.BindingContext = selectedPet;
        this.idPet = selectedPet.Id;
        this.idUserPet = selectedPet.User.Id;
        // Verifica se o pet está na lista de favoritos
        CheckIfPetIsFavorite();
    }
    private void CheckIfPetIsFavorite()
    {
        var list = Preferences.Get("ListFavorite", string.Empty);
        if(string.IsNullOrEmpty(list))
        {
            return;
        }
        var listArray = JsonSerializer.Deserialize<List<int>>(list);
        if (!string.IsNullOrEmpty(list) && listArray.Contains(this.idPet))
        {
            FavoriteButton.IsVisible = false;
            DesfavoriteButton.IsVisible = true;
        }
    }
    private async void OnFavoriteClicked(object sender, EventArgs e)
    {
        var list = Preferences.Get("ListFavorite", string.Empty);
        var favorites = new List<int>();

        if (!string.IsNullOrEmpty(list))
        {
            favorites = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(list);
        }

        if (!favorites.Contains(this.idPet))
        {
            string jsonString = Preferences.Get("User", string.Empty);
            var user = JsonSerializer.Deserialize<UserDto>(jsonString);
            var favoritePet = new FavoritePetRequest
            {
                UserId = user.Id,
                PetId = this.idPet
            };
            var result = await _api.PostAsync<FavoritePetRequest, FavoritePetDto>("Pets/favorite", favoritePet);
            if (result != null)
            {
                favorites.Add(this.idPet);
                var newList = Newtonsoft.Json.JsonConvert.SerializeObject(favorites);
                Preferences.Set("ListFavorite", newList);
                FavoriteButton.IsVisible = false;
                DesfavoriteButton.IsVisible = true;
            }
        }
    }
    private async void OnReportClicked(object sender, EventArgs e)
    {
        var list = Preferences.Get("ListReport", string.Empty);
        var reports = new List<int>();

        if (!string.IsNullOrEmpty(list))
        {
            reports = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(list);
        }

        if (!reports.Contains(this.idPet))
        {
            var reportPet = new ReportedPetRequest
            {
                PetId = this.idPet
            };
            var result = await _api.PostAsync<ReportedPetRequest, ReportedPetDto>("Pets/report", reportPet);
            if (result != null)
            {
                reports.Add(this.idPet);
                var newList = Newtonsoft.Json.JsonConvert.SerializeObject(reports);
                Preferences.Set("ListReport", newList);
                OnPetUpdated?.Invoke();
                Navigation.PopModalAsync();
            }
        }
    }
    private async void OnDesfavoriteClicked(object sender, EventArgs e)
    {
        var list = Preferences.Get("ListFavorite", string.Empty);
        var favorites = new List<int>();

        if (!string.IsNullOrEmpty(list))
        {
            favorites = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(list);
        }

        if (favorites.Contains(this.idPet))
        {
            string jsonString = Preferences.Get("User", string.Empty);
            var user = JsonSerializer.Deserialize<UserDto>(jsonString);
            var favoritePet = new FavoritePetRequest
            {
                UserId = user.Id,
                PetId = this.idPet
            };
            var result = await _api.PutAsync<FavoritePetRequest, FavoritePetDto>("Pets/favorite", favoritePet);
            
            favorites.Remove(this.idPet);
            var newList = Newtonsoft.Json.JsonConvert.SerializeObject(favorites);
            Preferences.Set("ListFavorite", newList);
            FavoriteButton.IsVisible = true;
            DesfavoriteButton.IsVisible = false;
        }
    }
    private async void OnAbrirChatClicked(object sender, EventArgs e)
    {
        string jsonString = Preferences.Get("User", string.Empty);
        var user = JsonSerializer.Deserialize<UserDto>(jsonString);
        var chatSession = new UserPetChatSessionRequest
        {
            PetId = this.idPet,
            User1Id = user.Id,
            User2Id = this.idUserPet
        };

        var result = await _api.PostAsync<UserPetChatSessionRequest, UserPetChatSessionDTO>("CreateSession", chatSession);
        if(result != null)
        {
            var chatDetailPage = new ChatDetailPage(result);
            await Navigation.PushModalAsync(chatDetailPage);
        }
    }
}