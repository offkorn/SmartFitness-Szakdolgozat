using Microsoft.Maui.Controls;

namespace SmartFitness.Pages
{
    public partial class ProgressPage : ContentPage
    {
        public ProgressPage()
        {
            InitializeComponent();
        }

        private async void OnSettingsClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SettingsPage());
        }
    }
}
