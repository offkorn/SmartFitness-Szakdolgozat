using Microsoft.Maui.Controls;

namespace SmartFitness.Authentication
{
    public partial class ForgotPasswordPage : ContentPage
    {
        public ForgotPasswordPage()
        {
            InitializeComponent();
        }

        private async void OnResetPassword(object sender, EventArgs e)
        {
            // Ide jön majd az email küldési logika

            await DisplayAlert("Success", "Password reset email sent!", "OK");
            await Navigation.PopAsync(); // Visszalépés a LoginPage-re
        }
    }
}
