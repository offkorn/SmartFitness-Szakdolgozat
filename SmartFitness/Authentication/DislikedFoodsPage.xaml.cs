using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using SmartFitness.Models;
using System.Collections.Generic;
using System.Linq;

namespace SmartFitness.Authentication
{
    public partial class DislikedFoodsPage : ContentPage
    {
        private readonly User _newUser;
        private readonly DietPreferences _dietPrefs;
        private readonly HashSet<string> _selectedDislikedFoods;
        private readonly Dictionary<string, Frame> _foodFrames;

        public DislikedFoodsPage(User user, DietPreferences dietPrefs)
        {
            InitializeComponent();
            _newUser = user;
            _dietPrefs = dietPrefs;

            // Inicializáljuk a kiválasztott nem kedvelt ételeket, ha már van érték
            _selectedDislikedFoods = string.IsNullOrEmpty(_dietPrefs.DislikedFoods)
                ? new HashSet<string>()
                : new HashSet<string>(_dietPrefs.DislikedFoods.Split(','));

            _foodFrames = new Dictionary<string, Frame>
            {
                { "Fish", FishFrame },
                { "Pork", PorkFrame },
                { "Egg", EggFrame },
                { "Milk", MilkFrame },
                { "Potato", PotatoFrame },
                { "Nuts", NutsFrame }
            };

            InitializeFrames();
            SetupGestureRecognizers();
        }

        private void InitializeFrames()
        {
            foreach (var frame in _foodFrames.Values)
            {
                frame.BackgroundColor = Colors.White;
            }

            // Visszaállítjuk a korábban kiválasztott ételeket
            foreach (var food in _selectedDislikedFoods)
            {
                if (_foodFrames.TryGetValue(food, out var frame))
                {
                    frame.BackgroundColor = Color.FromRgb(218, 215, 147); // LightBlue
                }
            }
        }

        private void SetupGestureRecognizers()
        {
            foreach (var (food, frame) in _foodFrames)
            {
                var tapGesture = new TapGestureRecognizer();
                tapGesture.Tapped += (s, e) => ToggleFoodSelection(food);
                frame.GestureRecognizers.Add(tapGesture);
            }
        }

        private void ToggleFoodSelection(string food)
        {
            if (_selectedDislikedFoods.Contains(food))
            {
                _selectedDislikedFoods.Remove(food);
                _foodFrames[food].BackgroundColor = Colors.White;
            }
            else
            {
                _selectedDislikedFoods.Add(food);
                _foodFrames[food].BackgroundColor = Color.FromRgb(218, 215, 147); // LightBlue
            }
        }

        private async void OnNextButtonClicked(object sender, EventArgs e)
        {
            // Ha semmi sincs kiválasztva, none stringként mentjük
            _dietPrefs.DislikedFoods = _selectedDislikedFoods.Any()
                ? string.Join(",", _selectedDislikedFoods)
                : "none";

            await Navigation.PushAsync(new DietGoalPage(_newUser, _dietPrefs));
        }

        private async void OnPreviousButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync(); // Vissza a FoodIntolerancePage-re
        }
    }
}