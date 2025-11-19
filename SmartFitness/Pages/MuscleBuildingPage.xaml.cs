using Microsoft.Maui.Controls;

namespace SmartFitness.Pages
{
    public partial class MuscleBuildingPage : ContentPage
    {
        public MuscleBuildingPage()
        {
            InitializeComponent();
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}