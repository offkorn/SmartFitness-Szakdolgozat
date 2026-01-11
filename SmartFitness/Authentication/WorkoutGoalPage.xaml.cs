using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using SmartFitness.Models;
using System.Collections.Generic;
using System.Linq;

namespace SmartFitness.Authentication
{
    public partial class WorkoutGoalPage : ContentPage
    {
        private readonly User _newUser;
        private readonly WorkoutModel _workoutPrefs;
        private readonly List<string> _selectedGoals;
        private readonly Dictionary<string, Frame> _goalFrames;

        public WorkoutGoalPage(User user, WorkoutModel workoutPrefs)
        {
            InitializeComponent();
            _newUser = user;
            _workoutPrefs = workoutPrefs;

            // Inicializáljuk a kiválasztott célokat a meglévõ WorkoutGoal alapján
            _selectedGoals = string.IsNullOrEmpty(_workoutPrefs.WorkoutGoal)
                ? new List<string>()
                : _workoutPrefs.WorkoutGoal.Split(',').ToList();

            _goalFrames = new Dictionary<string, Frame>
            {
                { "Strength", StrengthFrame },
                { "Muscle Gain", MuscleGainFrame },
                { "Cardio", CardioFrame },
                { "Healthier Life", HealthierLifeFrame }
            };

            InitializeFrames();
            SetupGestureRecognizers();
        }

        private void InitializeFrames()
        {
            foreach (var frame in _goalFrames.Values)
            {
                frame.BackgroundColor = Colors.White;
            }

            // Visszaállítjuk a korábban kiválasztott célokat
            foreach (var goal in _selectedGoals)
            {
                if (_goalFrames.TryGetValue(goal, out var frame))
                {
                    frame.BackgroundColor = Color.FromRgb(218, 215, 147); 
                }
            }
        }

        private void SetupGestureRecognizers()
        {
            foreach (var (goal, frame) in _goalFrames)
            {
                var tapGesture = new TapGestureRecognizer();
                tapGesture.Tapped += (s, e) => ToggleGoalSelection(goal);
                frame.GestureRecognizers.Add(tapGesture);
            }
        }

        private void ToggleGoalSelection(string goal)
        {
            if (_selectedGoals.Contains(goal))
            {
                _selectedGoals.Remove(goal);
                _goalFrames[goal].BackgroundColor = Colors.White;
            }
            else
            {
                _selectedGoals.Add(goal);
                _goalFrames[goal].BackgroundColor = Color.FromRgb(218, 215, 147); // LightBlue
            }
        }

        private async void OnNextButtonClicked(object sender, EventArgs e)
        {
            if (_selectedGoals.Count == 0)
            {
                await DisplayAlert("Warning", "Please select at least one goal", "OK");
                return;
            }

            // A kiválasztott célokat stringgé alakítjuk és mentjük a WorkoutPreferences-be
            _workoutPrefs.WorkoutGoal = string.Join(",", _selectedGoals);

            // Navigáció a következõ oldalra
            await Navigation.PushAsync(new WorkoutExperiencePage(_newUser, _workoutPrefs));
        }

        private async void OnPreviousButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync(); // Vissza a WorkoutDaysPage-re
        }
    }
}