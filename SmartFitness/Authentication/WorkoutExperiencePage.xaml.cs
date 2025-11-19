using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using SmartFitness.Models;
using SmartFitness.Services;

namespace SmartFitness.Authentication
{
    public partial class WorkoutExperiencePage : ContentPage
    {
        private User _newUser; // Nem readonly, hogy módosítható legyen
        private readonly WorkoutModel _workoutPrefs;
        private readonly Dictionary<string, Frame> _experienceFrames;

        public WorkoutExperiencePage(User user, WorkoutModel workoutPrefs)
        {
            InitializeComponent();
            _newUser = user;
            _workoutPrefs = workoutPrefs;

            _experienceFrames = new Dictionary<string, Frame>
            {
                { "Beginner", BeginnerFrame },
                { "Intermediate", IntermediateFrame },
                { "Advanced", AdvancedFrame }
            };

            InitializeFrames();
            SetupGestureRecognizers();

            if (!string.IsNullOrEmpty(_workoutPrefs.WorkoutExperience) &&
                _experienceFrames.ContainsKey(_workoutPrefs.WorkoutExperience))
            {
                SelectExperience(_workoutPrefs.WorkoutExperience);
            }
        }

        private void InitializeFrames()
        {
            foreach (var frame in _experienceFrames.Values)
            {
                frame.BackgroundColor = Colors.WhiteSmoke;
            }
        }

        private void SetupGestureRecognizers()
        {
            foreach (var (experience, frame) in _experienceFrames)
            {
                var tapGesture = new TapGestureRecognizer();
                tapGesture.Tapped += (s, e) => SelectExperience(experience);
                frame.GestureRecognizers.Add(tapGesture);
            }
        }

        private void SelectExperience(string experience)
        {
            foreach (var frame in _experienceFrames.Values)
            {
                frame.BackgroundColor = Colors.WhiteSmoke;
            }
            _experienceFrames[experience].BackgroundColor = Color.FromRgb(173, 216, 230);
            _workoutPrefs.WorkoutExperience = experience;
            NextButton.IsEnabled = true;
        }

        private async void OnNextButtonClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_workoutPrefs.WorkoutExperience))
            {
                await DisplayAlert("Warning", "Please select your experience level", "OK");
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
                _workoutPrefs.UserId = _newUser.Id; // Javítsd ki Id-rõl UserId-re

                // Csak a WorkoutPreferences-t mentsük
                var workoutResponse = await SupabaseClient.Client
                    .From<WorkoutModel>()
                    .Insert(_workoutPrefs);

                // Létrehozzuk a DietPreferences objektumot és továbbnavigálunk
                var dietPrefs = new DietPreferences { UserId = _newUser.Id };
                await Navigation.PushAsync(new FoodIntolerancePage(_newUser, dietPrefs));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to save workout preferences: {ex.Message}", "OK");
            }
        }

        private async void OnPreviousButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}