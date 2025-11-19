using System.Windows.Input;
using SmartFitness.Models;

namespace SmartFitness.Pages;

public partial class RecipeDetailPage : ContentPage
{
    private readonly Meal _meal;

    public RecipeDetailPage(Meal meal)
    {
        InitializeComponent();
        _meal = meal;

        if (_meal.Nutrition == null)
        {
            _meal.Nutrition = new Dictionary<string, int>
            {
                { "calories", 0 },
                { "protein", 0 },
                { "carbs", 0 },
                { "fat", 0 }
            };
        }

        BindingContext = _meal;
        AuthorLabel.Text = _meal.AuthorName ?? "Unknown";

    }

    private async void OnURLClicked(object sender, EventArgs e)
    {
        try
        {
            string url = _meal.TutorialUrl;
            if (!string.IsNullOrEmpty(url))
            {
                if (!url.StartsWith("http://") && !url.StartsWith("https://"))
                {
                    url = "https://" + url;
                }
                await Launcher.OpenAsync(new Uri(url));
            }
            else
            {
                await DisplayAlert("Info", "No tutorial URL provided.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to open tutorial link: {ex.Message}", "OK");
        }
    }
}