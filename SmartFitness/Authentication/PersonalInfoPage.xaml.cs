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

            // Ellenõrizzük, hogy az ID nem NULL
            if (string.IsNullOrEmpty(_newUser.Id))
            {
                await DisplayAlert("Error", "User ID is missing. Please try registering again.", "OK");
                return;
            }

            // Adatok kiegészítése
            _newUser.BornDate = BirthDatePicker.Date;
            _newUser.Height = height;
            _newUser.Weight = weight;
            _newUser.Gender = GenderPicker.SelectedItem.ToString();
            _newUser.CreatedAt = DateTime.UtcNow;

            try
            {
                // Hibakeresés: naplózzuk az ID-t beszúrás elõtt
                System.Diagnostics.Debug.WriteLine($"PersonalInfoPage: Attempting to save User ID = {_newUser.Id}");

                // Ellenõrizzük, hogy létezik-e már rekord a public.users táblában
                var existingUser = await SupabaseClient.Client
                    .From<User>()
                    .Where(x => x.Id == _newUser.Id)
                    .Single();

                if (existingUser != null)
                {
                    // Ha létezik, frissítjük a rekordot
                    await SupabaseClient.Client
                        .From<User>()
                        .Where(x => x.Id == _newUser.Id)
                        .Set(x => x.FirstName, _newUser.FirstName)
                        .Set(x => x.LastName, _newUser.LastName)
                        .Set(x => x.BornDate, _newUser.BornDate)
                        .Set(x => x.Height, _newUser.Height)
                        .Set(x => x.Weight, _newUser.Weight)
                        .Set(x => x.Gender, _newUser.Gender)
                        .Set(x => x.CreatedAt, _newUser.CreatedAt)
                        .Update();
                }
                else
                {
                    // Ha nem létezik, új rekord beszúrása
                    await SupabaseClient.Client
                        .From<User>()
                        .Insert(_newUser);
                }

                // Hibakeresés: naplózzuk a mentett rekordot
                System.Diagnostics.Debug.WriteLine($"PersonalInfoPage: Saved User ID = {_newUser.Id}");

                await Navigation.PushAsync(new WorkoutLocationPage(_newUser));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to save user: {ex.Message}", "OK");
                System.Diagnostics.Debug.WriteLine($"PersonalInfoPage: Error = {ex.Message}");
            }
        }

        private async void OnPreviousButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}