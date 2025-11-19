using Microsoft.Maui.Controls;
using SmartFitness.Services;
using SmartFitness.Authentication;
using SmartFitness.Models;
using System.Threading.Tasks;

namespace SmartFitness
{
    public partial class App : Application
    {
        public static User CurrentUser { get; set; }

        public App()
        {
            InitializeComponent();

            // Supabase inicializálás
            SupabaseClient.Initialize();

            // Ellenőrizzük, van-e bejelentkezett felhasználó
            MainPage = IsUserLoggedIn() ? new AppShell() : new NavigationPage(new WelcomePage());
        }

        private bool IsUserLoggedIn()
        {
            // Ha van tárolt UserId, lekérdezzük a Supabase-ből
            string? userId = Preferences.Get("UserId", null);
            if (!string.IsNullOrEmpty(userId))
            {
                Task.Run(async () =>
                {
                    var user = await SupabaseClient.Client
                        .From<SmartFitness.Models.User>()
                        .Where(u => u.Id == userId)
                        .Single();
                    if (user != null)
                    {
                        CurrentUser = user;
                    }
                }).Wait(); // Szinkron várakozás az egyszerűség kedvéért (élesben async legyen)
                return CurrentUser != null;
            }
            return false;
        }
    }
}