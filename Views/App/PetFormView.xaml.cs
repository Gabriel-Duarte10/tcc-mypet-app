using tcc_mypet_app.Models.Dto;
using tcc_mypet_app.Services;
using System.Text.Json;
using CommunityToolkit.Maui.Views;

namespace tcc_mypet_app.Views.App;

public partial class PetFormView : ContentPage
{
    private readonly ApiService _api = new ApiService();
    private List<Stream> ImageStreams = new List<Stream>();
    public UserDto _User;
    public PetFormView()
	{
		InitializeComponent();
        LoadPickers();
        string jsonString = Preferences.Get("User", string.Empty);
        _User = JsonSerializer.Deserialize<UserDto>(jsonString);
    }
    private async void LoadPickers()
    {
        PickerCharacteristicId.ItemsSource = await _api.GetAsync<List<CharacteristicDTO>>("Characteristics");
        PickerCharacteristicId.ItemDisplayBinding = new Binding("Name");

        PickerBreedId.ItemsSource = await _api.GetAsync<List<BreedDTO>>("Breeds");
        PickerBreedId.ItemDisplayBinding = new Binding("Name");

        PickerSizeId.ItemsSource = await _api.GetAsync<List<SizeDTO>>("Sizes");
        PickerSizeId.ItemDisplayBinding = new Binding("Name");
    }
    private async void OnAddImageButtonClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        if (button != null)
        {
            var imageIndex = button.CommandParameter.ToString();
            var action = await DisplayActionSheet(
                "Adicionar Imagem",
                "Cancelar",
                null,
                "Tirar Foto",
                "Escolher da Galeria"
            );
            // Pass the imageIndex to the methods
            switch (action)
            {
                case "Tirar Foto":
                    await CaptureAndDisplayPhoto(imageIndex);
                    break;
                case "Escolher da Galeria":
                    await PickAndDisplayPhoto(imageIndex);
                    break;
            }
        }
    }
    private async Task CaptureAndDisplayPhoto(string imageIndex)
    {
        if (MediaPicker.Default.IsCaptureSupported)
        {
            var photo = await MediaPicker.Default.CapturePhotoAsync();
            await DisplayPhoto(photo, imageIndex);
        }
    }
    private async Task PickAndDisplayPhoto(string imageIndex)
    {
        var photo = await MediaPicker.Default.PickPhotoAsync();
        await DisplayPhoto(photo, imageIndex);
    }
    private async Task DisplayPhoto(FileResult photo, string imageIndex)
    {
        if (photo != null)
        {
            using (Stream originalStream = await photo.OpenReadAsync())
            {
                MemoryStream memoryStream = new MemoryStream();
                await originalStream.CopyToAsync(memoryStream);
                memoryStream.Position = 0; // Reset the position after copying
                ImageStreams.Add(memoryStream);

                // Determine which image to set based on the imageIndex
                Image imageToSet = FindImageToSet(imageIndex);
                if (imageToSet != null)
                {
                    imageToSet.IsVisible = true;
                    imageToSet.Source = ImageSource.FromStream(() => new MemoryStream(memoryStream.ToArray()));
                }
            }
        }
    }
    private Image FindImageToSet(string imageIndex)
    {
        // Assuming imageIndex is a string like "1", "2", etc.
        switch (imageIndex)
        {
            case "1": return SelectedImage1;
            case "2": return SelectedImage2;
            case "3": return SelectedImage3;
            case "4": return SelectedImage4;
            case "5": return SelectedImage5;
            case "6": return SelectedImage6;
            default: return null;
        }
    }
    private async Task SubmitPetAsync()
    {
        var popup = new SpinnerPopup();
        this.ShowPopup(popup);

        var formData = new Dictionary<string, string>
            {
                {"Name", EntryName.Text},
                {"BirthMonth", EntryBirthMonth.Text},
                {"BirthYear", EntryBirthYear.Text},
                //{"Gender", CheckBoxGenderYes.IsChecked.ToString()},
                {"Description", EntryDescription.Text},
                {"IsNeutered", CheckBoxIsNeuteredYes.IsChecked.ToString()},
                {"IsVaccinated", CheckBoxIsVaccinatedYes.IsChecked.ToString()},
                {"AdoptionStatus", CheckBoxAdoptionStatusYes.IsChecked.ToString()},
                {"CharacteristicId", GetSelectedPickerItemId(PickerCharacteristicId)},
                {"BreedId", GetSelectedPickerItemId(PickerBreedId)},
                {"SizeId", GetSelectedPickerItemId(PickerSizeId)},
                {"UserId", _User.Id.ToString()}
            };

        // Suponha que 'images' é uma coleção de objetos Image que representam as imagens selecionadas pelo usuário
        var images = ImageStreams; // Substitua pela lógica para incluir imagens reais

        // Chame o método da sua API para enviar os dados do formulário e as imagens
        var userDto = await _api.PostFormDataAsync<UserDto>("Pets", formData, images);
        popup.Close();
        if (userDto != null)
        {
            await DisplayAlert("Sucesso", "Pet criado com sucesso", "OK");

            Navigation.PopModalAsync();
        }
        else
        {
            // Trate o erro aqui
        }


    }
    private string GetSelectedPickerItemId(Picker picker)
    {
        var selectedItem = picker.SelectedItem;

        // Verifique se o item selecionado é do tipo esperado
        if (selectedItem is CharacteristicDTO selectedCharacteristic)
        {
            return selectedCharacteristic.Id.ToString();
        }
        else if (selectedItem is BreedDTO selectedBreed)
        {
            return selectedBreed.Id.ToString();
        }
        else if (selectedItem is SizeDTO selectedSize)
        {
            return selectedSize.Id.ToString();
        }

        // Retornar um valor padrão ou lançar uma exceção se o tipo não for o esperado
        return null; // ou você pode lançar uma exceção
    }
    private void OnCheckBoxCheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        var checkBox = sender as CheckBox;

        if (checkBox == null || !e.Value)
            return;

        // Identifica o grupo do CheckBox baseado no Name
        string groupKey = checkBox.ClassId; // Você pode definir ClassId para cada CheckBox no XAML para identificar seu grupo

        // Obtém todos os CheckBox no mesmo grupo
        var checkBoxesInGroup = checkBox.Parent.FindByName<StackLayout>(groupKey).Children.OfType<CheckBox>();

        foreach (var otherCheckBox in checkBoxesInGroup)
        {
            if (otherCheckBox != checkBox)
            {
                otherCheckBox.IsChecked = false;
            }
        }
    }
    private async void OnSubmitButtonClicked(object sender, EventArgs e)
    {
        await SubmitPetAsync();
    }
    private void OnCheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        // Verifica se o evento foi disparado por um CheckBox sendo marcado, não desmarcado
        if (!e.Value) return;

        var checkBoxChanged = (CheckBox)sender;
        var parentLayout = (StackLayout)checkBoxChanged.Parent;
        foreach (var child in parentLayout.Children)
        {
            if (child is CheckBox checkBox && checkBox != checkBoxChanged)
            {
                checkBox.IsChecked = false;
            }
        }
    }
}