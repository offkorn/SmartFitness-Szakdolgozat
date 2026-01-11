using SmartFitness.Models;
using SmartFitness.Services;
using System;

namespace SmartFitness.Authentication
{
    public partial class PersonalInfoPage : ContentPage
    {
        private User _newUser;
       
        

        public PersonalInfoPage(User user)
        {
            InitializeComponent();
            _newUser = user;
            
        }



        private async void OnNextButtonClicked(object sender, EventArgs e)
        {
            // Ellenõrzések
            if (string.IsNullOrWhiteSpace(HeightEntry.Text) ||
                string.IsNullOrWhiteSpace(WeightEntry.Text) ||
                GenderPicker.SelectedItem == null)
            {
                await DisplayAlert("Error", "Please fill in all fields", "OK");
                return;
            }

            if (!float.TryParse(HeightEntry.Text, out float height) || !float.TryParse(WeightEntry.Text, out float weight))
            {
                await DisplayAlert("Error", "Height and Weight must be valid numbers", "OK");
                return;
            }

            if (string.IsNullOrEmpty(_newUser.Id))
            {
                await DisplayAlert("Error", "User ID is missing. Please try registering again.", "OK");
                return;
            }

            // Adatok 
            _newUser.BornDate = BirthDatePicker.Date;
            _newUser.Height = height;
            _newUser.Weight = weight;
            _newUser.Gender = GenderPicker.SelectedItem.ToString();
            //_newUser.CreatedAt = DateTime.UtcNow;

            try
            {
                // 1. DIAGNOSZTIKA: Van-e Session?
                var session = SupabaseClient.Client.Auth.CurrentSession;
                if (session == null)
                {
                    // Ha ez ugrik fel, akkor a Confirm Email van bekapcsolva, vagy elveszett a login!
                    await DisplayAlert("Hiba", "Nincs aktív bejelentkezés! (Session is null)", "OK");
                    return;
                }

                System.Diagnostics.Debug.WriteLine($"[DEBUG] Logged in user: {session.User.Id}");
                System.Diagnostics.Debug.WriteLine($"[DEBUG] Saving data for: {_newUser.Id}");

                // 2. MENTÉS (Upsert a legbiztosabb)
                await SupabaseClient.Client
                    .From<User>()
                    .Upsert(_newUser);

                System.Diagnostics.Debug.WriteLine("[DEBUG] Mentés sikeres!");
                await Navigation.PushAsync(new WorkoutLocationPage(_newUser));
            }
            catch (Exception ex)
            {
                // Ha itt 42501 jön, akkor az ID-k nem egyeznek, vagy az RLS rossz.
                await DisplayAlert("Error", $"Failed to save user: {ex.Message}", "OK");
                System.Diagnostics.Debug.WriteLine($"[ERROR] {ex.Message}");
            }
        }
        
        private async void OnPreviousButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
        private async void OnBackButton(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}