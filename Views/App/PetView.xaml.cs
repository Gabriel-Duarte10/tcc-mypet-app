namespace tcc_mypet_app.Views.App;

public partial class PetView : ContentPage
{
	public PetView()
	{
		InitializeComponent();
	}
    private async void OnPetFormViewClicked(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new PetFormView());
    }
}