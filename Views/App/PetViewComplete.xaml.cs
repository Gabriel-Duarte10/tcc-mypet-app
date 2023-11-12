using tcc_mypet_app.Models.Dto;

namespace tcc_mypet_app.Views.App;

public partial class PetViewComplete : ContentPage
{
    public PetViewComplete(PetDTO selectedPet)
    {
        InitializeComponent();
        this.BindingContext = selectedPet;
    }
}