using Microsoft.Maui.Controls;
using SmartFitness.Views;
using System;
using System.Drawing;
using System.Threading.Tasks;
using Color = Microsoft.Maui.Graphics.Color;


namespace SmartFitness.Pages
{
    public partial class MakeWorkoutPage : ContentPage
    {
        public MakeWorkoutPage()
        {
            InitializeComponent();

            // Alapból a SimpleWorkoutView töltődjön be
            workoutContentView.Content = new SimpleWorkoutView();
            //Content = new SimpleWorkoutView();


        }

        private async void SimpleWorkoutSelected(object sender, EventArgs e)
        {
            switchFrame.BackgroundColor = Colors.IndianRed;
            btnWorkout.BackgroundColor = Colors.White;
            btnPlan.BackgroundColor = Colors.Transparent;

            // Szövegszín váltás
            labelWorkout.TextColor = Colors.IndianRed;
            labelPlan.TextColor = Colors.White;

            labelPlan.FontFamily = "InriaSerifRegular";
            labelWorkout.FontFamily = "InriaSerifBold";

            // Tartalom frissítése
            workoutContentView.Content = new SimpleWorkoutView();
        }

        private async void PlanSelected(object sender, EventArgs e)
        {
            switchFrame.BackgroundColor = Color.Parse("#34B31D");
            btnWorkout.BackgroundColor = Colors.Transparent;
            btnPlan.BackgroundColor = Colors.White;


            // Szövegszín váltás
            labelWorkout.TextColor = Colors.White;
            labelPlan.TextColor = Color.Parse("#34B31D");

            labelPlan.FontFamily = "InriaSerifBold";
            labelWorkout.FontFamily = "InriaSerifRegular";

            // Tartalom frissítése
            workoutContentView.Content = new WorkoutProgramView();
        }








    }
}
