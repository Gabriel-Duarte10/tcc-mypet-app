using System.ComponentModel;

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

    public ForgotPasswordView()
    {
        InitializeComponent();
        this.BindingContext = this;
    }

    private async void OnProxButtonClicked(object sender, EventArgs e)
    {
        // Navega para a página LoginView
        this.Email = false;
        this.Codigo = true;
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
