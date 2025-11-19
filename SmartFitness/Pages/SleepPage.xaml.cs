using Microsoft.Maui.Controls;

namespace SmartFitness.Pages
{
    public partial class SleepPage : ContentPage
    {
        public SleepPage()
        {
            InitializeComponent();
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}