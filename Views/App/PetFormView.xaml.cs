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
    private PetDTO _selectedPet;
    public event Action OnPetUpdated;

    public PetFormView(PetDTO selectedPet = null)
    {
        InitializeComponent();
        _selectedPet = selectedPet;
        string jsonString = Preferences.Get("User", string.Empty);
        _User = JsonSerializer.Deserialize<UserDto>(jsonString);
        // Aguarda o carregamento dos Pickers

        LoadPetData();
    }

    private async void LoadPetData()
    {
        await LoadPickersAsync();
        if (_selectedPet != null)
        {
            EntryName.Text = _selectedPet.Name;
            EntryBirthMonth.Text = _selectedPet.BirthMonth.ToString();
            EntryBirthYear.Text = _selectedPet.BirthYear.ToString();
            EntryDescription.Text = _selectedPet.Description;

            // Defina os CheckBoxes de acordo com os dados
            CheckBoxGenderMale.IsChecked = _selectedPet.Gender;
            CheckBoxGenderFemale.IsChecked = !_selectedPet.Gender;
            CheckBoxIsNeuteredYes.IsChecked = _selectedPet.IsNeutered;
            CheckBoxIsNeuteredNo.IsChecked = !_selectedPet.IsNeutered;
            CheckBoxIsVaccinatedYes.IsChecked = _selectedPet.IsVaccinated;
            CheckBoxIsVaccinatedNo.IsChecked = !_selectedPet.IsVaccinated;
            CheckBoxAdoptionStatusYes.IsChecked = _selectedPet.AdoptionStatus;
            CheckBoxAdoptionStatusNo.IsChecked = !_selectedPet.AdoptionStatus;

            PickerCharacteristicId.SelectedItem = (
                PickerCharacteristicId.ItemsSource as List<CharacteristicDTO>
            )?.FirstOrDefault(item => item.Id == _selectedPet.CharacteristicId);
            PickerBreedId.SelectedItem = (
                PickerBreedId.ItemsSource as List<BreedDTO>
            )?.FirstOrDefault(item => item.Id == _selectedPet.BreedId);
            PickerSizeId.SelectedItem = (PickerSizeId.ItemsSource as List<SizeDTO>)?.FirstOrDefault(
                item => item.Id == _selectedPet.SizeId
            );

            await InitializePetImages();
            SubmitButton.Text = "Salvar"; // Mudar o texto do botão para "Salvar"
        }
    }

    private async Task InitializePetImages()
    {
        // Lista de Image views para preencher
        var imageViews = new List<Image>
        {
            SelectedImage1,
            SelectedImage2,
            SelectedImage3,
            SelectedImage4,
            SelectedImage5,
            SelectedImage6
        };

        int currentIndex = 0;
        foreach (var petImage in _selectedPet.PetImages)
        {
            if (currentIndex >= imageViews.Count)
                break; // Evita estouro de índice

            var imageStream = await DownloadImageStreamAsync(petImage.Image64);
            MemoryStream memoryStream = new MemoryStream();
            await imageStream.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            ImageStreams.Add(memoryStream); // Adicionando o stream para uso posterior no PUT

            var imageViewToUpdate = imageViews[currentIndex];
            imageViewToUpdate.Source = ImageSource.FromStream(
                () => new MemoryStream(memoryStream.ToArray())
            );

            currentIndex++;
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
                return await response.Content.ReadAsStreamAsync();
            }
            else
            {
                throw new Exception("Não foi possível baixar a imagem.");
            }
        }
        finally
        {
            httpClient.Dispose();
        }
    }

    private async Task LoadPickersAsync()
    {
        await LoadPickersAsyn2();
    }

    private async Task LoadPickersAsyn2()
    {
        PickerCharacteristicId.ItemsSource = await _api.GetAsync<List<CharacteristicDTO>>(
            "Characteristics"
        );
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

                int index = int.Parse(imageIndex) - 1; // Convertendo imageIndex para int e ajustando para base zero
                if (index < ImageStreams.Count)
                {
                    ImageStreams[index] = memoryStream; // Substitui o stream existente
                }
                else
                {
                    ImageStreams.Add(memoryStream); // Adiciona um novo stream se for uma nova imagem
                }

                // Determine which image to set based on the imageIndex
                Image imageToSet = FindImageToSet(imageIndex);
                if (imageToSet != null)
                {
                    imageToSet.IsVisible = true;
                    imageToSet.Source = ImageSource.FromStream(
                        () => new MemoryStream(memoryStream.ToArray())
                    );
                }
            }
        }
    }

    private Image FindImageToSet(string imageIndex)
    {
        // Assuming imageIndex is a string like "1", "2", etc.
        switch (imageIndex)
        {
            case "1":
                return SelectedImage1;
            case "2":
                return SelectedImage2;
            case "3":
                return SelectedImage3;
            case "4":
                return SelectedImage4;
            case "5":
                return SelectedImage5;
            case "6":
                return SelectedImage6;
            default:
                return null;
        }
    }

    private async Task SubmitPetAsync()
    {
        var popup = new SpinnerPopup();
        this.ShowPopup(popup);

        // Obtenha os IDs dos itens selecionados nos Pickers
        var characteristicId = (
            PickerCharacteristicId.SelectedItem as CharacteristicDTO
        )?.Id.ToString();
        var breedId = (PickerBreedId.SelectedItem as BreedDTO)?.Id.ToString();
        var sizeId = (PickerSizeId.SelectedItem as SizeDTO)?.Id.ToString();

        var formData = new Dictionary<string, string>
        {
            { "Name", EntryName.Text },
            { "BirthMonth", EntryBirthMonth.Text },
            { "BirthYear", EntryBirthYear.Text },
            { "Gender", CheckBoxGenderMale.IsChecked ? "True" : "False" },
            { "Description", EntryDescription.Text },
            { "IsNeutered", CheckBoxIsNeuteredYes.IsChecked.ToString() },
            { "IsVaccinated", CheckBoxIsVaccinatedYes.IsChecked.ToString() },
            { "AdoptionStatus", CheckBoxAdoptionStatusYes.IsChecked.ToString() },
            { "CharacteristicId", characteristicId },
            { "BreedId", breedId },
            { "SizeId", sizeId },
            { "UserId", _User.Id.ToString() }
        };

        try
        {
            if (_selectedPet == null)
            {
                // Cria um novo pet
                var response = await _api.PostFormDataAsync<UserDto>(
                    "Pets",
                    formData,
                    ImageStreams
                );
                if (response != null)
                {
                    OnPetUpdated?.Invoke();
                    await DisplayAlert("Sucesso", "Pet criado com sucesso", "OK");
                    
                }
            }
            else
            {
                // Atualiza o pet existente
                // Adicione o ID do pet ao formData, se necessário
                formData.Add("Id", _selectedPet.Id.ToString());
                var response = await _api.PutFormDataAsync<UserDto>(
                    $"Pets/{_selectedPet.Id}",
                    formData,
                    ImageStreams
                );
                // Trate a resposta conforme necessário
                if (response != null)
                {
                    OnPetUpdated?.Invoke();
                    await DisplayAlert("Sucesso", "Pet atualizado com sucesso", "OK");
                    
                }
            }
        }
        catch (Exception ex)
        {
            
        }
        finally
        {
            await Navigation.PopModalAsync();
            popup.Close();
        }
    }

    private void OnCheckBoxCheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        var checkBox = sender as CheckBox;

        if (checkBox == null || !e.Value)
            return;

        // Identifica o grupo do CheckBox baseado no Name
        string groupKey = checkBox.ClassId; // Você pode definir ClassId para cada CheckBox no XAML para identificar seu grupo

        // Obtém todos os CheckBox no mesmo grupo
        var checkBoxesInGroup = checkBox.Parent
            .FindByName<StackLayout>(groupKey)
            .Children.OfType<CheckBox>();

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
        if (!e.Value)
            return;

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
