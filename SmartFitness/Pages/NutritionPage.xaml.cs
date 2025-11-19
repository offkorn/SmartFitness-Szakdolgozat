using Microsoft.Maui.Controls;

namespace SmartFitness.Pages
{
    public partial class NutritionPage : ContentPage
    {
        public NutritionPage()
        {
            InitializeComponent();
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}