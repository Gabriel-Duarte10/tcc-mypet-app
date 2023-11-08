using CommunityToolkit.Maui.Views;
using System.ComponentModel;
using tcc_mypet_app.Models.Request;
using tcc_mypet_app.Services;

namespace tcc_mypet_app.Views.Auth;

public partial class ForgotPasswordView : ContentPage, INotifyPropertyChanged
{
    private bool _email = true;
    public bool Email
    {
        get => _email;
        set
        {
            _email = value;
            OnPropertyChanged(nameof(Email));
        }
    }
    private bool _Codigo = false;
    public bool Codigo
    {
        get => _Codigo;
        set
        {
            _Codigo = value;
            OnPropertyChanged(nameof(Codigo));
        }
    }
    private bool _Senha = false;
    public bool Senha
    {
        get => _Senha;
        set
        {
            _Senha = value;
            OnPropertyChanged(nameof(Senha));
        }
    }
    private string CodigoValue;
    private readonly ApiService Api = new ApiService();
    public ForgotPasswordView()
    {
        InitializeComponent();
        this.BindingContext = this;
        buttonCodigo.IsEnabled = false;
        buttonCodigo.Opacity = 0.5;
    }

    private async void OnProxButtonClicked(object sender, EventArgs e)
    {
        var popup = new SpinnerPopup();
        this.ShowPopup(popup);
        // Navega para a página LoginView
        if (this.Email == true)
        {
            // Usando o novo método PostFormDataAsync
            var resetPasswordInitiateRequest = new ResetPasswordInitiateRequest
            {
                Email = EntryEmail.Text
            };
            var userDto = await Api.PostAsync<ResetPasswordInitiateRequest, string>("Auth/InitiateResetUser", resetPasswordInitiateRequest);
            popup.Close();
            if (userDto != null)
            {
                PhoneSpan.Text = userDto;
                this.Email = false;
                this.Codigo = true;
            }
        }
        else
        {
            if(this.Codigo == true)
            {
                var codePassoword = new CodePassword()
                {
                    Email = EntryEmail.Text,
                    CellphoneCode = int.Parse(this.CodigoValue)
                };
                var validBool = await Api.PostAsync<CodePassword, bool>("Auth/ValidateCodeUser", codePassoword);
                popup.Close();
                if (validBool == true)
                {
                    this.Codigo = false;
                    this.Senha = true;
                }
             
            }
            else
            {
                var resetPasswordCompleteRequest = new ResetPasswordCompleteRequest
                {
                    Email = EntryEmail.Text,
                    NewPassword = EntrySenha.Text,
                    ConfirmNewPassword = EntryConfirmPassword.Text
                };
                var resetPassowordBool = await Api.PostAsync<ResetPasswordCompleteRequest, bool>("Auth/CompleteResetUser", resetPasswordCompleteRequest);
                popup.Close();
                if (resetPassowordBool == true)
                {
                    await DisplayAlert("Sucesso", "Senha alterada com sucesso", "OK");
                    await Navigation.PopModalAsync();
                }
            }   
        }
        
    }
    
    private void OnCodeEntryTextChanged(object sender, TextChangedEventArgs e)
    {
        Entry entry = sender as Entry;
        if (e.NewTextValue.Length == 1)
        {
            if (entry == CodeEntry6)
            {
                string fullCode =
                    CodeEntry1.Text
                    + CodeEntry2.Text
                    + CodeEntry3.Text
                    + CodeEntry4.Text
                    + CodeEntry5.Text
                    + CodeEntry6.Text;
                if (!string.IsNullOrEmpty(fullCode))
                {
                    this.CodigoValue = fullCode;
                    buttonCodigo.IsEnabled = true;
                    buttonCodigo.Opacity = 1;
                }
            }
            else
            {
                Entry nextEntry = this.FindByName<Entry>(entry.ReturnCommandParameter.ToString());
                if (nextEntry != null)
                    nextEntry.Focus();
            }
        }
        else if (e.OldTextValue.Length == 1 && e.NewTextValue.Length == 0)
        {
            MoveFocusToPreviousEntry(entry);
        }
    }

    private void MoveFocusToPreviousEntry(Entry currentEntry)
    {
        string currentEntryName = currentEntry.StyleId;
        int currentEntryNumber = int.Parse(currentEntryName.Substring(currentEntryName.Length - 1));
        if (currentEntryNumber > 1)
        {
            int previousEntryNumber = currentEntryNumber - 1;
            string previousEntryName = "CodeEntry" + previousEntryNumber;
            Entry previousEntry = this.FindByName<Entry>(previousEntryName);
            if (previousEntry != null)
            {
                previousEntry.Focus();
            }
        }
    }
}
