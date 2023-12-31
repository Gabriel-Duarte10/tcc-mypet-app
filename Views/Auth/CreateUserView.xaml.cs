using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Maui;
using System.Windows.Input;
using tcc_mypet_app.Models.Dto;
using tcc_mypet_app.Models.Request;
using tcc_mypet_app.Services;
using static tcc_mypet_app.Services.ViaCEPService;

using CommunityToolkit.Maui.Views;

namespace tcc_mypet_app.Views.Auth;

public partial class CreateUserView : ContentPage
{
    // Se estiver usando MVVM, voc� pode mover este comando para o seu ViewModel

    private readonly ApiService Api = new ApiService();
    private readonly ViaCEPService viaCEPService = new ViaCEPService();

    private List<Stream> ImageStreams = new List<Stream>();

    public CreateUserView()
    {
        InitializeComponent();
        SelectedImage.IsVisible = false;
    }

    // M�todo chamado quando o bot�o � clicado (se estiver usando c�digo-behind)
    // M�todo chamado quando o bot�o � clicado (se estiver usando c�digo-behind)
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
                SelectedImage.Source = ImageSource.FromStream(
                    () => new MemoryStream(memoryStream.ToArray())
                );
            }
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
                // O CEP n�o foi encontrado ou outro erro ocorreu
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
        try
        {
            // Criando o dicion�rio para os campos de formul�rio
            var formData = new Dictionary<string, string>
            {
                { "Name", EntryNome.Text },
                { "Email", EntryEmail.Text },
                { "Password", EntryPassword.Text },
                { "Cellphone", EntryCellphone.Text },
                { "ZipCode", EntryZipCode.Text },
                { "Street", EntryStreet.Text },
                { "Number", EntryNumber.Text },
                { "State", EntryState.Text },
                { "City", EntryCity.Text },
                { "IsActive", "true" } // supondo que IsActive seja sempre true
            };

            // Convertendo ImageStreams para List<IFormFile>
            // Convertendo ImageStreams para List<Stream> (j� � uma List<Stream>, ent�o n�o � necess�rio ToList())
            var images = ImageStreams;

            // Usando o novo m�todo PostFormDataAsync

            var userDto = await Api.PostFormDataAsync<UserDto>("User", formData, images);

            if (userDto != null)
            {
                await DisplayAlert("Sucesso", "Usu�rio criado com sucesso", "OK");
                popup.Close();
                Navigation.PopModalAsync();
            }
        }
        catch (Exception)
        {
            popup.Close();
            foreach (var stream in ImageStreams)
            {
                stream?.Close();
                stream?.Dispose();
            }
            ImageStreams.Clear();
            ImageStreams = new List<Stream>();
        }
    }
}
