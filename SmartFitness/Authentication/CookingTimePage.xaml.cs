using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using SmartFitness.Models;
using SmartFitness.Services;

namespace SmartFitness.Authentication
{
    public partial class CookingTimePage : ContentPage
    {
        private readonly User _newUser;
        private readonly DietPreferences _dietPrefs;
        private Frame? _selectedFrame; // Nullable Frame a kiválasztott idõhöz

        public CookingTimePage(User user, DietPreferences dietPrefs)
        {
            InitializeComponent();
            _newUser = user;
            _dietPrefs = dietPrefs;

            InitializeFrames();
            SetupGestureRecognizers();

            // Ha már van kiválasztott idõ, jelöljük ki
            if (_dietPrefs.CookingTime > 0)
            {
                switch (_dietPrefs.CookingTime)
                {
                    case 10:
                        SelectTime(10, LessThan10Frame);
                        break;
                    case 20:
                        SelectTime(20, TwentyMinFrame);
                        break;
                    case 30:
                        SelectTime(30, ThirtyMinFrame);
                        break;
                    case 60:
                        SelectTime(60, SixtyMinFrame);
                        break;
                    case 120:
                        SelectTime(120, OneTwentyMinFrame);
                        break;
                }
            }
        }

        private void InitializeFrames()
        {
            LessThan10Frame.BackgroundColor = Colors.WhiteSmoke;
            TwentyMinFrame.BackgroundColor = Colors.WhiteSmoke;
            ThirtyMinFrame.BackgroundColor = Colors.WhiteSmoke;
            SixtyMinFrame.BackgroundColor = Colors.WhiteSmoke;
            OneTwentyMinFrame.BackgroundColor = Colors.WhiteSmoke;
        }

        private void SetupGestureRecognizers()
        {
            var lessThan10Tap = new TapGestureRecognizer();
            lessThan10Tap.Tapped += (s, e) => SelectTime(10, LessThan10Frame);
            LessThan10Frame.GestureRecognizers.Add(lessThan10Tap);

            var twentyMinTap = new TapGestureRecognizer();
            twentyMinTap.Tapped += (s, e) => SelectTime(20, TwentyMinFrame);
            TwentyMinFrame.GestureRecognizers.Add(twentyMinTap);

            var thirtyMinTap = new TapGestureRecognizer();
            thirtyMinTap.Tapped += (s, e) => SelectTime(30, ThirtyMinFrame);
            ThirtyMinFrame.GestureRecognizers.Add(thirtyMinTap);

            var sixtyMinTap = new TapGestureRecognizer();
            sixtyMinTap.Tapped += (s, e) => SelectTime(60, SixtyMinFrame);
            SixtyMinFrame.GestureRecognizers.Add(sixtyMinTap);

            var oneTwentyMinTap = new TapGestureRecognizer();
            oneTwentyMinTap.Tapped += (s, e) => SelectTime(120, OneTwentyMinFrame);
            OneTwentyMinFrame.GestureRecognizers.Add(oneTwentyMinTap);
        }

        private void SelectTime(int minutes, Frame selectedFrame)
        {
            // Reset previous selection
            if (_selectedFrame != null)
            {
                _selectedFrame.BackgroundColor = Colors.WhiteSmoke;
            }

            // Set new selection
            _selectedFrame = selectedFrame;
            _selectedFrame.BackgroundColor = Color.FromRgb(173, 216, 230); // LightBlue
            _dietPrefs.CookingTime = minutes;
        }

        private async void OnFinishButtonClicked(object sender, EventArgs e)
        {
            if (_dietPrefs.CookingTime == 0)
            {
                await DisplayAlert("Warning", "Please select a cooking time", "OK");
                return;
            }

            try
            {
                // Ellenõrizzük, hogy a User ID létezik-e
                if (string.IsNullOrEmpty(_newUser.Id))
                {
                    await DisplayAlert("Error", "User ID is missing. Please restart registration.", "OK");
                    return;
                }

                // A User ID alapján állítsuk be a foreign key-t
                _dietPrefs.UserId = _newUser.Id;

                // Mentsük a DietPreferences-t a Supabase-be
                var dietResponse = await SupabaseClient.Client
                    .From<DietPreferences>()
                    .Insert(_dietPrefs);

                await DisplayAlert("Success", "Registration completed successfully!", "OK");

                if (Application.Current != null)
                {
                    Application.Current.MainPage = new AppShell(); // Fõoldalra irányítás
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to save diet preferences: {ex.Message}", "OK");
            }
        }

        private async void OnPreviousButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync(); // Vissza a DietGoalPage-re
        }
    }
}