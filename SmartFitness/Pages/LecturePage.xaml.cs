using Microsoft.Maui.Controls;

namespace SmartFitness.Pages
{
    public partial class LecturePage : ContentPage
    {
        public LecturePage()
        {
            InitializeComponent();
        }

        private async void OnMuscleBuildingClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MuscleBuildingPage());
        }

        private async void OnNutritionClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new NutritionPage());
        }

        private async void OnSleepClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SleepPage());
        }
    }
}