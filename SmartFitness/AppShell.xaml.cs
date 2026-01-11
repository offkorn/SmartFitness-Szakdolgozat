using CommunityToolkit.Maui.Behaviors;
using CommunityToolkit.Maui.Core;
using System.Runtime.Versioning;
using SmartFitness.Authentication;
using SmartFitness.Services;
using SmartFitness.Models;

namespace SmartFitness
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            _ = InitializeAppAsync();
            UpdateStatusBarColor();
        }

        protected override void OnNavigated(ShellNavigatedEventArgs args)
        {       
            if (Shell.Current?.CurrentPage is WelcomePage)
                return;

            base.OnNavigated(args);
            UpdateStatusBarColor();
        }


        private void UpdateStatusBarColor()
        {
            var route = Shell.Current?.CurrentState?.Location?.OriginalString ?? "";
            var routeWithoutBase = route.StartsWith("//") ? route.Substring(2) : route;
            var segments = routeWithoutBase.Split('/', StringSplitOptions.RemoveEmptyEntries);

            // Alapértelmezés: szürke státuszsáv
            var color = Color.Parse("#F6FAFD");

            // KIVÉTEL ha a WelcomePage vagy LoginPage az aktuális
            if (segments.Length > 0)
            {
                var current = segments.Last().ToLower();

                if (current.Contains("welcome") || current.Contains("login"))
                    color = Colors.Transparent;
            }

            if (StatusBarBehavior != null)
            {
                StatusBarBehavior.StatusBarColor = color;
                StatusBarBehavior.StatusBarStyle = StatusBarStyle.DarkContent;
            }
        }


        private async Task InitializeAppAsync()
        {
            await SupabaseClient.InitializeAsync();

            if (SupabaseClient.Client.Auth.CurrentUser != null)
            {
                var userId = SupabaseClient.Client.Auth.CurrentUser.Id;

                var user = await SupabaseClient.Client
                    .From<User>()
                    .Where(u => u.Id == userId)
                    .Single();

                App.CurrentUser = user;

                // Homepage ertesitese 
                MessagingCenter.Send(this, "UserLoaded");
            }
            else
            {
                // nincs session -> WelcomePage
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    Application.Current.MainPage = new NavigationPage(new WelcomePage());
                });
            }
        }




    }

}
