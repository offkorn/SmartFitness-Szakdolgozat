using System.Windows.Input;
using SmartFitness.Models;
using SmartFitness.Services;

namespace SmartFitness.Pages;

public partial class RecipeDetailPage : ContentPage
{
    private readonly Meal _meal;
    private bool _isSaved = false;
    private string _savedRecordId = null; // Ha törölni kell, tudjuk az ID-t

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

        CheckIfSaved();

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



    private async void OnBackButton(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }


    private async void CheckIfSaved()
    {
        if (App.CurrentUser == null) return;

        try
        {
            var response = await SupabaseClient.Client
                .From<SavedMeal>()
                .Where(x => x.UserId == App.CurrentUser.Id && x.MealId == _meal.Id)
                .Single();

            if (response != null)
            {
                _isSaved = true;
                _savedRecordId = response.Id;
                UpdateIcon();
            }
        }
        catch
        {
            // Nincs elmentve 
            _isSaved = false;
            UpdateIcon();
        }
    }

    private void UpdateIcon()
    {
        SaveButton.Source = _isSaved ? "bookmark_filled_icon.svg" : "bookmark_unfilled_icon.svg";
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (App.CurrentUser == null)
        {
            await DisplayAlert("Error", "Please log in to save recipes.", "OK");
            return;
        }

        SaveButton.IsEnabled = false; // Dupla kattintás elkerülése

        try
        {
            if (_isSaved)
            {
                // TÖRLÉS (Unsave)
                await SupabaseClient.Client
                    .From<SavedMeal>()
                    .Where(x => x.Id == _savedRecordId)
                    .Delete();

                _isSaved = false;
                _savedRecordId = null;
               // await DisplayAlert("Success", "Recipe removed from saved.", "OK");
            }
            else
            {
                // MENTÉS
                var newSave = new SavedMeal
                {
                    UserId = App.CurrentUser.Id,
                    MealId = _meal.Id
                };

                var response = await SupabaseClient.Client
                    .From<SavedMeal>()
                    .Insert(newSave);

                _isSaved = true;
                
                _savedRecordId = response.Models.FirstOrDefault()?.Id;

                //await DisplayAlert("Success", "Recipe saved!", "OK");
            }
            UpdateIcon();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Operation failed: {ex.Message}", "OK");
        }
        finally
        {
            SaveButton.IsEnabled = true;
        }
    }


}