using CommunityToolkit.Maui.Views;
using System.Text.Json;
using tcc_mypet_app.Models.Dto;
using tcc_mypet_app.Services;

namespace tcc_mypet_app.Views.App;

public partial class ChatView : ContentPage
{
    private readonly ApiService _api = new ApiService();
    public UserDto _User;
    public ChatView()
	{
		InitializeComponent();
        LoadDataAsync();
    }
    private async Task LoadDataAsync()
    {
        var popup = new SpinnerPopup();
        this.ShowPopup(popup);
        string jsonString = Preferences.Get("User", string.Empty);
        _User = JsonSerializer.Deserialize<UserDto>(jsonString);

        var chats = await _api.GetAsync<List<UserPetChatSessionDTO>>($"ListSessions/{_User.Id}");

        if(chats.Count == 0)
        {
            StackLayoutNull.IsVisible = true;
        }
        else
        {
            StackLayoutNull.IsVisible = false;
        }

        ChatsCollectionView.ItemsSource = chats;


        popup.Close();
    }
    private async void OnPetFormViewClicked(object sender, EventArgs e)
    {
        var petFormView = new PetFormView();
        await Navigation.PushModalAsync(petFormView);
    }
    private async void OnChatDetailViewClicked(object sender, EventArgs e)
    {
        if (sender is VerticalStackLayout frame && frame.BindingContext is UserPetChatSessionDTO UserPetChat)
        {
            var chatDetailPage = new ChatDetailPage(UserPetChat);

            await Navigation.PushModalAsync(chatDetailPage);
          
        }
    }
    private async void OnFrameTapped(object sender, EventArgs e)
    {
        if (sender is Frame frame && frame.BindingContext is UserPetChatSessionDTO UserPetChat)
        {
            var petViewComplete = new PetViewComplete(UserPetChat.Pet);

            await Navigation.PushModalAsync(petViewComplete);

        }
    }
}