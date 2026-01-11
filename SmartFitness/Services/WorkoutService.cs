using SmartFitness.Models;

namespace SmartFitness.Services
{
    public static class WorkoutService
    {
        public static async Task SaveWorkoutAsync(string userId, string workoutName)
        {
            // Log mentése
            var log = new WorkoutLogModel
            {
                UserId = userId,
                WorkoutName = workoutName
            };

            await SupabaseClient.Client.From<WorkoutLogModel>().Insert(log);

            // Streak növelése
            await WorkoutStreakService.AddWorkoutToStreakAsync(userId);
        }
    }
}
