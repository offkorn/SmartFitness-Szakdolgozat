using SmartFitness.Models;

namespace SmartFitness.Authentication
{
    public partial class WorkoutLocationPage : ContentPage
    {
        private readonly User _newUser;
        private readonly WorkoutModel _workoutPrefs;

        public WorkoutLocationPage(User user)
        {
            InitializeComponent();
            _newUser = user; // Az elõzõ oldalról kapott User objektum
            _workoutPrefs = new WorkoutModel { UserId = null }; // Az ID-t késõbb töltjük ki

            // Initialize colors
            GymFrame.BackgroundColor = Colors.White;
            HomeFrame.BackgroundColor = Colors.White;

            // Setup tap gestures
            var gymTap = new TapGestureRecognizer();
            gymTap.Tapped += (s, e) =>
            {
                _workoutPrefs.WorkoutLocation = "Gym";
                GymFrame.BackgroundColor = Color.FromRgb(218, 215, 147); 
                HomeFrame.BackgroundColor = Colors.White;
            };
            GymFrame.GestureRecognizers.Add(gymTap);

            var homeTap = new TapGestureRecognizer();
            homeTap.Tapped += (s, e) =>
            {
                _workoutPrefs.WorkoutLocation = "Home";
                HomeFrame.BackgroundColor = Color.FromRgb(218, 215, 147); 
                GymFrame.BackgroundColor = Colors.White;
            };
            HomeFrame.GestureRecognizers.Add(homeTap);
        }

        private async void OnNextButtonClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_workoutPrefs.WorkoutLocation))
            {
                await DisplayAlert("Warning", "Please select a workout location", "OK");
                return;
            }

            // Navigáció a következõ oldalra, átadva a User és WorkoutPreferences objektumokat
            await Navigation.PushAsync(new WorkoutTypePage(_newUser, _workoutPrefs));
        }

        private async void OnPreviousButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync(); // Vissza a PersonalInfoPage-re
        }
        
    }
}