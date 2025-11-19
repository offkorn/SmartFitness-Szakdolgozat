using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;

namespace SmartFitness.Authentication
{
    public partial class WelcomePage : ContentPage
    {
        public WelcomePage()
        {
            InitializeComponent();
        }

        private async void OnSignUpClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SignUpPage());
        }

        private async void OnLogInClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new LoginPage());
        }

        
    }
}
