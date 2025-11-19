using System;
using Microsoft.Maui.Controls;
using SmartFitness.Authentication;
using SmartFitness.Models;

namespace SmartFitness.Pages
{
    public partial class HomePage : ContentPage
    {
        public HomePage()
        {
            InitializeComponent();
            SetGreetingMessage();
        }

        private void SetGreetingMessage()
        {
            if (App.CurrentUser == null)
            {
                // Ha nincs bejelentkezett felhasználó, vissza a LoginPage-re
                if (Application.Current != null)
                {
                    Application.Current.MainPage = new NavigationPage(new WelcomePage());
                }
                return;
            }

            string[] greetingParts = GetGreeting().Split(' ');
            greetingPart1.Text = greetingParts[0]; // Good
            greetingPart2.Text = greetingParts[1]; // Morning/Afternoon/Evening
            firstNameLabel.Text = $"{App.CurrentUser.FirstName}!"; // A regisztráció során megadott FirstName
        }

        private string GetGreeting()
        {
            int hour = DateTime.Now.Hour;

            if (hour >= 5 && hour < 12)
                return "Good Morning";
            else if (hour >= 12 && hour < 18)
                return "Good Afternoon";
            else
                return "Good Evening";
        }
    }
}