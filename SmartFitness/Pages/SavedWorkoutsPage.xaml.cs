using SmartFitness.Models;
using SmartFitness.ViewModels;
using System.Collections.ObjectModel;

namespace SmartFitness.Pages;

public partial class SavedWorkoutsPage : ContentPage
{
    private List<Workout> _allSourceWorkouts;
    private List<Workout> _currentFilteredList;
    public ObservableCollection<Workout> DisplayedWorkouts { get; set; } = new();

    private const int BatchSize = 15;
    private bool _isLoading = false;

    public SavedWorkoutsPage(List<Workout> workouts)
    {
        InitializeComponent();
        _allSourceWorkouts = workouts;
        _currentFilteredList = new List<Workout>(_allSourceWorkouts);

        ResultsCollectionView.ItemsSource = DisplayedWorkouts;

        LoadNextBatch();
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
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
            int currentCount = DisplayedWorkouts.Count;
            int totalCount = _currentFilteredList.Count;

            if (currentCount >= totalCount) return;

            int amountToTake = Math.Min(BatchSize, totalCount - currentCount);
            var nextBatch = _currentFilteredList.GetRange(currentCount, amountToTake);

            foreach (var w in nextBatch)
            {
                DisplayedWorkouts.Add(w);
            }
        }
        finally
        {
            _isLoading = false;
        }
    }

    private async void OnResultTapped(object sender, TappedEventArgs e)
    {
        
        if (sender is Element element && element.BindingContext is Workout selectedWorkout)
        {
           
            var viewModel = new WorkoutDetailViewModel();

            viewModel.Workout = selectedWorkout;

            await Navigation.PushAsync(new WorkoutDetailPage(viewModel));
        }
    }
}