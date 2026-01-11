using SmartFitness.Models;
using SmartFitness.Services;
using SmartFitness.components;
using System;

namespace SmartFitness.Authentication
{
    public partial class SignUpPage : ContentPage
    {
        private User _newUser;

        public SignUpPage()
        {
            InitializeComponent();
            _newUser = new User();
        }

        private async void NextButton_Clicked(object sender, EventArgs e)
        {
            // Validációk 
            if (string.IsNullOrWhiteSpace(FirstNameEntry.Text) ||
                string.IsNullOrWhiteSpace(LastNameEntry.Text) ||
                string.IsNullOrWhiteSpace(EmailEntry.Text) ||
                string.IsNullOrWhiteSpace(PasswordEntry.Text) ||
                string.IsNullOrWhiteSpace(ConfirmPasswordEntry.Text))
            {
                await DisplayAlert("Error", "Please fill in all fields", "OK");
                return;
            }

            if (PasswordEntry.Text != ConfirmPasswordEntry.Text)
            {
                await DisplayAlert("Error", "Passwords do not match", "OK");
                return;
            }

            try
            {
                // 1. Regisztráció
                var authResponse = await SupabaseClient.Client.Auth.SignUp(EmailEntry.Text, PasswordEntry.Text);

                // 2. Ha a regisztráció sikeres
                if (authResponse.User != null)
                {
                    // --- JAVÍTÁS KEZDETE ---
                    // Ellenőrizzük, van-e Session. Ha nincs, beléptetjük a felhasználót kézzel.
                    if (SupabaseClient.Client.Auth.CurrentSession == null)
                    {
                        System.Diagnostics.Debug.WriteLine("Regisztráció kész, de nincs Session. Automatikus belépés...");
                        try
                        {
                            await SupabaseClient.Client.Auth.SignIn(EmailEntry.Text, PasswordEntry.Text);
                        }
                        catch (Exception loginEx)
                        {
                            await DisplayAlert("Hiba", "A regisztráció sikerült, de a belépés nem. Jelentkezz be manuálisan.", "OK");
                            await Navigation.PopAsync();
                            return;
                        }
                    }
                    // --- JAVÍTÁS VÉGE ---

                    // Ha most már van Session, mehetünk tovább
                    if (SupabaseClient.Client.Auth.CurrentSession != null)
                    {
                        NavigateToNextPage(authResponse.User.Id);
                    }
                    else
                    {
                        await DisplayAlert("Hiba", "Váratlan hiba: Nem sikerült a munkamenet (Session) létrehozása.", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                // Ha a user már létezik...
                if (ex.Message.Contains("User already registered") || ex.Message.Contains("422"))
                {
                    try
                    {
                        // Silent login próbálkozás
                        var loginResponse = await SupabaseClient.Client.Auth.SignIn(EmailEntry.Text, PasswordEntry.Text);
                        if (loginResponse.User != null)
                        {
                            NavigateToNextPage(loginResponse.User.Id);
                            return;
                        }
                    }
                    catch
                    {
                        await DisplayAlert("Hiba", "Ez az email cím már regisztrálva van. Kérlek jelentkezz be.", "OK");
                    }
                }
                else
                {
                    await DisplayAlert("Error", $"Registration failed: {ex.Message}", "OK");
                }
            }
        }

        private async void NavigateToNextPage(string userId)
        {
            _newUser.Id = userId;
            _newUser.FirstName = FirstNameEntry.Text;
            _newUser.LastName = LastNameEntry.Text;
            _newUser.Email = EmailEntry.Text;
            _newUser.CreatedAt = DateTime.UtcNow;

            await Navigation.PushAsync(new PersonalInfoPage(_newUser));
        }

        private async void OnBackButton(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

    }
}