using SmartFitness.Models;
using SmartFitness.Services;
using SmartFitness.ViewModels; // Kell a ViewModelhez
using SmartFitness.Views;
using System.Collections.ObjectModel;

namespace SmartFitness.Pages;

public partial class WorkoutSearchPage : ContentPage
{
    private List<SearchItem> _allSourceItems;
    private List<SearchItem> _currentFilteredList;

    public ObservableCollection<SearchItem> DisplayedItems { get; set; } = new();

    private const int BatchSize = 15;
    private bool _isLoading = false;

    public WorkoutSearchPage(List<SearchItem> items)
    {
        InitializeComponent();
        _allSourceItems = items;
        _currentFilteredList = new List<SearchItem>(_allSourceItems);

        ResultsCollectionView.ItemsSource = DisplayedItems;

        LoadNextBatch();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        SearchEntry.Focus();
    }

    // --- NAVIGÁCIÓS LOGIKA ---
    private async void OnResultTapped(object sender, TappedEventArgs e)
    {
        if (sender is Border border && border.BindingContext is SearchItem selectedItem)
        {
            //  PROGRAM NAVIGÁCIÓ
            if (selectedItem.IsPlan)
            {
                var plan = selectedItem.OriginalData as WorkoutPlan;
                if (plan != null)
                {
                    await Navigation.PushAsync(new WorkoutPlanDetailPage(plan.Id, SupabaseClient.Client));
                }
            }
            // EDZÉS NAVIGÁCIÓ (ViewModel átadással)
            else
            {
                var workout = selectedItem.OriginalData as Workout;
                if (workout != null)
                {
                    // Létrehozzuk a ViewModel-t és átadjuk neki az adatot
                    var viewModel = new WorkoutDetailViewModel();
                    viewModel.Workout = workout;

                    // Átnavigálunk a Page-re
                    await Navigation.PushAsync(new WorkoutDetailPage(viewModel));
                }
            }
        }
    }

    private void OnTextChanged(object sender, TextChangedEventArgs e)
    {
        string term = e.NewTextValue?.ToLower()?.Trim();

        if (ClearButton != null)
            ClearButton.IsVisible = !string.IsNullOrEmpty(term);

        if (string.IsNullOrWhiteSpace(term))
        {
            _currentFilteredList = new List<SearchItem>(_allSourceItems);
        }
        else
        {
            _currentFilteredList = _allSourceItems.Where(x =>
                x.Title.ToLower().Contains(term) ||
                x.Subtitle.ToLower().Contains(term)
            ).ToList();
        }

        DisplayedItems.Clear();
        LoadNextBatch();
    }

    private void OnLoadMoreItems(object sender, EventArgs e)
    {
        LoadNextBatch();
    }

    private void LoadNextBatch()
    {
        if (_isLoading) return;
        _isLoading = true;

        try
        {
            int currentCount = DisplayedItems.Count;
            int totalCount = _currentFilteredList.Count;

            if (currentCount >= totalCount) return;

            int amountToTake = Math.Min(BatchSize, totalCount - currentCount);
            var nextBatch = _currentFilteredList.GetRange(currentCount, amountToTake);

            foreach (var item in nextBatch)
            {
                DisplayedItems.Add(item);
            }
        }
        finally
        {
            _isLoading = false;
        }
    }

    private void OnClearSearchClicked(object sender, EventArgs e)
    {
        SearchEntry.Text = string.Empty;
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}



public class SearchItem
{
    public string Title { get; set; }
    public string ImageUrl { get; set; }
    public string Subtitle { get; set; }
    public object OriginalData { get; set; }
    public bool IsPlan { get; set; }

    //BADGE
    public string TypeText => IsPlan ? "PROGRAM" : "WORKOUT";

    // Coral szín a programnak, Zöld az edzésnek
    public Color TypeColor => IsPlan ? Color.FromArgb("#FF7F50") : Color.FromArgb("#859700");

    public Color TypeTextColor => Colors.White;
}
