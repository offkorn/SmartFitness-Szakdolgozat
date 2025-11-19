using Microsoft.Maui.Controls;
using SmartFitness.Models;
using SmartFitness.Services;
using SmartFitness.Pages;

namespace SmartFitness.Authentication
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private async void OnLogIn(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(emailEntry.Text) || string.IsNullOrEmpty(passwordEntry.Text))
            {
                await DisplayAlert("Error", "Please enter both email and password", "OK");
                return;
            }

            try
            {
                // Supabase Auth bejelentkezés
                var session = await SupabaseClient.Client.Auth.SignInWithPassword(emailEntry.Text, passwordEntry.Text);

                if (session.User == null)
                {
                    await DisplayAlert("Error", "Invalid email or password", "OK");
                    return;
                }

                var user = await SupabaseClient.Client
                    .From<User>()
                    .Where(u => u.Id == session.User.Id)
                    .Single();

               

                if (user != null)
                {
                    // Tároljuk a bejelentkezett felhasználót és az ID-t
                    App.CurrentUser = user;
                    Preferences.Set("UserId", user.Id);

                    //await DisplayAlert("Success", "Login successful!", "OK");
                    if (Application.Current != null)
                    {
                        Application.Current.MainPage = new AppShell();
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Login failed: {ex.Message}", "OK");
            }
        
        }

        private async void OnForgotPassword(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ForgotPasswordPage());
        }

        private async void OnBackButton(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}