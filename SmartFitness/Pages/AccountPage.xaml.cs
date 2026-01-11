using SmartFitness.Authentication;
using SmartFitness.Services;


namespace SmartFitness.Pages
{
    public partial class AccountPage : ContentPage
    {
        public AccountPage()
        {
            InitializeComponent();
            LoadUserData();
        }

        private async void LoadUserData()
        {
            try
            {
                // Null checkek
                firstNameLabel.Text = $"{App.CurrentUser.FirstName}";
                lastNameLabel.Text = $"{App.CurrentUser.LastName}";
                emailLabel.Text = $"{App.CurrentUser.Email}";

                
                bornDateLabel.Text = App.CurrentUser.BornDate?.ToShortDateString() ?? "Nincs megadva";

                
                weightLabel.Text = $"{(App.CurrentUser.Weight.HasValue ? App.CurrentUser.Weight.ToString() : "-")} kg";

                heightLabel.Text = $"{(App.CurrentUser.Height.HasValue ? App.CurrentUser.Height.ToString() : "-")} cm";

                genderLabel.Text = $"{App.CurrentUser.Gender}";

            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load account data: {ex.Message}", "OK");
            }
        }

        private async void OnSignOutClicked(object sender, EventArgs e)
        {
            bool confirmed = await DisplayAlert("Confirm Sign Out", 
                "Are you sure you want to log out?", "Yes", "No");

            if (!confirmed)
                return;

            try
            {
                //  Kijelentkezés Supabase Auth-ból
                await SupabaseClient.Client.Auth.SignOut();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to sign out: {ex.Message}", "OK");
            }

            //  Lokális user adat törlése
            App.CurrentUser = null;
            //  (mybe NE TÖRÖLD a Preferences UserId-t, ha már nem használjuk, , mindenhol auth-ra kene atallni currentuser helyett)
            Preferences.Remove("UserId");

            // session törlése
            await SupabaseClient.Client.Auth.SignOut();
            Preferences.Remove("supabase_session");

            
            Application.Current.MainPage = new NavigationPage(new WelcomePage());



            //  Visszairányítás a kezdolapra 
            Application.Current.MainPage = new NavigationPage(new WelcomePage());
        }


        private async void OnEditProfileClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Info", "Profile editing is not implemented yet.", "OK");
        }

        private async void OnBackButton(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}