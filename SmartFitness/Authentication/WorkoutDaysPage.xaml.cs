using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using SmartFitness.Models;

namespace SmartFitness.Authentication
{
    public partial class WorkoutDaysPage : ContentPage
    {
        private readonly User _newUser;
        private readonly WorkoutModel _workoutPrefs;
        private Frame? _selectedFrame;

        public WorkoutDaysPage(User user, WorkoutModel workoutPrefs)
        {
            InitializeComponent();
            _newUser = user;
            _workoutPrefs = workoutPrefs;

            InitializeFrames();
            SetupGestureRecognizers();

            // Restore previous selection if exists
            if (!string.IsNullOrEmpty(_workoutPrefs.WorkoutDays))
            {
                int days = int.Parse(_workoutPrefs.WorkoutDays);
                switch (days)
                {
                    case 1: SelectDays(1, Day1Frame); break;
                    case 2: SelectDays(2, Day2Frame); break;
                    case 3: SelectDays(3, Day3Frame); break;
                    case 4: SelectDays(4, Day4Frame); break;
                    case 5: SelectDays(5, Day5Frame); break;
                    case 6: SelectDays(6, Day6Frame); break;
                    case 7: SelectDays(7, Day7Frame); break;
                }
            }
        }

        private void InitializeFrames()
        {
            Day1Frame.BackgroundColor = Colors.WhiteSmoke;
            Day2Frame.BackgroundColor = Colors.WhiteSmoke;
            Day3Frame.BackgroundColor = Colors.WhiteSmoke;
            Day4Frame.BackgroundColor = Colors.WhiteSmoke;
            Day5Frame.BackgroundColor = Colors.WhiteSmoke;
            Day6Frame.BackgroundColor = Colors.WhiteSmoke;
            Day7Frame.BackgroundColor = Colors.WhiteSmoke;
        }

        private void SetupGestureRecognizers()
        {
            var day1Tap = new TapGestureRecognizer();
            day1Tap.Tapped += (s, e) => SelectDays(1, Day1Frame);
            Day1Frame.GestureRecognizers.Add(day1Tap);

            var day2Tap = new TapGestureRecognizer();
            day2Tap.Tapped += (s, e) => SelectDays(2, Day2Frame);
            Day2Frame.GestureRecognizers.Add(day2Tap);

            var day3Tap = new TapGestureRecognizer();
            day3Tap.Tapped += (s, e) => SelectDays(3, Day3Frame);
            Day3Frame.GestureRecognizers.Add(day3Tap);

            var day4Tap = new TapGestureRecognizer();
            day4Tap.Tapped += (s, e) => SelectDays(4, Day4Frame);
            Day4Frame.GestureRecognizers.Add(day4Tap);

            var day5Tap = new TapGestureRecognizer();
            day5Tap.Tapped += (s, e) => SelectDays(5, Day5Frame);
            Day5Frame.GestureRecognizers.Add(day5Tap);

            var day6Tap = new TapGestureRecognizer();
            day6Tap.Tapped += (s, e) => SelectDays(6, Day6Frame);
            Day6Frame.GestureRecognizers.Add(day6Tap);

            var day7Tap = new TapGestureRecognizer();
            day7Tap.Tapped += (s, e) => SelectDays(7, Day7Frame);
            Day7Frame.GestureRecognizers.Add(day7Tap);
        }

        private void SelectDays(int days, Frame selectedFrame)
        {
            // Reset previous selection
            if (_selectedFrame != null)
            {
                _selectedFrame.BackgroundColor = Colors.WhiteSmoke;
            }

            // Set new selection
            _selectedFrame = selectedFrame;
            _selectedFrame.BackgroundColor = Color.FromRgb(173, 216, 230); // LightBlue
            _workoutPrefs.WorkoutDays = days.ToString(); // Szövegként tároljuk
        }

        private async void OnNextButtonClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_workoutPrefs.WorkoutDays))
            {
                await DisplayAlert("Warning", "Please select workout days per week", "OK");
                return;
            }

            // Navigáció a következõ oldalra
            await Navigation.PushAsync(new WorkoutGoalPage(_newUser, _workoutPrefs));
        }

        private async void OnPreviousButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync(); // Vissza a WorkoutDurationPage-re
        }
    }
}