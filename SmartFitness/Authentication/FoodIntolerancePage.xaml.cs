using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using SmartFitness.Models;
using System.Collections.Generic;
using System.Linq;

namespace SmartFitness.Authentication
{
    public partial class FoodIntolerancePage : ContentPage
    {
        private readonly User _newUser;
        private readonly DietPreferences _dietPrefs;
        private readonly HashSet<string> _selectedIntolerances;
        private readonly Dictionary<string, Frame> _intoleranceFrames;

        public FoodIntolerancePage(User user, DietPreferences dietPrefs)
        {
            InitializeComponent();
            _newUser = user;
            _dietPrefs = dietPrefs;

            // Inicializáljuk a kiválasztott intoleranciákat, ha már van érték
            _selectedIntolerances = string.IsNullOrEmpty(_dietPrefs.FoodIntolerance)
                ? new HashSet<string>()
                : new HashSet<string>(_dietPrefs.FoodIntolerance.Split(','));

            _intoleranceFrames = new Dictionary<string, Frame>
            {
                { "Dairy", DairyFrame },
                { "Gluten", GlutenFrame },
                { "Nuts", NutsFrame },
                { "Seafood", SeafoodFrame }
            };

            InitializeFrames();
            SetupGestureRecognizers();
        }

        private void InitializeFrames()
        {
            foreach (var frame in _intoleranceFrames.Values)
            {
                frame.BackgroundColor = Colors.WhiteSmoke;
            }

            // Visszaállítjuk a korábban kiválasztott intoleranciákat
            foreach (var intolerance in _selectedIntolerances)
            {
                if (_intoleranceFrames.TryGetValue(intolerance, out var frame))
                {
                    frame.BackgroundColor = Color.FromRgb(173, 216, 230); // LightBlue
                }
            }
        }

        private void SetupGestureRecognizers()
        {
            foreach (var (intolerance, frame) in _intoleranceFrames)
            {
                var tapGesture = new TapGestureRecognizer();
                tapGesture.Tapped += (s, e) => ToggleIntoleranceSelection(intolerance);
                frame.GestureRecognizers.Add(tapGesture);
            }
        }

        private void ToggleIntoleranceSelection(string intolerance)
        {
            if (_selectedIntolerances.Contains(intolerance))
            {
                _selectedIntolerances.Remove(intolerance);
                _intoleranceFrames[intolerance].BackgroundColor = Colors.WhiteSmoke;
            }
            else
            {
                _selectedIntolerances.Add(intolerance);
                _intoleranceFrames[intolerance].BackgroundColor = Color.FromRgb(173, 216, 230); // LightBlue
            }
        }

        private async void OnNextButtonClicked(object sender, EventArgs e)
        {
            // Ha semmi sincs kiválasztva, none stringként mentjük
            _dietPrefs.FoodIntolerance = _selectedIntolerances.Any()
                ? string.Join(",", _selectedIntolerances)
                : "none";

            await Navigation.PushAsync(new DislikedFoodsPage(_newUser, _dietPrefs));
        }

        private async void OnPreviousButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync(); // Vissza a WorkoutExperiencePage-re
        }
    }
}