using SmartFitness.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SmartFitness.Services
{
    public static class WorkoutStreakService
    {
        public static async Task<WorkoutStreakModel?> GetStreakAsync(string userId)
        {
            var response = await SupabaseClient.Client
                .From<WorkoutStreakModel>()
                .Where(x => x.UserId == userId)
                .Get();

            return response.Models.FirstOrDefault();
        }

        public static async Task<int> AddWorkoutToStreakAsync(string userId)
        {
            var today = DateTime.UtcNow.Date;
            var yesterday = today.AddDays(-1);

            var streak = await GetStreakAsync(userId);

            if (streak == null)
            {
                // usernek még nincs streak rekordja, létrehozzuk
                streak = new WorkoutStreakModel
                {
                    UserId = userId,
                    CurrentStreak = 1,
                    LastWorkoutDate = today
                };

                await SupabaseClient.Client.From<WorkoutStreakModel>().Insert(streak);
                return streak.CurrentStreak;
            }

            // Ha ma már volt workout ne növelje újra
            if (streak.LastWorkoutDate == today)
                return streak.CurrentStreak;

            // Folytonos streak tegnap is volt
            if (streak.LastWorkoutDate == yesterday)
                streak.CurrentStreak++;
            else
                streak.CurrentStreak = 1; // megszakadt, új streak

            streak.LastWorkoutDate = today;

            await SupabaseClient.Client
                .From<WorkoutStreakModel>()
                .Where(x => x.Id == streak.Id)
                .Update(streak);

            return streak.CurrentStreak;
        }
    }
}
