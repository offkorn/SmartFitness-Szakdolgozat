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
            MainPage = new AppShell();

        }


        // amikor az app visszatér a háttérből
        protected override async void OnResume()
        {
            base.OnResume();

            
            try
            {
                System.Diagnostics.Debug.WriteLine("App felébredt - Session frissítése...");

                // érvényes-e még a token, és ha nem - kér újat 
                await SupabaseClient.Client.Auth.RetrieveSessionAsync();

                System.Diagnostics.Debug.WriteLine("Session updated!");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to update session: {ex.Message}");
            }
        }





    }



}
