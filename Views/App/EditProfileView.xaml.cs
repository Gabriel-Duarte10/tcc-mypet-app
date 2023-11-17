using CommunityToolkit.Maui.Views;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Maui;
using System.Windows.Input;
using tcc_mypet_app.Models.Dto;
using tcc_mypet_app.Models.Request;
using tcc_mypet_app.Services;
using static tcc_mypet_app.Services.ViaCEPService;
using System.Text.Json;
using System.Diagnostics;

namespace tcc_mypet_app.Views.App;

public partial class EditProfileView : ContentPage
{
    // Se estiver usando MVVM, você pode mover este comando para o seu ViewModel
    private readonly ApiService Api = new ApiService();
    private readonly ViaCEPService viaCEPService = new ViaCEPService();

    private List<Stream> ImageStreams = new List<Stream>();
    public UserDto _User;
    public EditProfileView()
	{
		InitializeComponent();
        string jsonString = Preferences.Get("User", string.Empty);
        _User = JsonSerializer.Deserialize<UserDto>(jsonString);

        EntryNome.Text = _User.Name;
        EntryCellphone.Text = _User.Cellphone;
        EntryZipCode.Text = _User.ZipCode;
        EntryStreet.Text = _User.Street;
        EntryNumber.Text = _User.Number.ToString();
        EntryState.Text = _User.State;
        EntryCity.Text = _User.City;
        InitializeProfileImage();
    }
    public async Task InitializeProfileImage()
    {
        try
        {
            var profileImageStream = await DownloadImageStreamAsync(_User.UserImages[0].Image64);

            using (Stream originalStream = profileImageStream)
            {
                MemoryStream memoryStream = new MemoryStream();
                await originalStream.CopyToAsync(memoryStream);
                memoryStream.Position = 0;
                ImageStreams.Add(memoryStream); 
                SelectedImage.Source = ImageSource.FromStream(() => new MemoryStream(memoryStream.ToArray()));
            }
        }
        catch (Exception ex)
        {
            // Handle any errors here
            Debug.WriteLine($"Erro ao baixar a imagem de perfil: {ex.Message}");
        }
    }
    private async Task<Stream> DownloadImageStreamAsync(string imageUrl)
    {
        var httpClient = new HttpClient();
        try
        {
            var response = await httpClient.GetAsync(imageUrl);
            if (response.IsSuccessStatusCode)
            {
                // O Stream será fechado e descartado mais tarde, não aqui
                return await response.Content.ReadAsStreamAsync();
            }
            else
            {
                throw new Exception("Não foi possível baixar a imagem.");
            }
        }
        finally
        {
            // Libere os recursos do HttpClient manualmente
            httpClient.Dispose();
        }
    }
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        foreach (var stream in ImageStreams)
        {
            stream?.Close();
            stream?.Dispose();
        }
        ImageStreams.Clear();
    }


    // Método chamado quando o botão é clicado (se estiver usando código-behind)
    // Método chamado quando o botão é clicado (se estiver usando código-behind)
    private async void OnAddImageButtonClicked(object sender, EventArgs e)
    {
        var action = await DisplayActionSheet(
            "Adicionar Imagem",
            "Cancelar",
            null,
            "Tirar Foto",
            "Escolher da Galeria"
        );
        switch (action)
        {
            case "Tirar Foto":
                await CaptureAndDisplayPhoto();
                break;
            case "Escolher da Galeria":
                await PickAndDisplayPhoto();
                break;
        }
    }

    private async Task CaptureAndDisplayPhoto()
    {
        if (MediaPicker.Default.IsCaptureSupported)
        {
            var photo = await MediaPicker.Default.CapturePhotoAsync();
            await DisplayPhoto(photo);
        }
    }

    private async Task PickAndDisplayPhoto()
    {
        var photo = await MediaPicker.Default.PickPhotoAsync();
        await DisplayPhoto(photo);
    }

    private async Task DisplayPhoto(FileResult photo)
    {
        if (photo != null)
        {
            using (Stream originalStream = await photo.OpenReadAsync())
            {
                MemoryStream memoryStream = new MemoryStream();
                await originalStream.CopyToAsync(memoryStream);
                memoryStream.Position = 0; // Reset the position after copying
                ImageStreams = new List<Stream>
                {
                    memoryStream // Add the copy to your list
                };
                SelectedImage.IsVisible = true;
                // Set the source to the copy of the stream
                SelectedImage.Source = ImageSource.FromStream(() => new MemoryStream(memoryStream.ToArray()));
            }
        }
    }

    private async void OnZipCodeTextChanged(object sender, TextChangedEventArgs e)
    {
        string cep = e.NewTextValue;
        if (cep.Length == 8) // Verifique se o CEP tem o formato correto (ex: 12345-678)
        {
            try
            {
                Address address = await viaCEPService.GetAddressByCEPAsync(cep);
                EntryStreet.Text = address.Street;
                EntryCity.Text = address.City;
                EntryState.Text = address.UF;
            }
            catch (Exception ex)
            {
                // O CEP não foi encontrado ou outro erro ocorreu
                await DisplayAlert("Erro", ex.Message, "OK");
            }
        }
        else
        {
            EntryStreet.Text = "";
            EntryCity.Text = "";
            EntryState.Text = "";
        }
    }

    private async void OnCreateButtonClicked(object sender, EventArgs e)
    {
        var popup = new SpinnerPopup();
        this.ShowPopup(popup);
        // Criando o dicionário para os campos de formulário
        var formData = new Dictionary<string, string>
        {
            { "Name", EntryNome.Text },
            { "Cellphone", EntryCellphone.Text },
            { "ZipCode", EntryZipCode.Text },
            { "Street", EntryStreet.Text },
            { "Number", EntryNumber.Text },
            { "State", EntryState.Text },
            { "City", EntryCity.Text }
        };

        // Convertendo ImageStreams para List<IFormFile>
        // Convertendo ImageStreams para List<Stream> (já é uma List<Stream>, então não é necessário ToList())
        var images = ImageStreams;


        // Usando o novo método PostFormDataAsync
        try
        {
            var userDto = await Api.PutFormDataAsync<UserDto>("User/" + _User.Id, formData, images);
            if (userDto != null)
            {
                string jsonString = JsonSerializer.Serialize(userDto);
                Preferences.Default.Remove("User");
                Preferences.Default.Set("User", jsonString);
            }

            if (userDto != null)
            {
                await DisplayAlert("Sucesso", "Usuário atualizado com sucesso", "OK");
               
            }
        }
        catch (Exception)
        {
            
            throw;
        }
        finally
        {
            await Navigation.PopModalAsync();
            popup.Close();
        }   
    }
}