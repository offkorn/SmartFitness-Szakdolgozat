using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using SmartFitness.Models;

namespace SmartFitness.Authentication
{
    public partial class DietGoalPage : ContentPage
    {
        private readonly User _newUser;
        private readonly DietPreferences _dietPrefs;
        private Frame? _selectedFrame; // Nullable Frame a kiválasztott célhoz

        public DietGoalPage(User user, DietPreferences dietPrefs)
        {
            InitializeComponent();
            _newUser = user;
            _dietPrefs = dietPrefs;

            InitializeFrames();
            SetupGestureRecognizers();

            // Ha már van kiválasztott cél, jelöljük ki
            if (!string.IsNullOrEmpty(_dietPrefs.DietGoal))
            {
                switch (_dietPrefs.DietGoal.ToLower())
                {
                    case "bulk":
                        SelectGoal("bulk", BulkFrame);
                        break;
                    case "lose fat":
                        SelectGoal("lose fat", LoseFatFrame);
                        break;
                    case "maintain":
                        SelectGoal("maintain", MaintainMuscleFrame);
                        break;
                }
            }
        }

        private void InitializeFrames()
        {
            BulkFrame.BackgroundColor = Colors.White;
            LoseFatFrame.BackgroundColor = Colors.White;
            MaintainMuscleFrame.BackgroundColor = Colors.White;
        }

        private void SetupGestureRecognizers()
        {
            var bulkTap = new TapGestureRecognizer();
            bulkTap.Tapped += (s, e) => SelectGoal("bulk", BulkFrame);
            BulkFrame.GestureRecognizers.Add(bulkTap);

            var loseFatTap = new TapGestureRecognizer();
            loseFatTap.Tapped += (s, e) => SelectGoal("lose fat", LoseFatFrame);
            LoseFatFrame.GestureRecognizers.Add(loseFatTap);

            var maintainTap = new TapGestureRecognizer();
            maintainTap.Tapped += (s, e) => SelectGoal("maintain", MaintainMuscleFrame);
            MaintainMuscleFrame.GestureRecognizers.Add(maintainTap);
        }

        private void SelectGoal(string goal, Frame selectedFrame)
        {
            // Reset previous selection
            if (_selectedFrame != null)
            {
                _selectedFrame.BackgroundColor = Colors.White;
            }

            // Set new selection
            _selectedFrame = selectedFrame;
            _selectedFrame.BackgroundColor = Color.FromRgb(218, 215, 147); // LightBlue
            _dietPrefs.DietGoal = goal;
        }

        private async void OnNextButtonClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_dietPrefs.DietGoal))
            {
                await DisplayAlert("Warning", "Please select a diet goal", "OK");
                return;
            }

            await Navigation.PushAsync(new CookingTimePage(_newUser, _dietPrefs));
        }

        private async void OnPreviousButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync(); // Vissza a DislikedFoodsPage-re
        }
    }
}