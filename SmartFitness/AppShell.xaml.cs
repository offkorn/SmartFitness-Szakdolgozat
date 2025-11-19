using CommunityToolkit.Maui.Behaviors;
using CommunityToolkit.Maui.Core;
using System.Runtime.Versioning;

namespace SmartFitness
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            UpdateStatusBarColor();
        }

        protected override void OnNavigated(ShellNavigatedEventArgs args)
        {
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

            // Kivétel: ha a WelcomePage vagy LoginPage az aktuális
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

    }
    
}
