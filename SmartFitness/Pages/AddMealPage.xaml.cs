using Supabase;
using System.Collections.ObjectModel;
using SmartFitness.Services;
using System.Windows.Input;
using SmartFitness.Models;
using FileResult = Microsoft.Maui.Storage.FileResult;
using System.Text;
using System.IO;

namespace SmartFitness.Pages;

public partial class AddMealPage : ContentPage
{
    private readonly Client _supabase = SupabaseClient.Client;
    private ObservableCollection<Entry> ingredientEntries = new();
    private FileResult selectedImageFile;
    private string imageUrl = null;

    public List<string> MealTypes { get; } = new List<string> { "BREAKFAST", "LUNCH", "DINNER" };

    public string AuthorName => App.CurrentUser != null
        ? $"{App.CurrentUser.FirstName} {App.CurrentUser.LastName}"
        : "Unknown";

    public AddMealPage()
    {
        InitializeComponent();
        BindingContext = this;

        MealTypePicker.ItemsSource = MealTypes;
        MealTypePicker.SelectedIndex = 0;
        ingredientEntries.Add(FirstIngredientEntry);
        AuthorLabel.Text = AuthorName;
    }

    private void NewIngredientButton_Clicked(object sender, EventArgs e)
    {
        var newEntry = new Entry
        {
            Placeholder = "•  Ingredient",
            PlaceholderColor = Colors.Black,
            FontSize = 18,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Start
        };
        IngredientsStack.Children.Insert(IngredientsStack.Children.Count - 1, newEntry);
        ingredientEntries.Add(newEntry);
    }

    private async void OnImageButton(object sender, EventArgs e)
    {
        try
        {
            selectedImageFile = await MediaPicker.PickPhotoAsync();

            if (selectedImageFile != null)
            {
                AddImageButton.Text = "Image Selected";
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Image selection failed: {ex.Message}", "OK");
        }
    }

    private async void SubmitButton_Clicked(object sender, EventArgs e)
    {
        try
        {

            //  Képfeltöltés csak itt történik meg
            if (selectedImageFile != null)
            {
                using var stream = await selectedImageFile.OpenReadAsync();
                using var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
                var bytes = memoryStream.ToArray();

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(selectedImageFile.FileName)}";
                var response = await _supabase.Storage
                    .From("recipes")
                    .Upload(bytes, fileName, new Supabase.Storage.FileOptions { Upsert = true });

                imageUrl = _supabase.Storage
                    .From("recipes")
                    .GetPublicUrl(fileName);
            }

            var meal = new Meal
            {
                UserId = App.CurrentUser.Id,
                Type = MealTypePicker.SelectedItem?.ToString() ?? "BREAKFAST",
                Title = TitleEntry.Text,
                CookingTime = int.TryParse(CookingTimeEntry.Text, out int time) ? time : 0,
                Nutrition = new Dictionary<string, int>
                {
                    { "calories", int.TryParse(CaloriesEntry.Text, out int cal) ? cal : 0 },
                    { "protein", int.TryParse(ProteinEntry.Text, out int prot) ? prot : 0 },
                    { "carbs", int.TryParse(CarbsEntry.Text, out int carb) ? carb : 0 },
                    { "fat", int.TryParse(FatEntry.Text, out int fat) ? fat : 0 }
                },
                Ingredients = ingredientEntries.Select(e => e.Text).ToArray(),
                Directions = DirectionsEditor.Text,
                TutorialUrl = TutorialEditor.Text,
                AuthorName = AuthorName,
                ImageUrl = imageUrl
            };

            await _supabase.From<Meal>().Insert(meal);
            //await Navigation.PushAsync(new FoodPage());
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to submit recipe: {ex.Message}", "OK");
        }
    }
}
