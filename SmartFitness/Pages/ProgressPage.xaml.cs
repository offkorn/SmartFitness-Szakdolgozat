using Microsoft.Maui.Controls;
using SmartFitness.Models;
using SmartFitness.Services;
using System.Collections.ObjectModel;
using System.Linq;


namespace SmartFitness.Pages
{
    public partial class ProgressPage : ContentPage
    {
        public ObservableCollection<WeightModel> WeightData { get; set; }

        public ProgressPage()
        {
            InitializeComponent();

            WeightData = new ObservableCollection<WeightModel>();
            BindingContext = this;

           
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _ = LoadWeightHistoryAsync();
        }


        private async Task LoadWeightHistoryAsync()
        {
            var response = await SupabaseClient.Client
                .From<WeightHistoryModel>()
                .Where(x => x.UserId == App.CurrentUser.Id)
                .Order("created_at", Supabase.Postgrest.Constants.Ordering.Descending)
                .Limit(5)
                .Get();

            WeightData.Clear();

            var list = response.Models.ToList();
            list.Reverse(); // idõrend növekvõ sorrend

            foreach (var item in list)
            {
                WeightData.Add(new WeightModel(
                    item.CreatedAt,  
                    item.Weight
                ));
            }

        }


        private async void OnSettingsClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SettingsPage());
        }
    }

    public class WeightModel
    {
        public string DateLabel { get; set; }
        public DateTime? Timestamp { get; set; }
        public float Kg { get; set; }

        public WeightModel(DateTime? ts, float kg)
        {
            Timestamp = ts ?? DateTime.UtcNow; // fallback hogy ne essen szét
            DateLabel = ts?.ToString("MM.dd") ?? "--.--";
            Kg = kg;
        }
    }


}
