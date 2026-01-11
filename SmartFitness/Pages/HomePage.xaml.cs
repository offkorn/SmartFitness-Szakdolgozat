using System;
using Microsoft.Maui.Controls;
using SmartFitness.Authentication;
using SmartFitness.Models;
using SmartFitness.components;
using SmartFitness.Services;
using SmartFitness.ViewModels;

namespace SmartFitness.Pages
{
    public partial class HomePage : ContentPage
    {
        private Workout _nextWorkoutData = null;
        public HomePage()
        {
            InitializeComponent();

            greetingPart1.Text = "Loading";
            greetingPart2.Text = "...";
            firstNameLabel.Text = "";

            // ha a user betölt frissítsük a UI-t
            MessagingCenter.Subscribe<AppShell>(this, "UserLoaded", async (sender) =>
            {
                SetGreetingMessage();
                LoadUserWeight();

                await LoadStreak();
                await LoadActiveProgram();

            });
        }

        private void SetGreetingMessage()
        {
            if (App.CurrentUser == null)
                return;

            string[] greetingParts = GetGreeting().Split(' ');
            greetingPart1.Text = greetingParts[0];
            greetingPart2.Text = greetingParts[1];
            firstNameLabel.Text = $"{App.CurrentUser.FirstName}!";
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            DailyQuote.RefreshQuote();
            LoadUserWeight();

            // ha még nincs bejelentkezett user, ne próbáljunk streaket / programot behozni
            if (App.CurrentUser == null)
            {
                StreakLabel.Text = "0";
                NextWorkoutLabel.Text = " - ";
                ProgramNameLabel.Text = "No active program";
                return;
            }

            await LoadStreak();
            await LoadActiveProgram();
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

        //streak betoltese
        private async Task LoadStreak()
        {
            if (App.CurrentUser == null)
            {
                StreakLabel.Text = "0";
                return;
            }

            var streak = await WorkoutStreakService.GetStreakAsync(App.CurrentUser.Id);
            StreakLabel.Text = streak?.CurrentStreak.ToString() ?? "0";
        }

        




        //wegiht tracking gomb

        private async void LoadUserWeight()
        {

            try
            {
                
                UserWeightLabel.Text = $"{App.CurrentUser.Weight} kg";
                
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load user weight: {ex.Message}", "OK");
            }
        }



        private async void MinusWeightButton_Clicked(object sender, EventArgs e)
        {
            App.CurrentUser.Weight -= 0.5f;
            UserWeightLabel.Text = $"{App.CurrentUser.Weight:F1} kg";

            await SaveWeightAsync(App.CurrentUser.Weight ?? 0);
        }


        private async void PlusWeightButton_Clicked(object sender, EventArgs e)
        {
            App.CurrentUser.Weight += 0.5f;
            UserWeightLabel.Text = $"{App.CurrentUser.Weight:F1} kg";

            await SaveWeightAsync(App.CurrentUser.Weight ?? 0);
        }

        private async Task SaveWeightAsync(float newWeight)
        {
            try
            {
                await SupabaseClient.Client
                    .From<User>()
                    .Where(x => x.Id == App.CurrentUser.Id)
                    .Set(x => x.Weight, newWeight)
                    .Update();

                App.CurrentUser.Weight = newWeight;

                var entry = new WeightHistoryModel
                {
                    UserId = App.CurrentUser.Id,
                    Weight = newWeight,
                    CreatedAt = DateTime.UtcNow   
                };
                
                await SupabaseClient.Client
                    .From<WeightHistoryModel>()
                    .Insert(entry);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to update weight: {ex.Message}", "OK");
            }
        }





        private async Task LoadActiveProgram()
        {
           

            if (App.CurrentUser == null)
            {
                ProgramFrame.IsVisible = false;
                NoProgramLabel.IsVisible = true;
                return;
            }

            var active = await ProgramService.GetActiveProgramAsync(App.CurrentUser.Id);

            if (active == null)
            {
                ProgramFrame.IsVisible = false;
                NoProgramLabel.IsVisible = true;
                return;
            }

            // Plan adatainak lekérése
            var plan = await SupabaseClient.Client
                .From<WorkoutPlan>()
                .Where(x => x.Id == active.PlanId)
                .Single();

            ProgramFrame.IsVisible = true;
            NoProgramLabel.IsVisible = false;

            ProgramNameLabel.Text = plan.Title;
            ProgramWeeksLabel.Text = $"{plan.Weeks} weeks";
            ProgramExperienceLabel.Text = plan.ExperienceLevel;

            // Következõ nap meghatározása 
            var today = DateTime.Now.DayOfWeek.ToString(); 
            string nextDayName = GetNextWorkoutDay(plan.Days, today);

            NextWorkoutLabel.Text = nextDayName;

            // a konkret edzes betoltese a naphoz
            if (nextDayName != "-")
            {
                try
                {
                   
                    var planWorkoutRelation = await SupabaseClient.Client
                        .From<PlanWorkout>()
                        .Where(x => x.PlanId == plan.Id && x.Day == nextDayName)
                        .Single();

                    if (planWorkoutRelation != null)
                    {
                        // Ha megvan a kapcsolat, lekérjük magát az edzést
                        var workout = await SupabaseClient.Client
                            .From<Workout>()
                            .Where(x => x.Id == planWorkoutRelation.WorkoutId)
                            .Single();

                        _nextWorkoutData = workout; 
                        ProgramNextWorkoutButton.IsEnabled = true;
                        ProgramNextWorkoutButton.Text = "Start: " + workout.Title; 
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to load the next Workout: {ex.Message}");
                    _nextWorkoutData = null;
                    ProgramNextWorkoutButton.IsEnabled = false; // Ha nincs adat, ne lehessen kattintani
                    ProgramNextWorkoutButton.Text = "Rest Day";
                }
            }
            else
            {
                _nextWorkoutData = null;
                ProgramNextWorkoutButton.Text = "No upcoming workouts";
                ProgramNextWorkoutButton.IsEnabled = false;
            }
        }

        // Nextworkout Button
        private async void OnNextWorkoutClicked(object sender, EventArgs e)
        {
            if (_nextWorkoutData != null)
            {
                var viewModel = new WorkoutDetailViewModel();
                viewModel.Workout = _nextWorkoutData;

                // Navigálunk a részletezõ oldalra
                await Navigation.PushAsync(new WorkoutDetailPage(viewModel));
            }
            else
            {
                await DisplayAlert("Info", "No workout data found for the next scheduled day.", "OK");
            }
        }

        // A nap kereso logika
        private string GetNextWorkoutDay(List<string> days, string today)
        {
            if (days == null || days.Count == 0) return "-";

            // Ha ma van edzés, akkor a "Next Workout" ma van. 
            if (days.Contains(today)) return today;

            int todayIndex = (int)DateTime.Now.DayOfWeek; 

            // A következõ 7 napot vizsgáljuk meg
            for (int i = 1; i <= 7; i++)
            {
                int nextIndex = (todayIndex + i) % 7;
                string nextDayName = ((DayOfWeek)nextIndex).ToString();

                if (days.Contains(nextDayName))
                {
                    return nextDayName;
                }
            }

            return "-";
        }




    }
}