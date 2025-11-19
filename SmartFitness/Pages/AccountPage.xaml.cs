using SmartFitness.Authentication;


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
                firstNameLabel.Text = $"{App.CurrentUser.FirstName}";
                lastNameLabel.Text = $"{App.CurrentUser.LastName}";
                emailLabel.Text = $"{App.CurrentUser.Email}";
                bornDateLabel.Text = App.CurrentUser.BornDate.ToShortDateString();
                weightLabel.Text = $"{App.CurrentUser.Weight} kg";
                heightLabel.Text = $"{App.CurrentUser.Height} cm";
                genderLabel.Text = $"{App.CurrentUser.Gender}"; 
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load account data: {ex.Message}", "OK");
            }
        }

        private async void OnSignOutClicked(object sender, EventArgs e)
        {
            // Megerõsítõ kérdés
            bool confirmed = await DisplayAlert("Confirm Sign Out", "Are you sure you want to log out?", "Yes", "No");

            if (confirmed)
            {
                // Kijelentkezés logikája
                Preferences.Remove("UserId");
                App.CurrentUser = null;

                // Visszairányítás a WelcomePage-re és az összes többi oldal bezárása
                await Navigation.PopToRootAsync();
                if (Application.Current != null)
                {
                    Application.Current.MainPage = new NavigationPage(new WelcomePage());
                }
            }
            // Ha "No"-t nyom, nem történik semmi, marad az AccountPage-en
        }

        private async void OnEditProfileClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Info", "Profile editing is not yet implemented.", "OK");
        }

        private async void OnBackButton(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}