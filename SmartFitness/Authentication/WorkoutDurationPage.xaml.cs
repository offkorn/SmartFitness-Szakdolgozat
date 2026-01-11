using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using SmartFitness.Models;

namespace SmartFitness.Authentication
{
    public partial class WorkoutDurationPage : ContentPage
    {
        private readonly User _newUser;
        private readonly WorkoutModel _workoutPrefs;
        private readonly Dictionary<string, Frame> _durationFrames;

        public WorkoutDurationPage(User user, WorkoutModel workoutPrefs)
        {
            InitializeComponent();
            _newUser = user;
            _workoutPrefs = workoutPrefs;

            _durationFrames = new Dictionary<string, Frame>
                {
                    { "<10 mins", TenMinutesFrame },
                    { "20 mins", TwentyMinutesFrame },
                    { "30 mins", ThirtyMinutesFrame },
                    { "1 hour", OneHourFrame },
                    { "1.5 hours", OneAndHalfHourFrame }
                };

            // Initialize all frames
            foreach (var frame in _durationFrames.Values)
            {
                frame.BackgroundColor = Colors.White;
            }

            // Restore previous selection if exists
            if (_workoutPrefs.WorkoutDuration > 0)
            {
                string? selectedDuration = _workoutPrefs.WorkoutDuration switch
                {
                    10 => "<10 mins",
                    20 => "20 mins",
                    30 => "30 mins",
                    60 => "1 hour",
                    90 => "1.5 hours",
                    _ => null
                };
                if (selectedDuration != null && _durationFrames.TryGetValue(selectedDuration, out var frame))
                {
                    frame.BackgroundColor = Color.FromRgb(218, 215, 147); 
                }
            }

            // Setup tap gestures
            foreach (var (duration, frame) in _durationFrames)
            {
                var tap = new TapGestureRecognizer();
                tap.Tapped += (s, e) => SelectDuration(duration);
                frame.GestureRecognizers.Add(tap);
            }
        }

        private void SelectDuration(string duration)
        {
            _workoutPrefs.WorkoutDuration = duration switch
            {
                "<10 mins" => 10,
                "20 mins" => 20,
                "30 mins" => 30,
                "1 hour" => 60,
                "1.5 hours" => 90,
                _ => 0
            };

            foreach (var (dur, frame) in _durationFrames)
            {
                frame.BackgroundColor = dur == duration
                    ? Color.FromRgb(218, 215, 147) // LightBlue for selected
                    : Colors.White;            // WhiteSmoke for others
            }
        }

        private async void OnNextButtonClicked(object sender, EventArgs e)
        {
            if (_workoutPrefs.WorkoutDuration == 0)
            {
                await DisplayAlert("Warning", "Please select a workout duration", "OK");
                return;
            }

            // Navigáció a következõ oldalra
            await Navigation.PushAsync(new WorkoutDaysPage(_newUser, _workoutPrefs));
        }

        private async void OnPreviousButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync(); // Vissza a WorkoutTypePage-re
        }
    }
}