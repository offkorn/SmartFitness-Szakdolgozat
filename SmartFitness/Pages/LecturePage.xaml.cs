using Microsoft.Maui.Controls;
using SmartFitness.Models;

namespace SmartFitness.Pages
{
    public partial class LecturePage : ContentPage
    {
        public LecturePage()
        {
            InitializeComponent();
        }

        private async void OnMuscleBuildingClicked(object sender, EventArgs e)
        {
            
            await Navigation.PushAsync(new ArticleDetailPage(new ArticleData
            {
                PageTitle = "Building Muscle",
                Category = "Fitness",
                MainTitlePart1 = "Understanding Muscle",
                MainTitlePart2 = "Building",
                Description = "Muscle building, or hypertrophy, is the process of increasing muscle size through resistance training.",
                KeyTip1Title = "• Progressive Overload:",
                KeyTip1Text = "Gradually increase weight or reps.",
                KeyTip2Title = "• Nutrition:",
                KeyTip2Text = "Consume adequate protein (1.6-2.2g/kg body weight).",
                AdditionalInfo = "Start with compound exercises like squats and deadlifts. Aim for 3-5 sets of 8-12 reps."
            }));
        }

        // 2. Nutrition
        private async void OnNutritionClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ArticleDetailPage(new ArticleData
            {
                PageTitle = "Nutrition Basics",
                Category = "Diet",
                MainTitlePart1 = "Basics of",
                MainTitlePart2 = "Nutrition",
                Description = "Good nutrition is the foundation of a healthy lifestyle. It fuels your body and supports recovery.",
                KeyTip1Title = "• Macronutrients:",
                KeyTip1Text = "Balance your Protein, Carbs, and Fats intake.",
                KeyTip2Title = "• Hydration:",
                KeyTip2Text = "Drink at least 2-3 liters of water daily.",
                AdditionalInfo = "Avoid processed foods and focus on whole ingredients like vegetables, lean meats, and fruits."
            }));
        }

        // 3. Sleep
        private async void OnSleepClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ArticleDetailPage(new ArticleData
            {
                PageTitle = "Sleep Importance",
                Category = "Recovery",
                MainTitlePart1 = "Importance of",
                MainTitlePart2 = "Sleep",
                Description = "Sleep is when your muscles repair and grow. Without it, training is less effective.",
                KeyTip1Title = "• Duration:",
                KeyTip1Text = "Aim for 7-9 hours of quality sleep per night.",
                KeyTip2Title = "• Consistency:",
                KeyTip2Text = "Go to bed and wake up at the same time every day.",
                AdditionalInfo = "Avoid screens (blue light) at least 1 hour before bed to improve sleep quality."
            }));
        }

        // 4. Cardio 
        private async void OnCardioClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ArticleDetailPage(new ArticleData
            {
                PageTitle = "Cardio Basics",
                Category = "Endurance",
                MainTitlePart1 = "Cardio for",
                MainTitlePart2 = "Beginners",
                Description = "Cardiovascular exercise strengthens your heart and lungs and helps burn calories.",
                KeyTip1Title = "• Start Slow:",
                KeyTip1Text = "Begin with walking or light jogging for 20 minutes.",
                KeyTip2Title = "• Frequency:",
                KeyTip2Text = "Aim for 150 minutes of moderate activity per week.",
                AdditionalInfo = "Mix low-intensity steady state (LISS) with high-intensity interval training (HIIT) for best results."
            }));
        }

        // 5. Flexibility 
        private async void OnFlexibilityClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ArticleDetailPage(new ArticleData
            {
                PageTitle = "Mobility",
                Category = "Recovery",
                MainTitlePart1 = "Flexibility and",
                MainTitlePart2 = "Mobility",
                Description = "Good mobility prevents injuries and improves your performance in all exercises.",
                KeyTip1Title = "• Dynamic Stretching:",
                KeyTip1Text = "Do this before workout to warm up muscles.",
                KeyTip2Title = "• Static Stretching:",
                KeyTip2Text = "Do this after workout to improve flexibility.",
                AdditionalInfo = "Focus on tight areas like hips and shoulders, especially if you sit a lot during the day."
            }));
        }

        // 6. Mental Health
        private async void OnMentalHealthClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ArticleDetailPage(new ArticleData
            {
                PageTitle = "Mindset",
                Category = "Wellness",
                MainTitlePart1 = "Mental Health",
                MainTitlePart2 = "Basics",
                Description = "A healthy mind is just as important as a healthy body. Stress affects physical results.",
                KeyTip1Title = "• Mindfulness:",
                KeyTip1Text = "Practice meditation or deep breathing daily.",
                KeyTip2Title = "• Balance:",
                KeyTip2Text = "Listen to your body and don't overtrain.",
                AdditionalInfo = "Exercise is a natural anti-depressant, releasing endorphins that boost your mood."
            }));
        }
    }
}