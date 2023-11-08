using System.IdentityModel.Tokens.Jwt;
using tcc_mypet_app.Services;
using tcc_mypet_app.Views.App;
using tcc_mypet_app.Views.Auth;

namespace tcc_mypet_app
{
    public partial class App : Application
    {
        public App(HomeView homeView)
        {
            InitializeComponent();
            CheckTokenAndNavigate();
        }

        private async void CheckTokenAndNavigate()
        {
            var token = Preferences.Get("Token", string.Empty);
            if (!string.IsNullOrEmpty(token))
            {
                // Supondo que você tenha um método para verificar a validade do token
                var isValidToken = TokenService.VerifyTokenValidity(token);
                if (isValidToken)
                {
                    MainPage = new NavigationPage(new AppView());
                }
                else
                {
                    MainPage = new NavigationPage(new HomeView());
                }
            }
            else
            {
                MainPage = new NavigationPage(new HomeView());
            }
        }
    }
}