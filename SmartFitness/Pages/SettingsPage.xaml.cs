using Microsoft.Maui.Controls;

namespace SmartFitness.Pages
{
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();
        }

        private async void OnAccountClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AccountPage());
        }


        private async void OnWorkoutPreferenceClicked(object sender, TappedEventArgs e)
        {
            await Navigation.PushAsync(new WorkoutPreferencePage());
        }

        private async void OnDietPreferenceClicked(object sender, TappedEventArgs e)
        {
            await Navigation.PushAsync(new DietPreferencePage());
        }

        private async void OnBackButton(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}
