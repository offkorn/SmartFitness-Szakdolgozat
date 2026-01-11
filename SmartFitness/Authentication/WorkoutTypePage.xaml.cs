using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using SmartFitness.Models;
using System.Collections.Generic;
using System.Linq;

namespace SmartFitness.Authentication
{
    public partial class WorkoutTypePage : ContentPage
    {
        private readonly User _newUser;
        private readonly WorkoutModel _workoutPrefs;
        private readonly Dictionary<string, Frame> _workoutFrames;
        private List<string> _selectedTypes;

        public WorkoutTypePage(User user, WorkoutModel workoutPrefs)
        {
            InitializeComponent();
            _newUser = user;
            _workoutPrefs = workoutPrefs;
            _selectedTypes = string.IsNullOrEmpty(_workoutPrefs.WorkoutType)
                ? new List<string>()
                : _workoutPrefs.WorkoutType.Split(',').ToList();

            _workoutFrames = new Dictionary<string, Frame>
            {
                { "Weights", WeightsFrame },
                { "Bodyweight", BodyweightFrame },
                { "Machine", MachineFrame }
            };

            // Initialize colors and restore selections
            foreach (var frame in _workoutFrames.Values)
            {
                frame.BackgroundColor = Colors.White;
            }

            foreach (var type in _selectedTypes)
            {
                if (_workoutFrames.TryGetValue(type, out var frame))
                {
                    frame.BackgroundColor = Color.FromRgb(218, 215, 147); 
                }
            }

            // Setup tap gestures
            foreach (var (type, frame) in _workoutFrames)
            {
                var tap = new TapGestureRecognizer();
                tap.Tapped += (s, e) => ToggleWorkoutType(type);
                frame.GestureRecognizers.Add(tap);
            }
        }

        private void ToggleWorkoutType(string type)
        {
            if (_selectedTypes.Contains(type))
            {
                _selectedTypes.Remove(type);
                _workoutFrames[type].BackgroundColor = Colors.White;
            }
            else
            {
                _selectedTypes.Add(type);
                _workoutFrames[type].BackgroundColor = Color.FromRgb(218, 215, 147); // 
            }
        }

        private async void OnNextButtonClicked(object sender, EventArgs e)
        {
            if (!_selectedTypes.Any())
            {
                await DisplayAlert("Warning", "Please select at least one workout type", "OK");
                return;
            }

            // A kiválasztott típusokat stringgé alakítjuk és mentjük a WorkoutPreferences-be
            _workoutPrefs.WorkoutType = string.Join(",", _selectedTypes);

            // Navigáció a következõ oldalra
            await Navigation.PushAsync(new WorkoutDurationPage(_newUser, _workoutPrefs));
        }

        private async void OnPreviousButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync(); // Vissza a WorkoutLocationPage-re
        }
    }
}