using SmartFitness.Models;
using SmartFitness.Services;
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
            // Ellenőrzések
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
                // Ellenőrizzük, hogy az email már létezik-e a public.users táblában
                var existingUser = await SupabaseClient.Client
                    .From<User>()
                    .Where(x => x.Email == EmailEntry.Text)
                    .Single();
                if (existingUser != null)
                {
                    await DisplayAlert("Error", "This email is already registered. Please use a different email or log in.", "OK");
                    return;
                }

                // Supabase Auth regisztráció
                var authResponse = await SupabaseClient.Client.Auth.SignUp(EmailEntry.Text, PasswordEntry.Text);
                if (authResponse.User == null)
                {
                    await DisplayAlert("Error", "Registration failed: Unknown error", "OK");
                    return;
                }


                // Adatok mentése az ideiglenes objektumba
                _newUser.Id = authResponse.User.Id;
                _newUser.FirstName = FirstNameEntry.Text;
                _newUser.LastName = LastNameEntry.Text;
                _newUser.Email = EmailEntry.Text;
                _newUser.CreatedAt = DateTime.UtcNow;

                // Hibakeresés: naplózzuk az ID-t
                System.Diagnostics.Debug.WriteLine($"SignUpPage: User ID = {_newUser.Id}");

                // Navigáció a következő oldalra
                await Navigation.PushAsync(new PersonalInfoPage(_newUser));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Registration failed: {ex.Message}", "OK");
            }
        }

        private async void OnBackButton(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

    }
}