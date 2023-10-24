using tcc_mypet_app.Views.Auth;

namespace tcc_mypet_app
{
    public partial class App : Application
    {
        public App(HomeView homeView)
        {
            InitializeComponent();

            MainPage = new NavigationPage(homeView);
        }
    }
}